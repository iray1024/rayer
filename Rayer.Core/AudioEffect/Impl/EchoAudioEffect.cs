using Rayer.Core.AudioEffect.Abstractions;

namespace Rayer.Core.AudioEffect.Impl;

/// <summary>
/// 不支持 Flac 格式的音乐
/// </summary>
internal class EchoAudioEffect(int length = 20000, float factor = 0.5f) : IAudioEffect
{
    private readonly Queue<float> _samples = new(Enumerable.Range(0, length).Select(x => 0f));

    public int EchoLength { get; set; } = length;

    public float EchoFactor { get; set; } = factor;

    public float Apply(float sample)
    {
        _samples.Enqueue(sample);

        return Math.Min(1, Math.Max(-1, sample + EchoFactor * _samples.Dequeue()));
    }
}