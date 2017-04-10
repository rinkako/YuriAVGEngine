using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Yuri.PlatformCore;
using Yuri.PlatformCore.Graphic;

namespace Yuri.PageView
{
    /// <summary>
    /// Stage3D.xaml 的交互逻辑
    /// </summary>
    public partial class Stage3D : Page
    {
        /// <summary>
        /// 导演类
        /// </summary>
        private readonly Director core = Director.GetInstance();

        /// <summary>
        /// 初始化标记位
        /// </summary>
        private bool isInit = false;

        /// <summary>
        /// 构造器
        /// </summary>
        public Stage3D()
        {
            InitializeComponent();

            this.BO_MainGrid.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.BO_MainGrid.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            this.ST3D_Viewport.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.ST3D_Viewport.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;

            //GlobalConfigContext.GAME_IS3D = true;
            //ViewPageManager.RegisterPage(GlobalConfigContext.FirstViewPage, this);

        }

        /// <summary>
        /// 事件：页面加载完毕
        /// </summary>
        private void Stage3D_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.isInit == false)
            {
                SCamera3D.Init();
                this.core.GetMainRender().ViewLoaded();
                NotificationManager.Init();
                this.isInit = true;
            }
        }
        
        /// <summary>
        /// 事件：鼠标按下按钮
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.core.UpdateMouse(e);
        }

        /// <summary>
        /// 事件：鼠标松开按钮
        /// </summary>
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.core.UpdateMouse(e);
        }

        /// <summary>
        /// 事件：鼠标滚轮滑动
        /// </summary>
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.core.UpdateMouseWheel(e.Delta);
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            p3 = this.ST3D_Camera.Position;
            var vm = ViewManager.GetInstance();
            var p = ResourceManager.GetInstance().GetPicture("伊泽塔1.png", ResourceManager.FullImageRect);

            Director.ScrMana.AddBackground3D("bg_school.jpg", -8);
            Director.ScrMana.GetSpriteDescriptor(0, ResourceType.Background).Slot3D = 0;

            Director.ScrMana.AddCharacterStand2D(0, "伊泽塔1.png", CharacterStandType.Mid, 0, 0, 1, SpriteAnchorType.Center,
                ResourceManager.FullImageRect);
            Director.ScrMana.GetSpriteDescriptor(0, ResourceType.Stand).Slot3D = 9;
            Director.ScrMana.AddCharacterStand2D(2, "公主1.png", CharacterStandType.Mid, 0, 0, 1, SpriteAnchorType.Center,
                ResourceManager.FullImageRect);
            Director.ScrMana.GetSpriteDescriptor(2, ResourceType.Stand).Slot3D = 12;

            Director.ScrMana.AddCharacterStand2D(1, "Zoithyt-4-2.png", CharacterStandType.Mid, 0, 0, 1, SpriteAnchorType.Center,
                ResourceManager.FullImageRect);
            Director.ScrMana.GetSpriteDescriptor(1, ResourceType.Stand).Slot3D = 24;

            vm.Draw(0, ResourceType.Background);
            vm.Draw(0, ResourceType.Stand);
            vm.Draw(1, ResourceType.Stand);
            vm.Draw(2, ResourceType.Stand);



            //var p2 = ResourceManager.GetInstance().GetPicture("Zoithyt-4-2.png", ResourceManager.FullImageRect);
            //var mat2 = new DiffuseMaterial();
            //var brb2 = new ImageBrush(p2.SpriteBitmapImage);
            //brb2.AlignmentX = AlignmentX.Center;
            //brb2.AlignmentY = AlignmentY.Center;
            //brb2.TileMode = TileMode.None;
            //mat2.Brush = brb2;
            //F2.Material = mat2;

            //var pl = ResourceManager.GetInstance().GetPicture("ScrPartitionLine.jpg", ResourceManager.FullImageRect);
            //var matl = new DiffuseMaterial();
            //var brbl = new ImageBrush(pl.SpriteBitmapImage);
            //brbl.AlignmentX = AlignmentX.Center;
            //brbl.AlignmentY = AlignmentY.Center;
            //brbl.TileMode = TileMode.None;
            //matl.Brush = brbl;
            //Fline.Material = matl;



            //var b3 = ResourceManager.GetInstance().GetPicture("bg_school.jpg", ResourceManager.FullImageRect);
            //var mat3 = new DiffuseMaterial();
            //var brb3 = new ImageBrush(b3.SpriteBitmapImage);
            //brb3.AlignmentX = AlignmentX.Center;
            //brb3.AlignmentY = AlignmentY.Center;
            //brb3.TileMode = TileMode.None;
            //mat3.Brush = brb3;
            //ST3D_Background_Fore.Material = mat3;

            //var bp = ResourceManager.GetInstance().GetPicture("uuz.jpg", ResourceManager.FullImageRect);
            //var matp = new DiffuseMaterial();
            //var brbp = new ImageBrush(bp.SpriteBitmapImage);
            //brbp.AlignmentX = AlignmentX.Center;
            //brbp.AlignmentY = AlignmentY.Center;
            //brbp.TileMode = TileMode.None;
            //matp.Brush = brbp;
            //ST3D_Frontier_1.Material = matp;

        }

        private Point3D p3;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            p3.Z += 1;
            this.ST3D_Camera.Position = p3;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            p3.Z -= 1;
            this.ST3D_Camera.Position = p3;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            p3.X += 0.2;
            this.ST3D_Camera.Position = p3;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            p3.X -= 0.2;
            this.ST3D_Camera.Position = p3;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            p3.Y += 0.2;
            this.ST3D_Camera.Position = p3;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            p3.Y -= 0.2;
            this.ST3D_Camera.Position = p3;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            //this.F1.Material = null;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            SCamera3D.FocusOn(Convert.ToInt32(tb_row.Text), Convert.ToInt32(tb_col.Text), Convert.ToDouble(tb_scale.Text));
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            SCamera3D.ResetFocus(true);
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            SCamera3D.Translate(Convert.ToInt32(tb_row.Text), Convert.ToInt32(tb_col.Text));
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            try
            {
                //ScriptEngine engine = Python.CreateEngine();
                //ScriptScope scope = engine.CreateScope();
                Student stu = new Student { Name = "Wilber", Age = 28 };
                //scope.SetVariable("stuObj", stu);
                //ScriptSource script = engine.CreateScriptSourceFromFile(@"PrintStuInfo.py");
                //var result = script.Execute(scope);
                Dictionary<string, object> d = new Dictionary<string, object> {{"stuObj", stu}};

                YuririWorld.ExecuteFile(@"PrintStuInfo.py", d);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }
    }

    public class Student
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            var t = string.Format("{0} is {1} years old", this.Name, this.Age);
            MessageBox.Show(t);
            return t;
        }
    }
}
