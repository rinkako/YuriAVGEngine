using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Threading;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Multimedia;
using SlimDX.XAudio2;

namespace SlimSlyvia
{
    public class GameCore
    {
        public bool initSuccess = false;
        public Form1 mainForm = null;
        public Device DeviceMain = null;
        public XAudio2 audioDevice = null;
        public MasteringVoice MasteringVoice;
        private readonly static GameCore instance = new GameCore();

        public static GameCore GetInstance()
        {
            return instance;
        }

        private GameCore()
        {
            this.mainForm = new Form1();
            this.InitD3D();
            this.audioDevice = new XAudio2();
            this.MasteringVoice = new MasteringVoice(this.audioDevice);
            this.initSuccess = true;
        }

        public Bitmap myLoad(string fname)
        {
            return new Bitmap(fname);
        }

        public void Init()
        {
            this.mainForm.Init();
        }

        public void InitD3D()
        {
            try
            {
                CreateFlags cf;
                Direct3D directd = new Direct3D();
                AdapterInformation defaultAdapter = directd.Adapters.DefaultAdapter;
                if ((directd.GetDeviceCaps(defaultAdapter.Adapter, DeviceType.Hardware).DeviceCaps & DeviceCaps.HWTransformAndLight) == DeviceCaps.HWTransformAndLight)
                {
                    cf = CreateFlags.HardwareVertexProcessing;
                }
                else
                {
                    cf = CreateFlags.SoftwareVertexProcessing;
                    Console.WriteLine("Lyynehem Warn:Software Processing, Low effect");
                }
                PresentParameters parameters = new PresentParameters
                {
                    BackBufferWidth = 640,
                    BackBufferHeight = 480,
                    BackBufferCount = 1,
                    BackBufferFormat = defaultAdapter.CurrentDisplayMode.Format,
                    DeviceWindowHandle = this.mainForm.Handle,
                    Windowed = true,
                    PresentFlags = PresentFlags.DiscardDepthStencil,
                    SwapEffect = SwapEffect.Discard,
                    PresentationInterval = PresentInterval.Default
                };
                this.DeviceMain = new Device(directd, defaultAdapter.Adapter, DeviceType.Hardware, this.mainForm.Handle, cf, new PresentParameters[] { parameters });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        public Dictionary<string, TextureObject> TextureObjectDictionay = new Dictionary<string, TextureObject>();

        public void LoadResourceTexture(Bitmap bmp, string Name, Rectangle cliper)
        {
            Texture texture;
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            this.SetTexture(stream, 0, rect, out texture);
            TextureObject obj2 = new TextureObject
            {
                TXTure = texture,
                PosRect = cliper,
                SrcWidth = rect.Width,
                SrcHeight = rect.Height,
                Name = Name
            };
            this.TextureObjectDictionay.Add(Name, obj2);
            bmp.Dispose();
        }


        public void LoadResourceTexture(Bitmap bmp, string Name)
        {
            Texture texture;
            MemoryStream stream = new MemoryStream();
            bmp.Save(stream, ImageFormat.Png);
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            this.SetTexture(stream, 0, rect, out texture);
            TextureObject obj2 = new TextureObject
            {
                TXTure = texture,
                PosRect = new Rectangle(0, 0, rect.Width, rect.Height),
                SrcWidth = rect.Width,
                SrcHeight = rect.Height,
                Name = Name
            };
            this.TextureObjectDictionay.Add(Name, obj2);
            bmp.Dispose();
        }

        public bool SetTexture(Stream imageStream, int colorKey, Rectangle Rect, out Texture texture)
        {
            try
            {
                imageStream.Position = 0L;
                texture = Texture.FromStream(this.DeviceMain, imageStream, Rect.Width, Rect.Height, 0, Usage.None, Format.Unknown, Pool.Managed, Filter.Linear, Filter.None, colorKey);
                return true;
            }
            catch
            {
                texture = null;
                return false;
            }

        }

        public bool SetTexture(string imageFileName, int colorKey, Rectangle Rect, out Texture texture)
        {
            try
            {
                texture = Texture.FromFile(this.DeviceMain, imageFileName, Rect.Width, Rect.Height, 0, Usage.None, Format.Unknown, Pool.Managed, Filter.Linear, Filter.None, colorKey);
                return true;
            }
            catch
            {
                texture = null;
                return false;
            }
        }

        public void Processor()
        {
            if (!this.mainForm.activeMutex)
            {
                return;
            }

            this.mainForm.UpdateData();
            this.DeviceMain.Clear(ClearFlags.ZBuffer | ClearFlags.Target, Color.Black, 1f, 0);
            this.DeviceMain.BeginScene();
            this.mainForm.Render();
            this.DeviceMain.EndScene();
            this.DeviceMain.Present(Present.None);


        }
    }
}
