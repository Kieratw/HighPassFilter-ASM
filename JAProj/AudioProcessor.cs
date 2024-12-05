using System;
using System.IO;
using NAudio.Wave;
using System.Runtime.CompilerServices;

public static class AudioProcessor
{
    /// <summary>
    /// Ładuje dane audio z pliku do tablicy `AlignedMemoryFloat` odpowiedniej do przetwarzania SIMD.
    /// </summary>
    public static unsafe (AlignedMemoryFloat[] samples, int sampleRate, int channels, int totalSamples) LoadAudioToFloat(string filePath)
    {
        using (var audioFileReader = new AudioFileReader(filePath))
        {
            int sampleRate = audioFileReader.WaveFormat.SampleRate;
            int channels = audioFileReader.WaveFormat.Channels;
            long totalSamples = audioFileReader.Length / (audioFileReader.WaveFormat.BitsPerSample / 8) / channels;

            // Inicjalizacja tablic dla każdego kanału z odpowiednim wyrównaniem
            AlignedMemoryFloat[] samples = new AlignedMemoryFloat[channels];
            for (int ch = 0; ch < channels; ch++)
            {
                samples[ch] = new AlignedMemoryFloat((int)totalSamples);
            }

            float[] buffer = new float[1024 * channels];
            int samplesRead;
            long sampleIndex = 0;

            while ((samplesRead = audioFileReader.Read(buffer, 0, buffer.Length)) > 0)
            {
                int samplesPerChannel = samplesRead / channels;
                for (int n = 0; n < samplesPerChannel; n++)
                {
                    for (int ch = 0; ch < channels; ch++)
                    {
                        samples[ch].AlignedPointer[sampleIndex + n] = buffer[n * channels + ch];
                    }
                }
                sampleIndex += samplesPerChannel;
            }

            return (samples, sampleRate, channels, (int)totalSamples);
        }
    }

    /// <summary>
    /// Zapisuje przetworzone próbki do pliku audio.
    /// </summary>
    public static unsafe void SaveFloatArrayToAudio(AlignedMemoryFloat[] samples, string outputFilePath, int sampleRate, int channels)
    {
        int totalSamples = samples[0].Length;
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);

        using (var waveFileWriter = new WaveFileWriter(outputFilePath, waveFormat))
        {
            for (int n = 0; n < totalSamples; n++)
            {
                for (int ch = 0; ch < channels; ch++)
                {
                    waveFileWriter.WriteSample(samples[ch].AlignedPointer[n]);
                }
            }
        }
    }
}