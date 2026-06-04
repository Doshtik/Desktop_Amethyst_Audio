using System.Windows;
using System.Windows.Media;
using Desktop_Amethyst_Audio.Models.Services.Implementation;
using NAudio.Wave;

namespace Desktop_Amethyst_Audio.Views.CustomElements.WaveVisualizer;

public enum VisMode { Ambient, Equalizer }
public enum VisStyle { Sphere, Ring, Bars }
public enum VisColor { Amethyst, Ocean, Sunset, Rainbow }

public class WaveRenderer : FrameworkElement
{
    private const int Bars = 64;
    private const int BlobCount = 5;
    private const int ParticleCount = 36;

    public AudioService? Audio { get; set; }

    public VisMode Mode { get; set; } = VisMode.Equalizer;

    public VisStyle Style { get; set; } = VisStyle.Ring;
    public VisColor Color { get; set; } = VisColor.Amethyst;
    public double Sensitivity { get; set; } = 1.3;
    public double BassBoost { get; set; } = 1.2;
    public double AnimSpeed { get; set; } = 1.0;
    public double Glow { get; set; } = 1.0;
    public double SphereScale { get; set; } = 1.0;
    public bool ShowRays { get; set; } = true;
    public bool ShowParticles { get; set; } = true;
    public bool ShowOrbits { get; set; } = true;

    private double _t;
    private DateTime _last = DateTime.Now;
    private readonly Random _rng = new(42);

    private readonly float[] _raw = new float[Bars];
    private readonly double[] _val = new double[Bars];
    private readonly double[] _smooth = new double[Bars];
    private readonly double[] _peak = new double[Bars];
    private double _bass, _mid, _high, _energy;

    private double _bassAvg;
    private double _beat;
    private bool _wasPlaying;

    private struct Blob { public double X, Y, Vx, Vy, R, Hue, Phase, Speed; }
    private readonly Blob[] _blobs = new Blob[BlobCount];

    private struct Ptc { public double X, Y, Vx, Vy, Life, MaxL, Sz, Hue, A; }
    private readonly Ptc[] _pts = new Ptc[ParticleCount];

    private static readonly Brush OvBr = Fb(System.Windows.Media.Color.FromArgb(24, 0, 0, 0));

    private readonly DrawingVisual _vis = new();
    protected override int VisualChildrenCount => 1;
    protected override Visual GetVisualChild(int i) => _vis;

    public WaveRenderer()
    {
        Width = 560;
        Height = 560;
        AddVisualChild(_vis);
        AddLogicalChild(_vis);
        InitBlobs();
        for (int i = 0; i < ParticleCount; i++) Respawn(ref _pts[i], true);
        CompositionTarget.Rendering += OnRender;
    }

    private (double baseHue, double span) Scheme() => Color switch
    {
        VisColor.Ocean => (200, 45),
        VisColor.Sunset => (15, 40),
        VisColor.Rainbow => (0, 360),
        _ => (271, 32),
    };

    private void InitBlobs()
    {
        for (int i = 0; i < BlobCount; i++)
            _blobs[i] = new Blob
            {
                X = _rng.NextDouble() * 1920,
                Y = _rng.NextDouble() * 1080,
                Vx = (_rng.NextDouble() - 0.5) * 0.035,
                Vy = (_rng.NextDouble() - 0.5) * 0.035,
                R = 240 + _rng.NextDouble() * 130,
                Hue = _rng.NextDouble(),
                Phase = _rng.NextDouble() * Math.PI * 2,
                Speed = 0.0015 + _rng.NextDouble() * 0.003,
            };
    }

    private void Respawn(ref Ptc p, bool rndAge)
    {
        double a = _rng.NextDouble() * Math.PI * 2;
        double d = 80 + _rng.NextDouble() * 70;
        p.X = Math.Cos(a) * d; p.Y = Math.Sin(a) * d;
        double sp = 0.10 + _rng.NextDouble() * 0.4;
        p.Vx = Math.Cos(a) * sp + (_rng.NextDouble() - 0.5) * 0.2;
        p.Vy = Math.Sin(a) * sp + (_rng.NextDouble() - 0.5) * 0.2;
        p.MaxL = 2500 + _rng.NextDouble() * 4000;
        p.Life = rndAge ? _rng.NextDouble() * p.MaxL : 0;
        p.Sz = 0.9 + _rng.NextDouble() * 2.4;
        p.Hue = _rng.NextDouble();
        p.A = 0.22 + _rng.NextDouble() * 0.45;
    }

    private void OnRender(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        double dt = Math.Min((now - _last).TotalMilliseconds, 50);
        _last = now;
        double w = ActualWidth, h = ActualHeight;
        if (w < 1 || h < 1) return;
        _t += dt * AnimSpeed;

        bool playing = Audio?.State == PlaybackState.Playing;
        bool gotFFT = playing && Audio!.GetSpectrum(_raw, Bars);

        if (playing && !_wasPlaying) { _bassAvg = 0; _beat = 0; }
        _wasPlaying = playing;

        double rise = 1 - Math.Pow(0.06, dt / 16.0);
        double fall = 1 - Math.Pow(0.30, dt / 16.0);
        for (int i = 0; i < Bars; i++)
        {
            double tgt = gotFFT
                ? Math.Min(1.0, _raw[i] * Sensitivity)
                : 0.05 + 0.03 * Math.Sin(_t * 0.001 + i * 0.13) * (1 - (double)i / Bars * 0.4);
            double k = tgt > _val[i] ? rise : fall;
            _val[i] += (tgt - _val[i]) * k;
            if (_val[i] > _peak[i]) _peak[i] = _val[i];
            else _peak[i] *= Math.Pow(0.992, dt / 16.0);
        }

        for (int i = 0; i < Bars; i++)
        {
            double a = _val[(i - 1 + Bars) % Bars];
            double b = _val[i];
            double c = _val[(i + 1) % Bars];
            _smooth[i] = (a + 2 * b + c) * 0.25;
        }

        _bass = Avg(0, 8); _mid = Avg(8, 30); _high = Avg(30, Bars);
        double tE = _bass * 0.55 + _mid * 0.3 + _high * 0.15;
        _energy += (tE - _energy) * (1 - Math.Pow(0.10, dt / 16.0));

        _bassAvg += (_bass - _bassAvg) * (1 - Math.Pow(0.4, dt / 16.0));
        if (gotFFT && _bass > _bassAvg * 1.4 + 0.05 && _beat < 0.3)
            _beat = 1.0;
        _beat *= Math.Pow(0.92, dt / 16.0);

        double react = gotFFT ? 1.0 : 0.0;
        double beat = _beat * BassBoost;

        double breathe = 0.5 + 0.5 * Math.Sin(_t * 0.0011);
        double p2 = 0.5 + 0.5 * Math.Sin(_t * 0.0043);
        double p3 = 0.5 + 0.5 * Math.Sin(_t * 0.0021 + 1.5);

        double drive = breathe * (1 - react) + (_energy * 1.3 + beat * 0.35) * react;

        double cx = w / 2, cy = h / 2;

        using var dc = _vis.RenderOpen();
        //DrawBlobs(dc, dt, w, h, drive, react);
        if (ShowRays) DrawRays(dc, cx, cy, drive, beat, react);
        if (ShowOrbits) DrawOrbits(dc, cx, cy, p2, react);
        if (ShowParticles) DrawParticles(dc, cx, cy, dt, react, beat);

        if (Style == VisStyle.Ring)
            DrawRingBars(dc, cx, cy, drive, react);
        else if (Style == VisStyle.Bars)
            DrawLinearBars(dc, w, h, react);

        DrawSphere(dc, cx, cy, drive, beat, p2, p3, react);
    }

    private double Avg(int lo, int hi) { double s = 0; for (int i = lo; i < hi; i++) s += _val[i]; return s / (hi - lo); }

    private double H(double pos)
    {
        var (baseHue, span) = Scheme();
        double rot = Color == VisColor.Rainbow ? _t * 0.02 : 0;
        return baseHue + pos * span + rot;
    }

    private void DrawBlobs(DrawingContext dc, double dt, double w, double h, double drive, double react)
    {
        double speed = 1 + (_energy * 1.6) * react;
        for (int i = 0; i < BlobCount; i++)
        {
            ref var b = ref _blobs[i];
            b.X += b.Vx * dt * speed; b.Y += b.Vy * dt * speed;
            b.Phase += b.Speed * dt * (1 + _energy * react);
            if (b.X < -b.R) b.X = w + b.R; if (b.X > w + b.R) b.X = -b.R;
            if (b.Y < -b.R) b.Y = h + b.R; if (b.Y > h + b.R) b.Y = -b.R;

            double r = b.R + Math.Sin(b.Phase) * b.R * 0.18;
            double hue = H(b.Hue);
            double bri = 0.20 + drive * 0.16;

            var grd = new RadialGradientBrush
            {
                GradientStops =
                {
                    new GradientStop(Hsl(hue, 0.7, 0.5, bri), 0),
                    new GradientStop(Hsl(hue, 0.7, 0.45, bri * 0.18), 0.55),
                    new GradientStop(Hsl(hue, 0.7, 0.45, 0), 1),
                }
            };
            grd.Freeze();
            dc.DrawEllipse(grd, null, new Point(b.X, b.Y), r, r);
        }
    }

    private void DrawRays(DrawingContext dc, double cx, double cy, double drive, double beat, double react)
    {
        const int N = 14;
        double inner = 105;
        double outer = 210 + drive * 55;
        for (int i = 0; i < N; i++)
        {
            double a = (double)i / N * Math.PI * 2 + _t * 0.00018;
            double hue = H((double)i / N);
            double cos = Math.Cos(a), sin = Math.Sin(a);
            var p1 = new Point(cx + cos * inner, cy + sin * inner);
            var p2 = new Point(cx + cos * outer, cy + sin * outer);

            double glowA = (0.03 + drive * 0.04) * Glow;
            var glow = new Pen(Fb(Hsl(hue, 0.55, 0.55, Math.Clamp(glowA, 0, 1))), 16 + drive * 10)
            { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            glow.Freeze();
            dc.DrawLine(glow, p1, p2);

            double coreA = (0.07 + drive * 0.10) * Glow;
            var core = new Pen(Fb(Hsl(hue, 0.7, 0.7, coreA)), 2)
            { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            core.Freeze();
            dc.DrawLine(core, p1, p2);
        }
    }

    private void DrawOrbits(DrawingContext dc, double cx, double cy, double p2, double react)
    {
        for (int r = 0; r < 3; r++)
        {
            double rot = _t * (0.00018 + r * 0.0001) * (r % 2 == 0 ? 1 : -1);
            double rR = (165 + r * 52 + p2 * 10 + _energy * react * 35) * (0.85 + SphereScale * 0.15);
            double hue = H(0.4 + r * 0.1);
            double al = (0.05 + p2 * 0.07) * Glow;

            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                int pts = 80;
                for (int i = 0; i <= pts; i++)
                {
                    double a = (double)i / pts * Math.PI * 2 + rot;
                    double wob = Math.Sin(a * 6 + _t * 0.003 + r * 2) * (3.5 + p2 * 7);
                    var pt = new Point(cx + Math.Cos(a) * (rR + wob), cy + Math.Sin(a) * (rR + wob));
                    if (i == 0) ctx.BeginFigure(pt, false, false);
                    else ctx.LineTo(pt, true, false);
                }
            }
            geo.Freeze();
            var pen = new Pen(Fb(Hsl(hue, 0.55, 0.6, al)), 0.9 + p2 * 0.5);
            pen.Freeze();
            dc.DrawGeometry(null, pen, geo);
        }
    }

    private void DrawParticles(DrawingContext dc, double cx, double cy, double dt, double react, double beat)
    {
        double boost = 1 + react * (_energy * 2.5 + beat * 2);
        for (int i = 0; i < ParticleCount; i++)
        {
            ref var p = ref _pts[i];
            p.Life += dt;
            if (p.Life > p.MaxL) { Respawn(ref p, false); continue; }

            p.X += p.Vx * dt * 0.05 * boost;
            p.Y += p.Vy * dt * 0.05 * boost;

            double lf = p.Life / p.MaxL;
            double fade = lf < 0.12 ? lf / 0.12 : lf > 0.7 ? (1 - lf) / 0.3 : 1.0;
            double alpha = p.A * fade;
            if (alpha < 0.01) continue;

            double hue = H(p.Hue);
            double sz = p.Sz * (0.7 + 0.3 * Math.Sin(_t * 0.004 + i)) * (1 + beat * 0.4 * react);

            dc.DrawEllipse(Fb(Hsl(hue, 0.7, 0.8, alpha)), null,
                new Point(cx + p.X, cy + p.Y), sz, sz);
        }
    }

    private void DrawRingBars(DrawingContext dc, double cx, double cy, double drive, double react)
    {
        double R = 92 + drive * 20;
        double inner = R + 16;
        double maxH = 46 + 52 * Sensitivity;

        for (int i = 0; i < Bars; i++)
        {
            double ang = (double)i / Bars * Math.PI * 2 - Math.PI / 2;
            double v = Math.Pow(_smooth[i], 1.15);
            double bh = Math.Max(1.5, v * maxH);
            double hue = H((double)i / Bars);
            double alpha = 0.28 + v * 0.5;

            double cos = Math.Cos(ang), sin = Math.Sin(ang);
            double barW = Math.Max(1.8, Math.PI * 2 * inner / Bars * 0.38);

            var pen = new Pen(Fb(Hsl(hue, 0.72, 0.62, alpha)), barW)
            {
                StartLineCap = PenLineCap.Round,
                EndLineCap = PenLineCap.Round
            };
            pen.Freeze();
            dc.DrawLine(pen,
                new Point(cx + cos * inner, cy + sin * inner),
                new Point(cx + cos * (inner + bh), cy + sin * (inner + bh)));
        }
    }

    private void DrawLinearBars(DrawingContext dc, double w, double h, double react)
    {
        double baseY = h * 0.82;
        double maxH = h * 0.32 * Sensitivity;
        double bw = w / Bars;
        for (int i = 0; i < Bars; i++)
        {
            double bh = Math.Max(2, _smooth[i] * maxH);
            double hue = H((double)i / Bars);
            double alpha = 0.4 + _smooth[i] * 0.6;
            double x = i * bw + bw * 0.15;
            double bwi = bw * 0.7;
            var br = new LinearGradientBrush
            {
                StartPoint = new Point(0, 1), EndPoint = new Point(0, 0),
                GradientStops =
                {
                    new GradientStop(Hsl(hue, 0.8, 0.45, alpha * 0.5), 0),
                    new GradientStop(Hsl(hue, 0.85, 0.65, alpha), 1),
                }
            };
            br.Freeze();
            dc.DrawRoundedRectangle(br, null, new Rect(x, baseY - bh, bwi, bh), 3, 3);
        }
    }

    private void DrawSphere(DrawingContext dc, double cx, double cy,
        double drive, double beat, double p2, double p3, double react)
    {
        double R = (82 + drive * 20 + beat * 16 * react + _bass * react * 12) * SphereScale;
        var (baseHue, _) = Scheme();
        double hue1 = baseHue + 15 * Math.Sin(_t * 0.0005);
        double hue2 = hue1 + 22;
        double hue3 = hue1 - 22;

        for (int k = 4; k >= 1; k--)
        {
            double pr = R + k * (20 + drive * 16 + beat * 12 * react);
            double a = (1 - k / 5.0) * 0.12 * (0.5 + drive * 0.7 + beat * 0.4 * react) * Glow;
            var gb = new RadialGradientBrush
            {
                GradientStops =
                {
                    new GradientStop(System.Windows.Media.Color.FromArgb(0, 138, 43, 226), 0),
                    new GradientStop(Hsl(hue1 + k * 5, 0.75, 0.55, Math.Max(0, a)), 0.5),
                    new GradientStop(System.Windows.Media.Color.FromArgb(0, 80, 30, 160), 1),
                }
            };
            gb.Freeze();
            dc.DrawEllipse(gb, null, new Point(cx, cy), pr, pr);
        }

        double haloA = (0.15 + drive * 0.2 + beat * 0.2 * react) * Glow;
        var halo = new RadialGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Hsl(hue1, 0.82, 0.6, haloA), 0),
                new GradientStop(Hsl(hue2, 0.75, 0.45, haloA * 0.3), 0.45),
                new GradientStop(System.Windows.Media.Color.FromArgb(0, 40, 18, 90), 1),
            }
        };
        halo.Freeze();
        dc.DrawEllipse(halo, null, new Point(cx, cy), R * 2.4, R * 2.4);

        int pts = 140;
        const int FormBins = 20;
        var geo = new StreamGeometry();
        using (var ctx = geo.Open())
        {
            for (int i = 0; i <= pts; i++)
            {
                double a = (double)i / pts * Math.PI * 2;
                double d;
                if (react > 0.5)
                {
                    double fpos = (double)i / pts * FormBins;
                    int bi = (int)fpos % FormBins;
                    double frac = fpos - Math.Floor(fpos);
                    double s0 = _smooth[bi];
                    double s1 = _smooth[(bi + 1) % FormBins];
                    double sv = s0 + (s1 - s0) * frac;
                    d = sv * 18 * Sensitivity + _bass * 8
                      + Math.Sin(a * 3 + _t * 0.0013) * 4 * (0.4 + _energy);
                }
                else
                {
                    d = Math.Sin(a * 3 + _t * 0.0014) * 6 * (0.5 + drive * 0.8)
                      + Math.Sin(a * 5 - _t * 0.002) * 4 * (0.3 + p2 * 0.5)
                      + Math.Sin(a * 7 + _t * 0.0033) * 2.5 * p3;
                }
                double rr = R + d;
                var pt = new Point(cx + Math.Cos(a) * rr, cy + Math.Sin(a) * rr);
                if (i == 0) ctx.BeginFigure(pt, true, true);
                else ctx.LineTo(pt, true, true);
            }
        }
        geo.Freeze();

        double ox = 0.30 + Math.Sin(_t * 0.0009) * 0.08;
        double oy = 0.28 + Math.Cos(_t * 0.0012) * 0.06;
        var body = new RadialGradientBrush
        {
            Center = new Point(0.5, 0.5),
            GradientOrigin = new Point(ox, oy),
            RadiusX = 0.5, RadiusY = 0.5,
            GradientStops =
            {
                new GradientStop(Hsl(hue3, 0.6, 0.92, 0.97), 0),
                new GradientStop(Hsl(hue2, 0.72, 0.7, 0.95), 0.2),
                new GradientStop(Hsl(hue1, 0.85, 0.52, 0.93), 0.55),
                new GradientStop(Hsl(hue1 - 30, 0.78, 0.22, 0.9), 1),
            }
        };
        body.Freeze();
        dc.DrawGeometry(body, null, geo);

        double sx = 0.32 + Math.Sin(_t * 0.0007) * 0.06;
        double sy = 0.28 + Math.Cos(_t * 0.0009) * 0.05;
        var spec = new RadialGradientBrush
        {
            Center = new Point(sx, sy), GradientOrigin = new Point(sx, sy),
            RadiusX = 0.38, RadiusY = 0.38,
            GradientStops =
            {
                new GradientStop(System.Windows.Media.Color.FromArgb(170, 255, 255, 255), 0),
                new GradientStop(System.Windows.Media.Color.FromArgb(25, 255, 255, 255), 0.38),
                new GradientStop(System.Windows.Media.Color.FromArgb(0, 255, 255, 255), 1),
            }
        };
        spec.Freeze();
        dc.DrawGeometry(spec, null, geo);

        var rim = new RadialGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Colors.Transparent, 0),
                new GradientStop(Colors.Transparent, 0.70),
                new GradientStop(Hsl(hue1 + 35, 0.82, 0.66, 0.4 + drive * 0.16 + beat * 0.16 * react), 1),
            }
        };
        rim.Freeze();
        dc.DrawGeometry(rim, null, geo);

        double gs = (0.10 + drive * 0.15 + beat * 0.16 * react) * Glow;
        var gp = new Pen(Fb(Hsl(hue1, 0.78, 0.72, Math.Clamp(gs, 0, 1))), 2 + drive * 3 + beat * 3 * react)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round
        };
        gp.Freeze();
        dc.DrawGeometry(null, gp, geo);
    }

    private static readonly RadialGradientBrush VigBr;
    static WaveRenderer()
    {
        var v = new RadialGradientBrush
        {
            GradientStops =
            {
                new GradientStop(System.Windows.Media.Color.FromArgb(0, 0, 0, 0), 0.32),
                new GradientStop(System.Windows.Media.Color.FromArgb(105, 0, 0, 0), 1),
            }
        };
        v.Freeze();
        VigBr = v;
    }
    private void DrawVignette(DrawingContext dc, double w, double h)
        => dc.DrawRectangle(VigBr, null, new Rect(0, 0, w, h));

    private static Brush Fb(System.Windows.Media.Color c) { var b = new SolidColorBrush(c); b.Freeze(); return b; }
    private static System.Windows.Media.Color Hsl(double h, double s, double l, double a)
    {
        h %= 360; if (h < 0) h += 360;
        s = Math.Clamp(s, 0, 1); l = Math.Clamp(l, 0, 1);
        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs(h / 60.0 % 2 - 1));
        double m = l - c / 2;
        double r, g, b2;
        if (h < 60) { r = c; g = x; b2 = 0; }
        else if (h < 120) { r = x; g = c; b2 = 0; }
        else if (h < 180) { r = 0; g = c; b2 = x; }
        else if (h < 240) { r = 0; g = x; b2 = c; }
        else if (h < 300) { r = x; g = 0; b2 = c; }
        else { r = c; g = 0; b2 = x; }
        return System.Windows.Media.Color.FromArgb(
            (byte)(Math.Clamp(a, 0, 1) * 255),
            (byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b2 + m) * 255));
    }
}
