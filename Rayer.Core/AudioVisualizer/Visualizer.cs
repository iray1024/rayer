using FftSharp;
using FftSharp.Windows;
using Complex = System.Numerics.Complex;

namespace Rayer.Core.AudioVisualizer;

public class Visualizer
{
    private readonly double[] _sampleData;
    private DateTime _lastTime;
    private readonly DynamicArray2D _dynamics;

    public int waveDataSize;

    /// <summary>
    /// 采样数据
    /// </summary>
    public double[] SampleData => _sampleData;

    public Visualizer(int waveDataSize)
    {
        if (!Get2Flag(waveDataSize))
        {
            throw new ArgumentException("长度必须是 2 的 n 次幂");
        }

        //_m = (int)Math.Log2(waveDataSize);
        _lastTime = DateTime.Now;
        _sampleData = new double[waveDataSize];
        _dynamics = new DynamicArray2D(1, 1, 1, 0, waveDataSize / 2);

        this.waveDataSize = waveDataSize;
    }

    /// <summary>
    /// 判断是否是 2 的整数次幂
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    private static bool Get2Flag(int num)
    {
        return num >= 1 && (num & (num - 1)) == 0;
    }

    public void PushSampleData(double[] waveData)
    {
        if (waveData.Length > _sampleData.Length)
        {
            Array.Copy(waveData, waveData.Length - _sampleData.Length, _sampleData, 0, _sampleData.Length);
        }
        else
        {
            Array.Copy(_sampleData, waveData.Length, _sampleData, 0, _sampleData.Length - waveData.Length);
            Array.Copy(waveData, 0, _sampleData, _sampleData.Length - waveData.Length, waveData.Length);
        }
    }

    /// <summary>
    /// 获取频谱数据 (数据已经删去共轭部分)
    /// </summary>
    /// <returns></returns>
    public double[] GetSpectrumData()
    {
        var now = DateTime.Now;
        var deltaTime = (now - _lastTime).TotalSeconds;
        _lastTime = now;

        var len = _sampleData.Length;
        var data = new Complex[len];

        for (var i = 0; i < len; i++)
        {
            data[i] = new Complex(_sampleData[i], 0);
        }

        FFT.Forward(data);

        var halfLen = len / 2;
        var spectrum = new double[halfLen]; // 傅里叶变换结果左右对称, 只需要取一半
        for (var i = 0; i < halfLen; i++)
        {
            spectrum[i] = data[i].Magnitude / len;
        }

        var window = new Bartlett();
        window.Create(halfLen);
        window.ApplyInPlace(spectrum, false);

        return _dynamics.Update(deltaTime, spectrum);
    }

    /// <summary>
    /// 取指定频率内的频谱数据
    /// </summary>
    /// <param name="spectrum">源频谱数据</param>
    /// <param name="sampleRate">采样率</param>
    /// <param name="frequency">目标频率</param>
    /// <returns></returns>
    public static double[] TakeSpectrumOfFrequency(double[] spectrum, double sampleRate, double frequency)
    {
        var frequencyPerSampe = sampleRate / spectrum.Length;

        var lengthInNeed = (int)Math.Min(frequency / frequencyPerSampe, spectrum.Length);
        var result = new double[lengthInNeed];
        Array.Copy(spectrum, 0, result, 0, lengthInNeed);
        return result;
    }

    /// <summary>
    /// 简单的数据模糊
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="radius">模糊半径</param>
    /// <returns>结果</returns>
    public static double[] MakeSmooth(double[] data, int radius)
    {
        double[] GetWeights(int radius)
        {
            double Gaussian(double x) => Math.Pow(Math.E, -4 * x * x); // 憨批高斯函数

            var len = 1 + (radius * 2);                                // 长度
            var end = len - 1;                                         // 最后的索引
            var radiusF = (double)radius;                              // 半径浮点数
            var weights = new double[len];                             // 权重

            for (var i = 0; i <= radius; i++)                          // 先把右边的权重算出来
            {
                weights[radius + i] = Gaussian(i / radiusF);
            }

            for (var i = 0; i < radius; i++)                           // 把右边的权重拷贝到左边
            {
                weights[i] = weights[end - i];
            }

            var total = weights.Sum();
            for (var i = 0; i < len; i++)                              // 使权重合为 0
            {
                weights[i] = weights[i] / total;
            }

            return weights;
        }

        void ApplyWeights(double[] buffer, double[] weights)
        {
            var len = buffer.Length;
            for (var i = 0; i < len; i++)
            {
                buffer[i] = buffer[i] * weights[i];
            }
        }

        var weights = GetWeights(radius);
        var buffer = new double[1 + (radius * 2)];

        var result = new double[data.Length];
        if (data.Length < radius)
        {
            Array.Fill(result, data.Average());
            return result;
        }

        for (var i = 0; i < radius; i++)
        {
            Array.Fill(buffer, data[i], 0, radius + 1);
            for (var j = 0; j < radius; j++)
            {
                buffer[radius + 1 + j] = data[i + j];
            }

            ApplyWeights(buffer, weights);
            result[i] = buffer.Sum();
        }

        for (var i = radius; i < data.Length - radius; i++)
        {
            for (var j = 0; j < radius; j++)
            {
                buffer[j] = data[i - j];
            }

            buffer[radius] = data[i];

            for (var j = 0; j < radius; j++)
            {
                buffer[radius + j + 1] = data[i + j];
            }

            ApplyWeights(buffer, weights);
            result[i] = buffer.Sum();
        }

        for (var i = data.Length - radius; i < data.Length; i++)
        {
            Array.Fill(buffer, data[i], 0, radius + 1);
            for (var j = 0; j < radius; j++)
            {
                buffer[radius + 1 + j] = data[i - j];
            }

            ApplyWeights(buffer, weights);
            result[i] = buffer.Sum();
        }

        return result;
    }
}