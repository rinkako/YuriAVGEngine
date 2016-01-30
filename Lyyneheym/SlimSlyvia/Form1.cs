using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;
using SlimDX.DirectSound;

namespace SlimSlyvia
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(640, 480);
        }

        SlimDX.DirectInput.DirectInput dv = new DirectInput();
        SlimDX.DirectInput.Keyboard kb;

        public void Init()
        {
            this.SpriteMain = new MySprite(GameCore.GetInstance().DeviceMain);
            this.Sprite2 = new MySprite(GameCore.GetInstance().DeviceMain);
            this.DXFont = new SlimDX.Direct3D9.Font(GameCore.GetInstance().DeviceMain, new System.Drawing.Font("微软雅黑", 12));
            kb = new Keyboard(dv);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            xflag = !xflag;
            if (xflag == true)
            {
                this.SpriteMain.Dispose();
            }
            else
            {
                this.SpriteMain = new MySprite(GameCore.GetInstance().DeviceMain);
                angle = 0;
            }
        }

        PointF pos = new PointF(320.0f, 240.0f);
        float angle = 0.0f;

        long TimeMain = 0;
        long TimeCount = 0;
        float FPS = 0f;
        public void UpdateFPS()
        {
            if ((this.TimeMain % 30) == 0)
            {
                long timestamp = Stopwatch.GetTimestamp();
                this.FPS = (30f * Stopwatch.Frequency) / ((float)(timestamp - this.TimeCount));
                this.TimeCount = timestamp;
            }
            this.TimeMain++;
        }

        public void UpdateData()
        {
            this.UpdateFPS();
            kb.Acquire();
            KeyboardState ks = kb.GetCurrentState();
            if (ks.IsPressed(Key.Escape))
            {
                this.Close();
                return;
            }
            if (ks.IsPressed(Key.A))
            {
                pos.X -= 5f;
                if (pos.X < 0)
                    pos.X = 0;
            }
            if (ks.IsPressed(Key.D))
            {
                pos.X += 5f;
                if (pos.X > GameCore.GetInstance().DeviceMain.Viewport.Width)
                    pos.X = GameCore.GetInstance().DeviceMain.Viewport.Width;

            }
            if (ks.IsPressed(Key.W))
            {
                pos.Y -= 5f;
                if (pos.Y < 0)
                    pos.Y = 0;
            }
            if (ks.IsPressed(Key.S))
            {
                pos.Y += 5f;
                if (pos.Y > GameCore.GetInstance().DeviceMain.Viewport.Height)
                    pos.Y = GameCore.GetInstance().DeviceMain.Viewport.Height;
            }
            angle += 1f;
            if (angle >= 360)
            {
                angle = 0;
            }
            if (aflag)
            {
                acapa += 3;
                if (acapa == 255)
                {
                    aflag = false;
                }
            }
            else
            {
                acapa -= 3;
                if (acapa == 0)
                {
                    aflag = true;
                }
            }

        }
        bool aflag = true;
        int acapa = 0;
        bool xflag = false;
        public void Render()
        {
            if (!this.SpriteMain.Disposed)
            {
                this.SpriteMain.Begin(SpriteFlags.AlphaBlend);
                this.SpriteMain.Draw2D(GameCore.GetInstance().TextureObjectDictionay["flowers"], (float)(1f * acapa / 255.0 * 1.5), (float)(angle / 180.0f * Math.PI), new PointF(640f, 480f), 0xff);
                this.DXFont.DrawString(this.SpriteMain.sprite, this.FPS.ToString("F1") + "fps", 570, 460, Color.White);
                this.SpriteMain.End();
            }
            if (!this.Sprite2.Disposed)
            {
                this.Sprite2.Begin(SpriteFlags.AlphaBlend);
                this.Sprite2.Draw2D(GameCore.GetInstance().TextureObjectDictionay["mytest"], 1f, 0f, pos, Color.FromArgb(this.acapa, Color.White));
                this.Sprite2.End();
            }



        }
        MySprite SpriteMain = null;
        MySprite Sprite2 = null;
        SlimDX.Direct3D9.Font DXFont;
        private void button2_Click(object sender, EventArgs e)
        {
            WavPlayer wp = new WavPlayer(GameCore.GetInstance().audioDevice);
            wp.FileName = "Boss01.wav";
            wp.Volume = 100;
            wp.PlayRepeat(0, 0, 0xff, 0, 0);
        }

        public bool activeMutex { get; private set; }

        private void button3_Click(object sender, EventArgs e)
        {
            WavPlayer wp = new WavPlayer(GameCore.GetInstance().audioDevice);
            wp.FileName = "se01.wav";
            wp.Volume = 100;
            wp.PlayRepeat(0, 0, 0, 0, 0);
        }

        private void Form1_Activated_1(object sender, EventArgs e)
        {
            this.activeMutex = true;
        }

        private void Form1_Deactivate_1(object sender, EventArgs e)
        {
            this.activeMutex = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Paint(object sender, PaintEventArgs e)
        {

            System.Drawing.Drawing2D.GraphicsPath myg = new System.Drawing.Drawing2D.GraphicsPath();

            //myg.AddEllipse(new Rectangle(0, 0, 100, 80)); //加椭圆
            //button1.BackColor = Color.Purple;
            //button1.Size = new System.Drawing.Size(256, 256);
            //button1.Region = new Region(myg);

            FontFamily ff = new FontFamily("Arial");
            string str = "Click Me!";
            int fs = (int)FontStyle.Bold;
            int emsize = 35;
            PointF origin = new PointF(0, 0);
            StringFormat sf = new StringFormat(StringFormat.GenericDefault);
            myg.AddString(str, ff, fs, emsize, origin, sf);
            
            button4.Region = new Region(myg); 
        }

    }
}
