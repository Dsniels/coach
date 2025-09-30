using NAudio.CoreAudioApi;
using NAudio.Wave;

public class SpeakerOutput : IDisposable
{

    private readonly BufferedWaveProvider _waveProvider;
    private readonly WaveOutEvent _waveOutEvent;
    public SpeakerOutput()
    {
        WaveFormat audioFormat = new(rate: 24000, bits: 16, channels: 1);
        _waveProvider = new(audioFormat)
        {
            BufferDuration = TimeSpan.FromMinutes(2)
        };
        _waveOutEvent = new();
        _waveOutEvent.Init(_waveProvider);
        _waveOutEvent.Play();

    }

    public void Queue(BinaryData binaryData)
    {
        byte[] buffer = binaryData.ToArray();
        _waveProvider.AddSamples(buffer, 0, buffer.Length);
    }
    public void Clear()
    {
        _waveProvider.ClearBuffer();

    }
    public void Dispose()
    {
        _waveOutEvent.Dispose();
    }
}