using System;
using System.Windows;
using System.Windows.Media;

namespace Desktop_Amethyst_Audio.Views.CustomElements.WaveVisualizer;

/// <summary>
/// Self-contained animated sphere control. Drop into any WPF project.
/// Usage: <local:WaveSphere /> — no dependencies, no configuration needed.
/// </summary>
public class WaveSphere : FrameworkElement
{
    // ── config ───────────────────────────────────────────────
    private const int BlobN = 7;
    private const int PartN = 45;
    private const int SphPts = 100;
    private const int RayN = 16;

    // ── state ────────────────────────────────────────────────
    private double _t;
    private DateTime _last = DateTime.Now;
    private readonly Random _rng = new(42);

    private struct Blob { public double X, Y, Vx, Vy, R, Hue, Ph, Sp; }
    private readonly Blob[] _blobs = new Blob[BlobN];

    private struct Pt { public double X, Y, Vx, Vy, Life, Max, Sz, Hue, A; }
    private readonly Pt[] _pts = new Pt[PartN];

    // ── pre-computed sin/cos for sphere ──────────────────────
    private readonly double[] _sphCos = new double[SphPts + 1];
    private readonly double[] _sphSin = new double[SphPts + 1];

    // ── cached static resources ──────────────────────────────
    private static readonly Brush BgBr, OvBr;
    private static readonly RadialGradientBrush VigBr;

    static WaveSphere()
    {
        BgBr = Fr(Color.FromRgb(5, 5, 22));
        OvBr = Fr(Color.FromArgb(40, 0, 0, 0));
        var v = new RadialGradientBrush { GradientStops = {
            new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.22),
            new GradientStop(Color.FromArgb(170, 0, 0, 0), 1) } };
        v.Freeze(); VigBr = v;
    }

    // ── visual tree ──────────────────────────────────────────
    private readonly DrawingVisual _vis = new();
    protected override int VisualChildrenCount => 1;
    protected override Visual GetVisualChild(int i) => _vis;

    public WaveSphere()
    {
        AddVisualChild(_vis);
        AddLogicalChild(_vis);
        PrecomputeTrig();
        InitBlobs();
        for (int i = 0; i < PartN; i++) SpawnPt(ref _pts[i], true);
        CompositionTarget.Rendering += Tick;
    }

    private void PrecomputeTrig()
    {
        for (int i = 0; i <= SphPts; i++)
        {
            double a = (double)i / SphPts * Math.PI * 2;
            _sphCos[i] = Math.Cos(a);
            _sphSin[i] = Math.Sin(a);
        }
    }

    private void InitBlobs()
    {
        double[] h = { 260, 310, 195, 28, 175, 338, 135 };
        double[] r = { 340, 280, 320, 240, 200, 270, 230 };
        for (int i = 0; i < BlobN; i++)
            _blobs[i] = new Blob {
                X = _rng.NextDouble() * 1920, Y = _rng.NextDouble() * 1080,
                Vx = (_rng.NextDouble() - 0.5) * 0.04,
                Vy = (_rng.NextDouble() - 0.5) * 0.04,
                R = r[i], Hue = h[i],
                Ph = _rng.NextDouble() * Math.PI * 2,
                Sp = 0.002 + _rng.NextDouble() * 0.004 };
    }

    private void SpawnPt(ref Pt p, bool age)
    {
        double a = _rng.NextDouble() * Math.PI * 2;
        double d = 65 + _rng.NextDouble() * 75;
        p.X = Math.Cos(a) * d; p.Y = Math.Sin(a) * d;
        double s = 0.10 + _rng.NextDouble() * 0.42;
        p.Vx = Math.Cos(a) * s + (_rng.NextDouble() - 0.5) * 0.22;
        p.Vy = Math.Sin(a) * s + (_rng.NextDouble() - 0.5) * 0.22;
        p.Max = 2800 + _rng.NextDouble() * 4200;
        p.Life = age ? _rng.NextDouble() * p.Max : 0;
        p.Sz = 1.0 + _rng.NextDouble() * 2.8;
        p.Hue = _rng.NextDouble() * 360;
        p.A = 0.22 + _rng.NextDouble() * 0.50;
    }

    // ══════════════════════════════════════════════════════════
    //  FRAME LOOP
    // ══════════════════════════════════════════════════════════
    private void Tick(object? s, EventArgs e)
    {
        var now = DateTime.Now;
        double dt = Math.Min((now - _last).TotalMilliseconds, 50);
        _last = now;
        double w = ActualWidth, h = ActualHeight;
        if (w < 1 || h < 1) return;
        _t += dt;

        double br = 0.5 + 0.5 * Math.Sin(_t * 0.0011);
        double p1 = 0.5 + 0.5 * Math.Sin(_t * 0.0029);
        double p2 = 0.5 + 0.5 * Math.Sin(_t * 0.0043);
        double p3 = 0.5 + 0.5 * Math.Sin(_t * 0.0021 + 1.5);
        double cx = w / 2, cy = h / 2;

        using var dc = _vis.RenderOpen();
        dc.DrawRectangle(BgBr, null, new Rect(0, 0, w, h));
        PaintBlobs(dc, dt, w, h, br);
        dc.DrawRectangle(OvBr, null, new Rect(0, 0, w, h));
        PaintRays(dc, cx, cy, br, p1);
        PaintOrbits(dc, cx, cy, p2);
        PaintParticles(dc, cx, cy, dt);
        PaintSphere(dc, cx, cy, br, p1, p2, p3);
        dc.DrawRectangle(VigBr, null, new Rect(0, 0, w, h));
    }

    // ══════════════════════════════════════════════════════════
    //  BLOBS
    // ══════════════════════════════════════════════════════════
    private void PaintBlobs(DrawingContext dc, double dt, double w, double h, double br)
    {
        for (int i = 0; i < BlobN; i++)
        {
            ref var b = ref _blobs[i];
            b.X += b.Vx * dt; b.Y += b.Vy * dt;
            b.Ph += b.Sp * dt;
            if (b.X < -b.R) b.X = w + b.R; if (b.X > w + b.R) b.X = -b.R;
            if (b.Y < -b.R) b.Y = h + b.R; if (b.Y > h + b.R) b.Y = -b.R;

            double r = b.R + Math.Sin(b.Ph) * b.R * 0.22;
            double hue = (b.Hue + _t * 0.007) % 360;
            double bri = 0.25 + br * 0.16;
            var g = new RadialGradientBrush { GradientStops = {
                new GradientStop(H(hue, .82, .55, bri), 0),
                new GradientStop(H(hue, .82, .50, bri * .20), .55),
                new GradientStop(H(hue, .82, .50, 0), 1) } };
            g.Freeze();
            dc.DrawEllipse(g, null, new Point(b.X, b.Y), r, r);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  RAYS
    // ══════════════════════════════════════════════════════════
    private void PaintRays(DrawingContext dc, double cx, double cy, double br, double p1)
    {
        double r2 = 330 + br * 85;
        for (int i = 0; i < RayN; i++)
        {
            double a = (double)i / RayN * Math.PI * 2 + _t * 0.0002;
            double hue = (210 + i * 22.5 + _t * 0.013) % 360;
            double al = 0.010 + p1 * 0.030 + Math.Sin(_t * 0.002 + i * 0.85) * 0.010;
            double wi = 5 + br * 14 + Math.Sin(_t * 0.003 + i) * 3.5;
            double cos = Math.Cos(a), sin = Math.Sin(a);
            var pen = new Pen(Fr(H(hue, .70, .58, al)), wi) {
                StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
            pen.Freeze();
            dc.DrawLine(pen,
                new Point(cx + cos * 90, cy + sin * 90),
                new Point(cx + cos * r2, cy + sin * r2));
        }
    }

    // ══════════════════════════════════════════════════════════
    //  ORBITS (3 rings with wobble + dots)
    // ══════════════════════════════════════════════════════════
    private void PaintOrbits(DrawingContext dc, double cx, double cy, double p2)
    {
        for (int r = 0; r < 3; r++)
        {
            double rot = _t * (0.00016 + r * 0.00009) * (r % 2 == 0 ? 1 : -1);
            double rR = 160 + r * 50 + p2 * 10;
            double hue = (200 + r * 50 + _t * 0.01) % 360;
            double al = 0.045 + p2 * 0.065;

            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                for (int i = 0; i <= 80; i++)
                {
                    double a = (double)i / 80 * Math.PI * 2 + rot;
                    double w = Math.Sin(a * 5 + _t * 0.0025 + r * 2.1) * (3.5 + p2 * 7);
                    var pt = new Point(cx + Math.Cos(a) * (rR + w), cy + Math.Sin(a) * (rR + w));
                    if (i == 0) ctx.BeginFigure(pt, false, false);
                    else ctx.LineTo(pt, true, false);
                }
            }
            geo.Freeze();
            var pen = new Pen(Fr(H(hue, .65, .58, al)), 0.8 + p2 * 0.45);
            pen.Freeze();
            dc.DrawGeometry(null, pen, geo);

            for (int d = 0; d < 5; d++)
            {
                double da = (double)d / 5 * Math.PI * 2 + rot * 1.4;
                double dw = Math.Sin(da * 5 + _t * 0.0025 + r * 2.1) * (3.5 + p2 * 7);
                double dA = 0.16 + Math.Sin(_t * 0.004 + d + r * 3) * 0.10;
                dc.DrawEllipse(Fr(H(hue + d * 30, .88, .78, dA)), null,
                    new Point(cx + Math.Cos(da) * (rR + dw), cy + Math.Sin(da) * (rR + dw)), 1.6, 1.6);
            }
        }
    }

    // ══════════════════════════════════════════════════════════
    //  PARTICLES
    // ══════════════════════════════════════════════════════════
    private void PaintParticles(DrawingContext dc, double cx, double cy, double dt)
    {
        for (int i = 0; i < PartN; i++)
        {
            ref var p = ref _pts[i];
            p.Life += dt;
            if (p.Life > p.Max) { SpawnPt(ref p, false); continue; }

            p.X += p.Vx * dt * 0.05;
            p.Y += p.Vy * dt * 0.05;

            double lf = p.Life / p.Max;
            double fade = lf < 0.12 ? lf / 0.12 : lf > 0.72 ? (1 - lf) / 0.28 : 1.0;
            double alpha = p.A * fade;
            if (alpha < 0.012) continue;

            double hue = (p.Hue + _t * 0.009) % 360;
            double sz = p.Sz * (0.72 + 0.28 * Math.Sin(_t * 0.0038 + i));

            dc.DrawEllipse(Fr(H(hue, .85, .83, alpha)), null,
                new Point(cx + p.X, cy + p.Y), sz, sz);

            if (sz > 2)
                dc.DrawEllipse(Fr(H(hue, .85, .72, alpha * 0.18)), null,
                    new Point(cx + p.X, cy + p.Y), sz * 2.2, sz * 2.2);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  SPHERE — morphing, color-shifting, 3D-lit
    // ══════════════════════════════════════════════════════════
    private void PaintSphere(DrawingContext dc, double cx, double cy,
        double br, double p1, double p2, double p3)
    {
        double R = 76 + br * 16 + p1 * 9;
        double h1 = (_t * 0.016) % 360;
        double h2 = (h1 + 120) % 360;
        double h3 = (h1 + 240) % 360;

        // ── pulse glow rings ──────────────────────────────────
        for (int k = 4; k >= 1; k--)
        {
            double pr = R + k * (20 + p1 * 14);
            double a = (1 - k / 5.0) * 0.15 * (0.5 + p1 * 0.65);
            var gb = new RadialGradientBrush { GradientStops = {
                new GradientStop(Color.FromArgb(0, 160, 90, 255), 0),
                new GradientStop(H(h1 + k * 30, .76, .56, a), .52),
                new GradientStop(Color.FromArgb(0, 70, 140, 255), 1) } };
            gb.Freeze();
            dc.DrawEllipse(gb, null, new Point(cx, cy), pr, pr);
        }

        // ── halo ──────────────────────────────────────────────
        double hA = 0.15 + br * 0.20;
        var hl = new RadialGradientBrush { GradientStops = {
            new GradientStop(H(h1, .85, .65, hA), 0),
            new GradientStop(H(h2, .80, .48, hA * .28), .45),
            new GradientStop(Color.FromArgb(0, 35, 18, 110), 1) } };
        hl.Freeze();
        dc.DrawEllipse(hl, null, new Point(cx, cy), R * 2.5, R * 2.5);

        // ── morphing shape (pre-computed trig) ────────────────
        var geo = new StreamGeometry();
        using (var ctx = geo.Open())
        {
            for (int i = 0; i <= SphPts; i++)
            {
                double d = Math.Sin(_sphCos[i] * 3 + _t * 0.0013) * 5.5 * (0.5 + br * 0.8)
                         + Math.Sin(_sphCos[i] * 5 - _t * 0.0020) * 3.5 * (0.3 + p2 * 0.5)
                         + Math.Sin(_sphCos[i] * 7 + _t * 0.0032) * 2.2 * p3
                         + Math.Sin(_sphCos[i] * 2 - _t * 0.0009) * 4.5 * br;
                double rr = R + d;
                var pt = new Point(cx + _sphCos[i] * rr, cy + _sphSin[i] * rr);
                if (i == 0) ctx.BeginFigure(pt, true, true);
                else ctx.LineTo(pt, true, true);
            }
        }
        geo.Freeze();

        // ── body gradient (moving origin for 3D) ──────────────
        double ox = 0.30 + Math.Sin(_t * 0.0008) * 0.07;
        double oy = 0.27 + Math.Cos(_t * 0.0011) * 0.06;
        var body = new RadialGradientBrush {
            Center = new Point(.5, .5), GradientOrigin = new Point(ox, oy),
            RadiusX = .5, RadiusY = .5,
            GradientStops = {
                new GradientStop(H(h3, .68, .95, .97), 0),
                new GradientStop(H(h2, .80, .78, .95), .18),
                new GradientStop(H(h1, .90, .56, .93), .50),
                new GradientStop(H((h1 + 180) % 360, .78, .28, .90), 1) } };
        body.Freeze();
        dc.DrawGeometry(body, null, geo);

        // ── specular ──────────────────────────────────────────
        double sx = 0.32 + Math.Sin(_t * 0.0007) * 0.05;
        double sy = 0.27 + Math.Cos(_t * 0.0009) * 0.04;
        var spec = new RadialGradientBrush {
            Center = new Point(sx, sy), GradientOrigin = new Point(sx, sy),
            RadiusX = .38, RadiusY = .38,
            GradientStops = {
                new GradientStop(Color.FromArgb(175, 255, 255, 255), 0),
                new GradientStop(Color.FromArgb(25, 255, 255, 255), .38),
                new GradientStop(Color.FromArgb(0, 255, 255, 255), 1) } };
        spec.Freeze();
        dc.DrawGeometry(spec, null, geo);

        // ── secondary shine ───────────────────────────────────
        double sH = (h1 + 55) % 360;
        var sh = new RadialGradientBrush {
            Center = new Point(.70, .73), GradientOrigin = new Point(.70, .73),
            RadiusX = .28, RadiusY = .28,
            GradientStops = {
                new GradientStop(H(sH, .72, .82, .26), 0),
                new GradientStop(H(sH, .72, .82, 0), 1) } };
        sh.Freeze();
        dc.DrawGeometry(sh, null, geo);

        // ── rim ───────────────────────────────────────────────
        var rim = new RadialGradientBrush { GradientStops = {
            new GradientStop(Colors.Transparent, 0),
            new GradientStop(Colors.Transparent, .72),
            new GradientStop(H((h1 + 90) % 360, .90, .65, .45 + p1 * .18), 1) } };
        rim.Freeze();
        dc.DrawGeometry(rim, null, geo);

        // ── glow outline ──────────────────────────────────────
        double gs = 0.08 + br * 0.15 + p1 * 0.06;
        var gpen = new Pen(Fr(H(h1, .85, .68, gs)), 2 + br * 3.2) {
            StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
        gpen.Freeze();
        dc.DrawGeometry(null, gpen, geo);

        // ── sparkles ──────────────────────────────────────────
        for (int i = 0; i < 8; i++)
        {
            double sa = _t * 0.001 * (1 + i * 0.13) + i * 0.55;
            double sr = R * (0.26 + 0.50 * Math.Abs(Math.Sin(sa * 0.7 + i)));
            double sA = 0.12 + 0.30 * Math.Pow(Math.Abs(Math.Sin(_t * 0.003 + i * 1.1)), 2);
            double sSz = 1.2 + Math.Sin(_t * 0.005 + i * 0.9) * 0.9;
            if (sSz > 0.4)
                dc.DrawEllipse(Fr(H((h1 + i * 45) % 360, .80, .92, sA)), null,
                    new Point(cx + Math.Cos(sa) * sr, cy + Math.Sin(sa) * sr), sSz, sSz);
        }
    }

    // ══════════════════════════════════════════════════════════
    //  HELPERS
    // ══════════════════════════════════════════════════════════
    private static Brush Fr(Color c) { var b = new SolidColorBrush(c); b.Freeze(); return b; }

    private static Color H(double h, double s, double l, double a)
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
