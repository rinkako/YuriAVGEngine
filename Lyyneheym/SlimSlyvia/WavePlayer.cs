using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlimSlyvia
{
    public class WavPlayer
    {
        private byte AudioIndex = 0;
        private SlimDX.XAudio2.XAudio2 device;
        private string[] fileName = new string[2];
        private SlyviaPlayer[] track = new SlyviaPlayer[2];

        public WavPlayer(SlimDX.XAudio2.XAudio2 device)
        {
            this.Volume = 100;
            this.device = device;
        }

        public void Dispose()
        {
            for (int i = 0; i < this.track.Length; i++)
            {
                if (this.track[this.AudioIndex] != null)
                {
                    this.track[this.AudioIndex].Dispose();
                }
            }
        }

        public void Play()
        {
            this.track[this.AudioIndex].Play();
        }

        public void PlayRepeat()
        {
            this.track[this.AudioIndex].SetLoop(0, 0);
            this.track[this.AudioIndex].Play();
        }

        public void PlayRepeat(int LoopBegin, int LoopEnd)
        {
            this.PlayRepeat(0, 0, 0xff, LoopBegin, LoopEnd);
        }

        public void PlayRepeat(int PlayBegin, int PlayLength, int LoopCount, int LoopBegin, int LoopEnd)
        {
            this.track[this.AudioIndex].Stop();
            this.track[this.AudioIndex].SetLoop(PlayBegin, PlayLength, LoopCount, LoopBegin, LoopEnd);
            this.track[this.AudioIndex].Play();
        }

        public void PreLoad(string FileName)
        {
            this.FileName = FileName;
            this.track[this.AudioIndex].Reset();
        }

        public void Puase()
        {
            this.OnPause = true;
            this.Stop();
        }

        public void Resume()
        {
            this.OnPause = false;
            this.track[this.AudioIndex].Resume();
        }

        public void Stop()
        {
            if (this.track[this.AudioIndex] != null)
            {
                this.track[this.AudioIndex].Stop();
            }
        }

        public long CurrentPosition
        {
            get
            {
                return this.track[this.AudioIndex].Position;
            }
        }

        public string FileName
        {
            get
            {
                return this.fileName[this.AudioIndex];
            }
            set
            {
                if (this.fileName[1 - this.AudioIndex] == value)
                {
                    this.AudioIndex = (byte)(1 - this.AudioIndex);
                }
                else
                {
                    int volume = 100;
                    this.fileName[this.AudioIndex] = value;
                    if (this.track[this.AudioIndex] != null)
                    {
                        volume = this.track[this.AudioIndex].Volume;
                        this.track[this.AudioIndex].Dispose();
                    }
                    this.track[this.AudioIndex] = new SlyviaPlayer(this.fileName[this.AudioIndex], this.device);
                    this.track[this.AudioIndex].Volume = volume;
                }
            }
        }

        public bool OnPause { get; set; }

        public int Volume
        {
            get
            {
                if (this.track[this.AudioIndex] != null)
                {
                    return this.track[this.AudioIndex].Volume;
                }
                return 100;
            }
            set
            {
                if (this.track[this.AudioIndex] != null)
                {
                    this.track[this.AudioIndex].Volume = value;
                }
            }
        }
    }
}
