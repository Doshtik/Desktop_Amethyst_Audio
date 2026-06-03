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
    private const int BlobCount = 7;
    private const int ParticleCount = 50;

    public AudioService? Audio { get; set; }
    public VisMode Mode { get; set; } = VisMode.Ambient;

    // timing
    private double _t;
    private DateTime _last = DateTime.Now;
    private readonly Random _rng = new(42);

    // spectrum
    private readonly float[] _raw = new float[Bars];
    private readonly double[] _val = new double[Bars];
    private readonly double[] _peak = new double[Bars];
    private double _bass, _mid, _high, _energy;

    // blobs
    private struct Blob { public double X, Y, Vx, Vy, R, Hue, Phase, Speed; }
    private readonly Blob[] _blobs = new Blob[BlobCount];

    // particles
    private struct Ptc { public double X, Y, Vx, Vy, Life, MaxL, Sz, Hue, A; }
    private readonly Ptc[] _pts = new Ptc[ParticleCount];

    //  cached static brushes 
    private static readonly Brush BgBr = Fb(Color.FromRgb(5, 5, 22));
    private static readonly Brush OvBr = Fb(Color.FromArgb(45, 0, 0, 0));

    // visual tree
    private readonly DrawingVisual _vis = new();
    protected override int VisualChildrenCount => 1;
    protected override Visual GetVisualChild(int i) => _vis;

    public WaveRenderer()
    {
        AddVisualChild(_vis);
        AddLogicalChild(_vis);
        InitBlobs();
        for (int i = 0; i < ParticleCount; i++) Respawn(ref _pts[i], true);
        CompositionTarget.Rendering += OnRender;
    }

    private void InitBlobs()
    {
        double[] h = { 260, 310, 200, 30, 180, 340, 140 };
        double[] r = { 350, 290, 330, 250, 210, 280, 240 };
        for (int i = 0; i < BlobCount; i++)
            _blobs[i] = new Blob {
                X = _rng.NextDouble() * 1920, Y = _rng.NextDouble() * 1080,
                Vx = (_rng.NextDouble() - 0.5) * 0.04,
                Vy = (_rng.NextDouble() - 0.5) * 0.04,
                R = r[i], Hue = h[i],
                Phase = _rng.NextDouble() * Math.PI * 2,
                Speed = 0.002 + _rng.NextDouble() * 0.004,
            };
    }

    private void Respawn(ref Ptc p, bool rndAge)
    {
        double a = _rng.NextDouble() * Math.PI * 2;
        double d = 70 + _rng.NextDouble() * 70;
        p.X = Math.Cos(a) * d; p.Y = Math.Sin(a) * d;
        double sp = 0.12 + _rng.NextDouble() * 0.45;
        p.Vx = Math.Cos(a) * sp + (_rng.NextDouble() - 0.5) * 0.25;
        p.Vy = Math.Sin(a) * sp + (_rng.NextDouble() - 0.5) * 0.25;
        p.MaxL = 2500 + _rng.NextDouble() * 4000;
        p.Life = rndAge ? _rng.NextDouble() * p.MaxL : 0;
        p.Sz = 1.0 + _rng.NextDouble() * 3.0;
        p.Hue = _rng.NextDouble() * 360;
        p.A = 0.25 + _rng.NextDouble() * 0.55;
    }

    // 
    //                     FRAME
    // 
    private void OnRender(object? sender, EventArgs e)
    {
        var now = DateTime.Now;
        double dt = Math.Min((now - _last).TotalMilliseconds, 50);
        _last = now;
        double w = ActualWidth, h = ActualHeight;
        if (w < 1 || h < 1) return;
        _t += dt;

        //  spectrum 
        bool gotFFT = false;
        if (Mode == VisMode.Equalizer && Audio?.State == PlaybackState.Playing)
            gotFFT = Audio.GetSpectrum(_raw, Bars);

        double rise = 1 - Math.Pow(0.015, dt / 16.0);
        double fall = 1 - Math.Pow(0.12, dt / 16.0);
        for (int i = 0; i < Bars; i++)
        {
            double tgt = gotFFT ? _raw[i] : 0.04 + 0.025 * Math.Sin(_t * 0.001 + i * 0.13) * (1 - (double)i / Bars * 0.4);
            double k = tgt > _val[i] ? rise : fall;
            _val[i] += (tgt - _val[i]) * k;
            if (_val[i] > _peak[i]) _peak[i] = _val[i];
            else _peak[i] *= Math.Pow(0.992, dt / 16.0);
        }
        _bass = Avg(0, 6); _mid = Avg(6, 30); _high = Avg(30, Bars);
        double tE = _bass * 0.5 + _mid * 0.3 + _high * 0.2;
        _energy += (tE - _energy) * (1 - Math.Pow(0.06, dt / 16.0));

        double breathe = 0.5 + 0.5 * Math.Sin(_t * 0.0011);
        double p1 = 0.5 + 0.5 * Math.Sin(_t * 0.0029);
        double p2 = 0.5 + 0.5 * Math.Sin(_t * 0.0043);
        double p3 = 0.5 + 0.5 * Math.Sin(_t * 0.0021 + 1.5);

        // in EQ mode, sphere reacts to music
        double react = (Mode == VisMode.Equalizer && gotFFT) ? 1.0 : 0.0;
        double sBreath = breathe * (1 - react * 0.6) + _energy * react * 2.5;
        double sP1 = p1 * (1 - react * 0.5) + _bass * react * 3.5;

        double cx = w / 2, cy = h / 2;

        using var dc = _vis.RenderOpen();
        dc.DrawRectangle(BgBr, null, new Rect(0, 0, w, h));
        DrawBlobs(dc, dt, w, h, sBreath);
        dc.DrawRectangle(OvBr, null, new Rect(0, 0, w, h));
        DrawRays(dc, cx, cy, sBreath, sP1);
        DrawOrbits(dc, cx, cy, p2, react);
        DrawParticles(dc, cx, cy, dt, react);

        if (Mode == VisMode.Equalizer)
            DrawEqBars(dc, cx, cy, sBreath);

        DrawSphere(dc, cx, cy, sBreath, sP1, p2, p3, react);
        DrawVignette(dc, w, h);
    }

    private double Avg(int lo, int hi) { double s = 0; for (int i = lo; i < hi; i++) s += _val[i]; return s / (hi - lo); }

    // 
    //  BLOBS
    // 
    private void DrawBlobs(DrawingContext dc, double dt, double w, double h, double br)
    {
        double speed = 1 + _energy * 1.2;
        for (int i = 0; i < BlobCount; i++)
        {
            ref var b = ref _blobs[i];
            b.X += b.Vx * dt * speed; b.Y += b.Vy * dt * speed;
            b.Phase += b.Speed * dt * (1 + _energy * 0.8);
            if (b.X < -b.R) b.X = w + b.R; if (b.X > w + b.R) b.X = -b.R;
            if (b.Y < -b.R) b.Y = h + b.R; if (b.Y > h + b.R) b.Y = -b.R;

            double r = b.R + Math.Sin(b.Phase) * b.R * 0.22;
            double hue = (b.Hue + _t * 0.007) % 360;
            double bri = 0.26 + br * 0.16;

            var grd = new RadialGradientBrush { GradientStops = {
                new GradientStop(Hsl(hue, 0.82, 0.55, bri), 0),
                new GradientStop(Hsl(hue, 0.82, 0.50, bri * 0.22), 0.55),
                new GradientStop(Hsl(hue, 0.82, 0.50, 0), 1),
            }}; grd.Freeze();
            dc.DrawEllipse(grd, null, new Point(b.X, b.Y), r, r);
        }
    }

    // 
    //  RAYS
    // 
    private void DrawRays(DrawingContext dc, double cx, double cy, double br, double p1)
    {
        const int N = 20;
        double r2 = 340 + br * 90;
        for (int i = 0; i < N; i++)
        {
            double a = (double)i / N * Math.PI * 2 + _t * 0.00025;
            double hue = (210 + i * 18 + _t * 0.014) % 360;
            double alpha = 0.012 + p1 * 0.035 + Math.Sin(_t * 0.002 + i * 0.9) * 0.012;
            double wid = 6 + br * 16 + Math.Sin(_t * 0.003 + i) * 4;
            var pen = new Pen(Fb(Hsl(hue, 0.72, 0.60, alpha)), wid) {
                StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            pen.Freeze();
            dc.DrawLine(pen,
                new Point(cx + Math.Cos(a) * 95, cy + Math.Sin(a) * 95),
                new Point(cx + Math.Cos(a) * r2, cy + Math.Sin(a) * r2));
        }
    }

    // 
    //  ORBITS
    // 
    private void DrawOrbits(DrawingContext dc, double cx, double cy, double p2, double react)
    {
        for (int r = 0; r < 3; r++)
        {
            double rot = _t * (0.00018 + r * 0.0001) * (r % 2 == 0 ? 1 : -1);
            double rR = 165 + r * 52 + p2 * 10 + _energy * react * 25;
            double hue = (200 + r * 50 + _t * 0.01) % 360;
            double al = 0.05 + p2 * 0.07;

            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                int pts = 100;
                for (int i = 0; i <= pts; i++)
                {
                    double a = (double)i / pts * Math.PI * 2 + rot;
                    double wob = Math.Sin(a * 6 + _t * 0.003 + r * 2) * (4 + p2 * 8);
                    var pt = new Point(cx + Math.Cos(a) * (rR + wob), cy + Math.Sin(a) * (rR + wob));
                    if (i == 0) ctx.BeginFigure(pt, false, false);
                    else ctx.LineTo(pt, true, false);
                }
            }
            geo.Freeze();
            var pen = new Pen(Fb(Hsl(hue, 0.68, 0.62, al)), 0.9 + p2 * 0.5); pen.Freeze();
            dc.DrawGeometry(null, pen, geo);

            // dots
            for (int d = 0; d < 6; d++)
            {
                double da = (double)d / 6 * Math.PI * 2 + rot * 1.5;
                double dw = Math.Sin(da * 6 + _t * 0.003 + r * 2) * (4 + p2 * 8);
                double dA = 0.18 + Math.Sin(_t * 0.004 + d + r * 3) * 0.12;
                dc.DrawEllipse(Fb(Hsl(hue + d * 25, 0.88, 0.78, dA)), null,
                    new Point(cx + Math.Cos(da) * (rR + dw), cy + Math.Sin(da) * (rR + dw)), 1.8, 1.8);
            }
        }
    }

    // 
    //  PARTICLES
    // 
    private void DrawParticles(DrawingContext dc, double cx, double cy, double dt, double react)
    {
        double boost = 1 + react * _energy * 2;
        for (int i = 0; i < ParticleCount; i++)
        {
            ref var p = ref _pts[i];
            p.Life += dt;
            if (p.Life > p.MaxL) { Respawn(ref p, false); continue; }

            p.X += p.Vx * dt * 0.055 * boost;
            p.Y += p.Vy * dt * 0.055 * boost;

            double lf = p.Life / p.MaxL;
            double fade = lf < 0.12 ? lf / 0.12 : lf > 0.7 ? (1 - lf) / 0.3 : 1.0;
            double alpha = p.A * fade;
            if (alpha < 0.01) continue;

            double hue = (p.Hue + _t * 0.01) % 360;
            double sz = p.Sz * (0.7 + 0.3 * Math.Sin(_t * 0.004 + i));

            dc.DrawEllipse(Fb(Hsl(hue, 0.85, 0.82, alpha)), null,
                new Point(cx + p.X, cy + p.Y), sz, sz);

            if (sz > 2.2)
                dc.DrawEllipse(Fb(Hsl(hue, 0.85, 0.75, alpha * 0.2)), null,
                    new Point(cx + p.X, cy + p.Y), sz * 2.2, sz * 2.2);
        }
    }

    // 
    //  EQ BARS (circular)
    // 
    private void DrawEqBars(DrawingContext dc, double cx, double cy, double br)
    {
        double R = 78 + br * 20 + _energy * 18;
        double inner = R + 22;
        double maxH = 120 + _energy * 100;

        for (int i = 0; i < Bars; i++)
        {
            double ang = (double)i / Bars * Math.PI * 2 - Math.PI / 2;
            double bh = Math.Max(2, _val[i] * maxH);
            double hue = (210.0 + i * 360.0 / Bars * 0.85 + _t * 0.015) % 360;
            double alpha = 0.35 + _val[i] * 0.65;

            double cos = Math.Cos(ang), sin = Math.Sin(ang);
            double barW = Math.Max(1.4, Math.PI * 2 * inner / Bars * 0.42);

            var pen = new Pen(Fb(Hsl(hue, 0.88, 0.65, alpha)), barW) {
                StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            pen.Freeze();
            dc.DrawLine(pen,
                new Point(cx + cos * inner, cy + sin * inner),
                new Point(cx + cos * (inner + bh), cy + sin * (inner + bh)));

            // bright tip
            if (_val[i] > 0.12)
                dc.DrawEllipse(Fb(Hsl(hue, 0.95, 0.88, alpha * 0.6)), null,
                    new Point(cx + cos * (inner + bh), cy + sin * (inner + bh)), 2.2, 2.2);

            // peak dot
            double ph = _peak[i] * maxH;
            if (ph > bh + 4)
                dc.DrawEllipse(Fb(Hsl(hue, 0.95, 0.92, 0.45)), null,
                    new Point(cx + cos * (inner + ph + 4), cy + sin * (inner + ph + 4)), 1.4, 1.4);
        }
    }

    // 
    //  SPHERE
    // 
    private void DrawSphere(DrawingContext dc, double cx, double cy,
        double br, double p1, double p2, double p3, double react)
    {
        double R = 76 + br * 18 + p1 * 10 + _energy * react * 20;
        double hue1 = (_t * 0.016) % 360;
        double hue2 = (hue1 + 120) % 360;
        double hue3 = (hue1 + 240) % 360;

        // pulse rings
        for (int k = 4; k >= 1; k--)
        {
            double pr = R + k * (22 + p1 * 16 + _bass * react * 20);
            double a = (1 - k / 5.0) * 0.16 * (0.5 + p1 * 0.7);
            var gb = new RadialGradientBrush { GradientStops = {
                new GradientStop(Color.FromArgb(0, 180, 100, 255), 0),
                new GradientStop(Hsl(hue1 + k * 30, 0.78, 0.58, a), 0.5),
                new GradientStop(Color.FromArgb(0, 80, 160, 255), 1),
            }}; gb.Freeze();
            dc.DrawEllipse(gb, null, new Point(cx, cy), pr, pr);
        }

        // halo
        double haloA = 0.16 + br * 0.20 + _energy * react * 0.25;
        var halo = new RadialGradientBrush { GradientStops = {
            new GradientStop(Hsl(hue1, 0.85, 0.65, haloA), 0),
            new GradientStop(Hsl(hue2, 0.80, 0.50, haloA * 0.3), 0.45),
            new GradientStop(Color.FromArgb(0, 40, 20, 120), 1),
        }}; halo.Freeze();
        dc.DrawEllipse(halo, null, new Point(cx, cy), R * 2.6, R * 2.6);

        // morphing sphere outline
        int pts = 120;
        var geo = new StreamGeometry();
        using (var ctx = geo.Open())
        {
            for (int i = 0; i <= pts; i++)
            {
                double a = (double)i / pts * Math.PI * 2;
                double d;
                if (react > 0.5)
                {
                    int bi = (int)((double)i / pts * Bars) % Bars;
                    d = _val[bi] * 28 + _bass * 16
                      + Math.Sin(a * 4 + _t * 0.002) * 3 * _energy;
                }
                else
                {
                    d = Math.Sin(a * 3 + _t * 0.0014) * 6 * (0.5 + br * 0.8)
                      + Math.Sin(a * 5 - _t * 0.002) * 4 * (0.3 + p2 * 0.5)
                      + Math.Sin(a * 7 + _t * 0.0033) * 2.5 * p3
                      + Math.Sin(a * 2 - _t * 0.001) * 5 * br;
                }
                double rr = R + d;
                var pt = new Point(cx + Math.Cos(a) * rr, cy + Math.Sin(a) * rr);
                if (i == 0) ctx.BeginFigure(pt, true, true);
                else ctx.LineTo(pt, true, true);
            }
        }
        geo.Freeze();

        // gradient body (shifting origin for 3D feel)
        double ox = 0.30 + Math.Sin(_t * 0.0009) * 0.08;
        double oy = 0.28 + Math.Cos(_t * 0.0012) * 0.06;
        var body = new RadialGradientBrush {
            Center = new Point(0.5, 0.5), GradientOrigin = new Point(ox, oy),
            RadiusX = 0.5, RadiusY = 0.5,
            GradientStops = {
                new GradientStop(Hsl(hue3, 0.68, 0.95, 0.97), 0),
                new GradientStop(Hsl(hue2, 0.80, 0.78, 0.95), 0.18),
                new GradientStop(Hsl(hue1, 0.90, 0.55, 0.93), 0.50),
                new GradientStop(Hsl((hue1 + 180) % 360, 0.78, 0.28, 0.90), 1),
            }
        }; body.Freeze();
        dc.DrawGeometry(body, null, geo);

        // specular
        double sx = 0.32 + Math.Sin(_t * 0.0007) * 0.06;
        double sy = 0.28 + Math.Cos(_t * 0.0009) * 0.05;
        var spec = new RadialGradientBrush {
            Center = new Point(sx, sy), GradientOrigin = new Point(sx, sy),
            RadiusX = 0.38, RadiusY = 0.38,
            GradientStops = {
                new GradientStop(Color.FromArgb(180, 255, 255, 255), 0),
                new GradientStop(Color.FromArgb(28, 255, 255, 255), 0.38),
                new GradientStop(Color.FromArgb(0, 255, 255, 255), 1),
            }
        }; spec.Freeze();
        dc.DrawGeometry(spec, null, geo);

        // secondary shine
        double shH = (hue1 + 60) % 360;
        var shine = new RadialGradientBrush {
            Center = new Point(0.70, 0.72), GradientOrigin = new Point(0.70, 0.72),
            RadiusX = 0.28, RadiusY = 0.28,
            GradientStops = {
                new GradientStop(Hsl(shH, 0.75, 0.82, 0.28), 0),
                new GradientStop(Hsl(shH, 0.75, 0.82, 0), 1),
            }
        }; shine.Freeze();
        dc.DrawGeometry(shine, null, geo);

        // rim
        var rim = new RadialGradientBrush { GradientStops = {
            new GradientStop(Colors.Transparent, 0),
            new GradientStop(Colors.Transparent, 0.70),
            new GradientStop(Hsl((hue1 + 90) % 360, 0.90, 0.65, 0.45 + p1 * 0.2), 1),
        }}; rim.Freeze();
        dc.DrawGeometry(rim, null, geo);

        // glow outline
        double gs = 0.09 + br * 0.16 + p1 * 0.07 + _energy * react * 0.2;
        var gp = new Pen(Fb(Hsl(hue1, 0.85, 0.70, gs)), 2 + br * 3.5 + _bass * react * 5) {
            StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
        gp.Freeze();
        dc.DrawGeometry(null, gp, geo);

        // inner sparkles
        for (int i = 0; i < 10; i++)
        {
            double sa = _t * 0.001 * (1 + i * 0.14) + i * 0.55;
            double sr = R * (0.28 + 0.52 * Math.Abs(Math.Sin(sa * 0.7 + i)));
            double sA = 0.13 + 0.32 * Math.Pow(Math.Abs(Math.Sin(_t * 0.003 + i * 1.1)), 2);
            double sSz = 1.3 + Math.Sin(_t * 0.005 + i * 0.9) * 1.0;
            if (sSz > 0.4)
                dc.DrawEllipse(Fb(Hsl((hue1 + i * 36) % 360, 0.80, 0.92, sA)),
                    null, new Point(cx + Math.Cos(sa) * sr, cy + Math.Sin(sa) * sr), sSz, sSz);
        }
    }

    // 
    //  VIGNETTE
    // 
    private static readonly RadialGradientBrush VigBr;
    static WaveRenderer()
    {
        var v = new RadialGradientBrush { GradientStops = {
            new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.20),
            new GradientStop(Color.FromArgb(175, 0, 0, 0), 1),
        }}; v.Freeze(); VigBr = v;
    }
    private void DrawVignette(DrawingContext dc, double w, double h)
        => dc.DrawRectangle(VigBr, null, new Rect(0, 0, w, h));

    // 
    //  HELPERS
    // 
    private static Brush Fb(Color c) { var b = new SolidColorBrush(c); b.Freeze(); return b; }
    private static Color Hsl(double h, double s, double l, double a)
    {
        h %= 360; if (h < 0) h += 360;
        s = Math.Clamp(s, 0, 1); l = Math.Clamp(l, 0, 1);
        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs(h / 60.0 % 2 - 1));
        double m = l - c / 2;
        double r, g, b2;
        if      (h < 60)  { r = c; g = x; b2 = 0; }
        else if (h < 120) { r = x; g = c; b2 = 0; }
        else if (h < 180) { r = 0; g = c; b2 = x; }
        else if (h < 240) { r = 0; g = x; b2 = c; }
        else if (h < 300) { r = x; g = 0; b2 = c; }
        else              { r = c; g = 0; b2 = x; }
        return Color.FromArgb(
            (byte)(Math.Clamp(a, 0, 1) * 255),
            (byte)((r + m) * 255), (byte)((g + m) * 255), (byte)((b2 + m) * 255));
    }
}
