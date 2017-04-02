using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yuri.PlatformCore;

namespace Yuri.PageView
{
    /// <summary>
    /// Stage3D.xaml 的交互逻辑
    /// </summary>
    public partial class Stage3D : Page
    {
        public Stage3D()
        {
            InitializeComponent();

            this.BO_MainGrid.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.BO_MainGrid.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            this.ST3D_Viewport.Width = GlobalConfigContext.GAME_WINDOW_WIDTH;
            this.ST3D_Viewport.Height = GlobalConfigContext.GAME_WINDOW_HEIGHT;
            // 算区块的中轴
            double scrWidth = 6.66;
            double blockWidth = scrWidth / 20.0;
            double blockOffset = blockWidth / 2.0;
            double[] xArr = new double[33];
            for (int i = 32; i >= 1; i--)
            {
                xArr[i] = 0 - blockOffset - blockWidth * (16 - i);
            }
            xArr[0] = 0;
            // 算各区块的显示区间
            pList = new List<Tuple<Point3D, Point3D, Point3D, Point3D>>();
            for (int i = 0; i <= 32; i++)
            {
                Point3D LeftBottom = new Point3D(xArr[i] - 2.031, -4.252, 0);
                Point3D RightBottom = new Point3D(xArr[i] + 2.031, -4.252, 0);
                Point3D LeftUp = new Point3D(xArr[i] - 2.031, 1.652, 0);
                Point3D RightUp = new Point3D(xArr[i] + 2.031, 1.652, 0);
                pList.Add(new Tuple<Point3D, Point3D, Point3D, Point3D>(LeftBottom, RightBottom, LeftUp, RightUp));
            }

        }

        private List<Tuple<Point3D, Point3D, Point3D, Point3D>> pList;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var p = ResourceManager.GetInstance().GetPicture("伊泽塔1.png", ResourceManager.FullImageRect);
            var mat = new DiffuseMaterial();
            var brb = new ImageBrush(p.SpriteBitmapImage);
            brb.AlignmentX = AlignmentX.Center;
            brb.AlignmentY = AlignmentY.Center;
            brb.TileMode = TileMode.None;
            var side1Plane = new MeshGeometry3D();
            var vp1 = pList[10];
            side1Plane.Positions.Add(vp1.Item1);
            side1Plane.Positions.Add(vp1.Item2);
            side1Plane.Positions.Add(vp1.Item3);
            side1Plane.Positions.Add(vp1.Item4);

            side1Plane.TriangleIndices.Add(0);
            side1Plane.TriangleIndices.Add(1);
            side1Plane.TriangleIndices.Add(2);
            side1Plane.TriangleIndices.Add(1);
            side1Plane.TriangleIndices.Add(3);
            side1Plane.TriangleIndices.Add(2);

            side1Plane.TextureCoordinates.Add(new Point(0, 1));
            side1Plane.TextureCoordinates.Add(new Point(1, 1));
            side1Plane.TextureCoordinates.Add(new Point(0, 0));
            side1Plane.TextureCoordinates.Add(new Point(1, 0));

            mat.Brush = brb;
            //mat.Brush = new SolidColorBrush(Colors.AliceBlue);

            F1.Geometry = side1Plane;
            //F1.Material = mat;
            //p3 = this.camera.Position;

            F1.Material = mat;
            p3 = this.ST3D_Camera.Position;




            var p2 = ResourceManager.GetInstance().GetPicture("Zoithyt-4-2.png", ResourceManager.FullImageRect);
            var mat2 = new DiffuseMaterial();
            var brb2 = new ImageBrush(p2.SpriteBitmapImage);
            brb2.AlignmentX = AlignmentX.Center;
            brb2.AlignmentY = AlignmentY.Center;
            brb2.TileMode = TileMode.None;
            mat2.Brush = brb2;
            F2.Material = mat2;

            var pl = ResourceManager.GetInstance().GetPicture("ScrPartitionLine.jpg", ResourceManager.FullImageRect);
            var matl = new DiffuseMaterial();
            var brbl = new ImageBrush(pl.SpriteBitmapImage);
            brbl.AlignmentX = AlignmentX.Center;
            brbl.AlignmentY = AlignmentY.Center;
            brbl.TileMode = TileMode.None;
            matl.Brush = brbl;
            Fline.Material = matl;



            var b3 = ResourceManager.GetInstance().GetPicture("bg_school.jpg", ResourceManager.FullImageRect);
            var mat3 = new DiffuseMaterial();
            var brb3 = new ImageBrush(b3.SpriteBitmapImage);
            brb3.AlignmentX = AlignmentX.Center;
            brb3.AlignmentY = AlignmentY.Center;
            brb3.TileMode = TileMode.None;
            mat3.Brush = brb3;
            ST3D_Background_Fore.Material = mat3;

            //var bp = ResourceManager.GetInstance().GetPicture("uuz.jpg", ResourceManager.FullImageRect);
            //var matp = new DiffuseMaterial();
            //var brbp = new ImageBrush(bp.SpriteBitmapImage);
            //brbp.AlignmentX = AlignmentX.Center;
            //brbp.AlignmentY = AlignmentY.Center;
            //brbp.TileMode = TileMode.None;
            //matp.Brush = brbp;
            //PP1.Material = matp;

        }

        private Point3D p3;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            p3.Z += 0.5;
            this.ST3D_Camera.Position = p3;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            p3.Z -= 0.5;
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
            (this.F1.Material as DiffuseMaterial).Brush = null;
        }
    }
}
