using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using static System.Net.Mime.MediaTypeNames;
//using static WpfApp1.ObjParser;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public class PositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 4 * (double)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public partial class MainWindow : Window
    {
        //ObjParser? objParser;
        static Drawer drawer = new Drawer(0.5, 0.5, 50);
        static Sun sun = new Sun(Color.FromRgb(255, 255, 255), Drawer.objWidth / 2 + Drawer.offsetX, Drawer.objHeight / 2 + Drawer.offsetY, 1000, drawer);
        public Projection projection = Projection.XY;
        public enum Projection { XY, XZ };
        //double kd = 0.5, ks = 0.5;
        //int m = 50;
        //Color sunColor = Color.FromRgb(255, 255, 255);
        //double x_bigSun = objWidth / 2 + offsetX, y_bigSun = objHeight / 2 + offsetY, z_bigSun = 1000
        //public static int bitmapWidth = 600, bitmapHeight = 600;
        //static int objWidth = 500, objHeight = 500;
        //static int offsetX = (bitmapWidth - objWidth) / 2, offsetY = (bitmapHeight - objHeight) / 2;
        /*WriteableBitmap bitmap = new WriteableBitmap(bitmapWidth, bitmapHeight, 96, 96, PixelFormats.Bgra32, null);
        byte[,,] pixels = new byte[bitmapHeight, bitmapWidth, 4];
        byte[] pixels1d = new byte[bitmapHeight * bitmapWidth * 4];
        Int32Rect rect = new Int32Rect(0, 0, bitmapWidth, bitmapHeight);
        int stride = 4 * bitmapWidth;
        Image bitmapImage = new Image();*/
        //static DispatcherTimer sunTimer = new DispatcherTimer();
        //List<(int x, int y)> sunTrajectory;
        //int sunPos = 0, direction = 1;

        public MainWindow()
        {
            InitializeComponent();            

            /*bitmapImage.Stretch = Stretch.None;
            bitmapImage.Margin = new Thickness(0);
            bitmapImage.Source = bitmap;*/
            canvas.Children.Add(drawer.bitmapImage);
            //sunTimer.Interval = TimeSpan.FromMilliseconds(10);
            //sunTimer.Tick += SunTimerEvent;
            //sunTrajectory = GetTrajectory(0.05, 15, 10, objWidth / 2 + offsetX, objHeight / 2 + offsetY);
        }
        
        void LoadFileEvent(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".obj";
            dialog.Filter = "Object files (.obj)|*.obj"; 

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                drawer.Initialize(dialog.FileName, projection);
                Drawer.Redraw(drawer, sun);
            }
        }
        void XY_AxisProjEvent(object sender, RoutedEventArgs e) => projection = Projection.XY;
        void XZ_AxisProjEvent(object sender, RoutedEventArgs e) => projection = Projection.XZ;

        void kdSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            drawer.kd = kdSlider.Value;
            Drawer.Redraw(drawer, sun);
        }
        void ksSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            drawer.ks = ksSlider.Value;
            Drawer.Redraw(drawer, sun);
        }
        void mSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            drawer.m = (int)mSlider.Value;
            Drawer.Redraw(drawer, sun);
        }

        private void zSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sun.z = (int)zSlider.Value;
            //z_bigSun = (int)zSlider.Value;
            Drawer.Redraw(drawer, sun);
        }

        private void SunSimulationEvent(object sender, RoutedEventArgs e)
        {
            if (sunSimulationButton.Content.ToString() == "Start simulation")
            {
                sunSimulationButton.Content = "Stop simulation";
                sun.timer.Start();
            }
            else
            {
                sunSimulationButton.Content = "Start simulation";
                sun.timer.Stop();
            }
        }

        /*public static void RedrawCanvas()
        {       
            if (objParser != null)
                ObjFunctions.DrawObj(objParser, kd, ks, m, sunColor, new Vertex3D(x_bigSun, y_bigSun, z_bigSun), bitmap, pixels, pixels1d, rect, stride);
        }*/

/*        void SunTimerEvent(object sender, EventArgs e)
        {
            x_bigSun = sunTrajectory[sunPos].x;
            y_bigSun = sunTrajectory[sunPos].y;
            if (sunPos == sunTrajectory.Count - 1) direction = -1;
            if (sunPos == 0) direction = 1;
            sunPos += direction;
            RedrawCanvas();
        }
        List<(int x, int y)> GetTrajectory(double scale, double delta, double revolutions, int centreX, int centreY)
        {
            List<(int x, int y)> trajectory = new List<(int x, int y)>();
            double X = centreX;
            double Y = centreY;
            trajectory.Add(((int)X, (int)Y));
            double theta = 0;
            double radius = 0;

            while (X > 0 && X < bitmapHeight && Y > 0 && Y < bitmapWidth && theta <= (revolutions * 360))
            {
                theta += delta;

                radius = (Math.Pow(theta / 180 * Math.PI, Math.E)) * scale;

                X = (radius * Math.Cos(theta / 180 * Math.PI)) + centreX;
                Y = (radius * Math.Sin(theta / 180 * Math.PI)) + centreY;

                trajectory.Add(((int)X, (int)Y));
            }

            return trajectory;
        }*/

    }
}
