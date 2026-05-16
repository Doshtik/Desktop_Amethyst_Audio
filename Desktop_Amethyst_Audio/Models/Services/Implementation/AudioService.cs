using System.IO;
using Desktop_Amethyst_Audio.Models.Services.Abstraction;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Desktop_Amethyst_Audio.Models.Services.Implementation;

public class AudioService : IAudioService, IDisposable
{
    private const int ChunkSize = 8192;
    private const int BufferSeconds = 4;
    private const int FftLen = 2048;
    private const int FftM = 11; // 2^11 = 2048

    private WaveOutEvent? _out;
    private BufferedWaveProvider? _buffer;
    private VolumeSampleProvider? _volumeProvider;
    private SampleGrabber? _grab;
    private CancellationTokenSource? _cts;
    private Task? _streamTask;
    private bool _isDisposed;

    public PlaybackState State => _out?.PlaybackState ?? PlaybackState.Stopped;
    public event Action? PlaybackEnded;

    /// <summary>
    /// Инициализирует движок для потокового воспроизведения.
    /// WaveFormat ДОЛЖЕН совпадать с декодированным PCM, который отдаёт сервер.
    /// </summary>
    public void Initialize(WaveFormat format)
    {
        if (_buffer != null) throw new InvalidOperationException("AudioEngine already initialized.");

        _buffer = new BufferedWaveProvider(format)
        {
            BufferDuration = TimeSpan.FromSeconds(BufferSeconds),
            DiscardOnBufferOverflow = false,
            ReadFully = true
        };

        // Цепочка: BufferedWaveProvider -> SampleProvider -> Volume -> Grabber -> WaveOut
        var sampleProvider = _buffer.ToSampleProvider();
        _volumeProvider = new VolumeSampleProvider(sampleProvider) { Volume = 1f };
        _grab = new SampleGrabber(_volumeProvider);

        _out = new WaveOutEvent { DesiredLatency = 100 };
        _out.Init(_grab);
        _out.PlaybackStopped += (_, _) => PlaybackEnded?.Invoke();
    }

    /// <summary>
    /// Запускает фоновую загрузку аудио из сетевого потока в буфер и начинает воспроизведение.
    /// </summary>
    public async Task StartAsync(Stream networkStream, CancellationToken ct = default)
    {
        if (_buffer == null) throw new InvalidOperationException("Call Initialize(format) first.");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        _streamTask = ReadAndBufferAsync(networkStream, _cts.Token);
        _out?.Play();
    }

    private async Task ReadAndBufferAsync(Stream stream, CancellationToken ct)
    {
        var readBuffer = new byte[ChunkSize];
        int bytesRead;
        try
        {
            while ((bytesRead = await stream.ReadAsync(readBuffer, ct)) > 0)
            {
                // Backpressure: ждём, если буфер заполнен
                while (_buffer.BufferedDuration >= _buffer.BufferDuration && !ct.IsCancellationRequested)
                    await Task.Delay(50, ct);

                _buffer.AddSamples(readBuffer, 0, bytesRead);
            }
        }
        catch (OperationCanceledException) { }
        catch (ObjectDisposedException) { }
        finally
        {
            stream.Dispose();
        }
    }

    public void Play() => _out?.Play();
    public void Pause() => _out?.Pause();
    public void Stop() => _out?.Stop();

    /// <summary>
    /// Перемотка в чистом потоковом режиме не поддерживается без HTTP Range на сервере.
    /// Метод оставлен для совместимости интерфейса.
    /// </summary>
    public void Seek(double frac) { /* Requires server-side Range support */ }

    public void SetVolume(float v)
    {
        if (_volumeProvider != null)
            _volumeProvider.Volume = Math.Clamp(v, 0f, 1f);
    }

    public bool GetSpectrum(float[] dest, int bars) => _grab?.FillBars(dest, bars) ?? false;

    public void Dispose()
    {
        if (_isDisposed) return;
        _isDisposed = true;
        _cts?.Cancel();
        _streamTask?.Wait(1000);
        _out?.Stop(); _out?.Dispose(); _out = null;
        _buffer = null;
        _cts?.Dispose();
    }

    // =====================================================================
    // SampleGrabber: intercepts PCM, mixes to mono, runs FFT, exposes bars
    // =====================================================================
    private sealed class SampleGrabber : ISampleProvider
    {
        private readonly ISampleProvider _src;
        private readonly int _ch;
        private readonly float _rate;

        private readonly float[] _ring = new float[FftLen];
        private int _wp;

        private readonly Complex[] _cpx = new Complex[FftLen];
        private readonly float[] _bands = new float[FftLen / 2];
        private readonly object _lock = new();

        private float _peakDb = -80f;
        private volatile bool _fftReady;

        public WaveFormat WaveFormat => _src.WaveFormat;

        public SampleGrabber(ISampleProvider src)
        {
            _src = src;
            _ch = src.WaveFormat.Channels;
            _rate = src.WaveFormat.SampleRate;
        }

        public int Read(float[] buf, int off, int count)
        {
            int n = _src.Read(buf, off, count);

            // Mix to mono and fill ring buffer
            for (int i = 0; i < n; i += _ch)
            {
                float s = buf[off + i];
                if (_ch >= 2 && i + 1 < n)
                    s = (s + buf[off + i + 1]) * 0.5f;

                _ring[_wp] = s;
                _wp++;
                if (_wp >= FftLen)
                {
                    _wp = 0;
                    DoFft();
                }
            }
            return n;
        }

        private void DoFft()
        {
            for (int i = 0; i < FftLen; i++)
            {
                float w = 0.54f - 0.46f * MathF.Cos(2f * MathF.PI * i / (FftLen - 1));
                _cpx[i].X = _ring[i] * w;
                _cpx[i].Y = 0;
            }

            FastFourierTransform.FFT(true, FftM, _cpx);

            lock (_lock)
            {
                float norm = 2f / FftLen;
                for (int i = 0; i < FftLen / 2; i++)
                {
                    float mag = MathF.Sqrt(_cpx[i].X * _cpx[i].X + _cpx[i].Y * _cpx[i].Y) * norm;
                    _bands[i] = mag;
                }
                _fftReady = true;
            }
        }

        public bool FillBars(float[] dest, int bars)
        {
            if (!_fftReady) { Array.Clear(dest); return false; }
            lock (_lock)
            {
                float nyq = _rate / 2f;
                double logLo = Math.Log10(30);
                double logHi = Math.Log10(Math.Min(18000, nyq));

                float maxMag = 0;
                for (int i = 1; i < FftLen / 2; i++)
                    if (_bands[i] > maxMag) maxMag = _bands[i];

                float curDb = maxMag > 0 ? 20f * MathF.Log10(maxMag) : -100f;
                if (curDb > _peakDb)
                    _peakDb += (curDb - _peakDb) * 0.3f;
                else
                    _peakDb += (curDb - _peakDb) * 0.002f;

                float refDb = MathF.Max(_peakDb, -10f);
                float floor = refDb - 60f;

                for (int i = 0; i < bars; i++)
                {
                    double f0 = Math.Pow(10, logLo + (logHi - logLo) * i / bars);
                    double f1 = Math.Pow(10, logLo + (logHi - logLo) * (i + 1) / bars);

                    int b0 = Math.Clamp((int)(f0 * FftLen / _rate), 1, FftLen / 2 - 1);
                    int b1 = Math.Clamp((int)(f1 * FftLen / _rate), b0 + 1, FftLen / 2);

                    float sum = 0;
                    float mx = 0;
                    int cnt = 0;
                    for (int j = b0; j < b1; j++)
                    {
                        sum += _bands[j] * _bands[j];
                        if (_bands[j] > mx) mx = _bands[j];
                        cnt++;
                    }
                    float rms = cnt > 0 ? MathF.Sqrt(sum / cnt) : 0;
                    float val = rms * 0.6f + mx * 0.4f;

                    float db = val > 1e-10f ? 20f * MathF.Log10(val) : -100f;
                    float norm01 = (db - floor) / (refDb - floor);
                    norm01 = MathF.Max(norm01, 0f);

                    dest[i] = MathF.Pow(norm01, 0.4f);
                }
            }
            return true;
        }
    }
}