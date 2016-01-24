using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SlimDX.XAudio2;
using SlimDX.Multimedia;

namespace SlimSlyvia
{
    public class SlyviaPlayer
    {
        private XAudio2 device;
        private AudioBuffer buffer;
        private SourceVoice sourceVoice;

        public SlyviaPlayer(string fileName, XAudio2 device)
        {
            this.device = device;
            this.loadSuccess = this.LoadSound(fileName);
        }

        public void Dispose()
        {
            if (this.loadSuccess)
            {
                this.Stop();
                this.sourceVoice.Dispose();
                this.buffer.AudioData.Dispose();
                this.buffer.Dispose();
            }
        }

        private bool LoadSound(string fileName)
        {
            try
            {
                FileInfo info = new FileInfo(fileName);
                WaveStream stream2 = new WaveStream(fileName);
                this.buffer = new AudioBuffer();
                this.buffer.AudioData = stream2;
                this.buffer.AudioBytes = (int)stream2.Length;
                this.buffer.Flags = BufferFlags.EndOfStream;
                this.sourceVoice = new SourceVoice(this.device, stream2.Format, VoiceFlags.Music | VoiceFlags.UseFilter);
                this.Volume = 100;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public void Play()
        {
            if (this.loadSuccess)
            {
                this.Reset();
                this.sourceVoice.Start(PlayFlags.None);
            }
        }

        public void Play(int Volume)
        {
            int volume = this.Volume;
            this.Volume = Volume;
            this.Play();
            this.Volume = volume;
        }

        public void Reset()
        {
            if (this.loadSuccess)
            {
                this.sourceVoice.Stop();
                this.sourceVoice.FlushSourceBuffers();
                this.sourceVoice.SubmitSourceBuffer(this.buffer);
            }
        }

        public void Resume()
        {
            if (this.loadSuccess)
            {
                this.sourceVoice.Start(PlayFlags.None);
            }
        }

        public void SetLoop(int LoopBegin, int LoopEnd)
        {
            this.SetLoop(0, 0, 0xff, LoopBegin, LoopEnd);
        }

        public void SetLoop(int PlayBegin, int PlayLength, int LoopCount, int LoopBegin, int LoopEnd)
        {
            this.buffer.PlayBegin = PlayBegin;
            this.buffer.PlayLength = PlayLength;
            this.buffer.LoopCount = LoopCount;
            this.buffer.LoopBegin = LoopBegin;
            if (LoopEnd > LoopBegin)
            {
                this.buffer.LoopLength = LoopEnd - LoopBegin;
            }
        }

        public void Stop()
        {
            if (this.loadSuccess)
            {
                try
                {
                    this.sourceVoice.Stop();
                }
                catch { }
            }
        }

        // Properties
        public bool loadSuccess { get; private set; }

        public long Position
        {
            get
            {
                return (this.sourceVoice.State.SamplesPlayed / 0x2cL);
            }
        }

        public int Volume
        {
            get
            {
                return (int)(this.sourceVoice.Volume * 100f);
            }
            set
            {
                if ((0 <= value) && (value <= 100))
                {
                    this.sourceVoice.Volume = ((float)value) / 100f;
                }
            }
        }
    }
}
