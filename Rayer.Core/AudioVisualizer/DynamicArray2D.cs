namespace Rayer.Core.AudioVisualizer;

internal class DynamicArray2D
{
    private readonly double[] xps, xds;
    private readonly double[] ys, yds;
    private readonly double _w, _z, _d, k1, k2, k3;

    public DynamicArray2D(double f, double z, double r, double x0, int size)
    {
        _w = 2 * Math.PI * f;
        _z = z;
        _d = _w * Math.Sqrt(Math.Abs((z * z) - 1));
        k1 = z / (Math.PI * f);
        k2 = 1 / (2 * Math.PI * f * (2 * Math.PI * f));
        k3 = r * z / (2 * Math.PI * f);

        xps = new double[size];
        ys = new double[size];

        xds = new double[size];
        yds = new double[size];

        Array.Fill(xps, x0);
        Array.Fill(ys, x0);
    }

    public double[] Update(double deltaTime, double[] xs)
    {
        if (xs.Length != xps.Length)
        {
            throw new ArgumentException(null, nameof(deltaTime));
        }

        for (var i = 0; i < xds.Length; i++)
        {
            xds[i] = (xs[i] - xps[i]) / deltaTime;
        }

        double k1_stable, k2_stable;
        if (_w * deltaTime < _z)
        {
            k1_stable = k1;
            k2_stable = Math.Max(Math.Max(k2, (deltaTime * deltaTime / 2) + (deltaTime * k1 / 2)), deltaTime * k1);
        }
        else
        {
            var t1 = Math.Exp(-_z * _w * deltaTime);
            var alpha = 2 * t1 * (_z <= 1 ? Math.Cos(deltaTime * _d) : Math.Cosh(deltaTime * _d));
            var beta = t1 * t1;
            var t2 = deltaTime / (1 + beta - alpha);
            k1_stable = (1 - beta) * t2;
            k2_stable = deltaTime * t2;
        }

        for (var i = 0; i < ys.Length; i++)
        {
            ys[i] = ys[i] + (deltaTime * yds[i]);
            yds[i] = yds[i] + (deltaTime * (xs[i] + (k3 * xds[i]) - ys[i] - (k1_stable * yds[i])) / k2_stable);
        }

        for (var i = 0; i < xps.Length; i++)
        {
            xps[i] = xs[i];
        }

        return ys;
    }
}