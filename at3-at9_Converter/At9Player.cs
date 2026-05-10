using System;
using System.IO;
using NAudio.Wave;
using VGAudio.Containers.At9;
using VGAudio.Formats;
using VGAudio.Formats.Pcm16;

namespace at3_at9_Converter
{
    public class At9Player : IDisposable
    {
        private WaveOutEvent outputDevice;
        private RawSourceWaveStream waveStream;
        private MemoryStream memoryStream;

        public bool IsPlaying { get; private set; }

        public void Play(string filePath)
        {
            Stop();

            AudioData audioData;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                At9Reader reader = new At9Reader();
                audioData = reader.Read(fs);
            }

            Pcm16Format pcm = audioData.GetFormat<Pcm16Format>();

            int channels = pcm.Channels.Length;
            int sampleRate = pcm.SampleRate;
            int sampleCount = pcm.Channels[0].Length;

            byte[] pcmBytes = new byte[sampleCount * channels * 2];
            int index = 0;

            for (int i = 0; i < sampleCount; i++)
            {
                for (int ch = 0; ch < channels; ch++)
                {
                    short sample = pcm.Channels[ch][i];

                    pcmBytes[index++] = (byte)(sample & 0xFF);
                    pcmBytes[index++] = (byte)((sample >> 8) & 0xFF);
                }
            }

            memoryStream = new MemoryStream(pcmBytes);
            WaveFormat waveFormat = new WaveFormat(sampleRate, 16, channels);
            waveStream = new RawSourceWaveStream(memoryStream, waveFormat);

            outputDevice = new WaveOutEvent();
            outputDevice.Init(waveStream);
            outputDevice.Play();

            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;

            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }

            if (waveStream != null)
            {
                waveStream.Dispose();
                waveStream = null;
            }

            if (memoryStream != null)
            {
                memoryStream.Dispose();
                memoryStream = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}