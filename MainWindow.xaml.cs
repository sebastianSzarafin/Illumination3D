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
using System.Reflection;

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
        static Color defaultVertexColor = Colors.Coral;
        static Color defaultSunColor = Colors.White;
        static Drawer drawer = new Drawer(0.5, 0.5, 50, drawOption);
        static Sun sun = new Sun(defaultSunColor, Drawer.objWidth / 2 + Drawer.offsetX, Drawer.objHeight / 2 + Drawer.offsetY, 1000, drawer);
        public static Projection projection = Projection.XY;
        public enum Projection { XY, XZ };
        public static DrawOption drawOption = DrawOption.interpolate;
        public enum DrawOption { interpolate, designate };

        public MainWindow()
        {
            InitializeComponent();
            string s = defaultVertexColor.ToString();
            canvas.Children.Add(drawer.bitmapImage);
            objColors.ItemsSource = typeof(Colors).GetProperties();
            objColors.SelectedItem = typeof(Colors).GetProperty("Coral");
            sunColors.ItemsSource = typeof(Colors).GetProperties();
            sunColors.SelectedItem = typeof(Colors).GetProperty("White");
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

        private void ObjColorChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            if (drawer.objParser == null) return;
            Color vertexColor = (Color)(objColors.SelectedItem as PropertyInfo).GetValue(null, null);
            foreach (ObjParser.Vertex3D v in drawer.objParser.vertices)
            {
                v.baseColor = vertexColor;
            }
            Drawer.Redraw(drawer, sun);
        }
        private void SunColorChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            Color sunColor = (Color)(sunColors.SelectedItem as PropertyInfo).GetValue(null, null);
            sun.color = sunColor;
            Drawer.Redraw(drawer, sun);
        }

        private void InterpolateDrawEvent(object sender, RoutedEventArgs e)
        {
            drawOption = DrawOption.interpolate;
            drawer.drawOption = drawOption;
            Drawer.Redraw(drawer, sun);
        }

        private void DesignateDrawEvent(object sender, RoutedEventArgs e)
        {
            drawOption = DrawOption.designate;
            drawer.drawOption = drawOption;
            Drawer.Redraw(drawer, sun);
        }
    }
}
