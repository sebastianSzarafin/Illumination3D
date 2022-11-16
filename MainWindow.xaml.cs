using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        static Color defaultVertexColor = Colors.Coral;
        static Color defaultSunColor = Colors.White;
        static int sleepTime = 0;
        static Drawer drawer = new Drawer(0.5, 0.5, 10, drawOption, defaultVertexColor, sleepTime);
        static Sun sun = new Sun(defaultSunColor, Drawer.objWidth / 2 + Drawer.offsetX, Drawer.objHeight / 2 + Drawer.offsetY, 1000, drawer);
        public static Projection projection = Projection.XY;
        public enum Projection { XY, XZ };
        public static DrawOption drawOption = DrawOption.interpolate;
        public enum DrawOption { interpolate, designate };

        public MainWindow()
        {
            InitializeComponent();

            canvas.Children.Add(drawer.bitmapImage);
            objColors.ItemsSource = typeof(Colors).GetProperties();
            objColors.SelectedItem = typeof(Colors).GetProperty("Coral");
            sunColors.ItemsSource = typeof(Colors).GetProperties();
            sunColors.SelectedItem = typeof(Colors).GetProperty("White");

            // Predefined scene
            drawer.Initialize("Objects\\Sphere.obj", projection, defaultVertexColor);
            Drawer.Redraw(drawer, sun);
            //
        }
        
        void LoadFileEvent(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.DefaultExt = ".obj";
            dialog.Filter = "Object files (.obj)|*.obj"; 

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                drawer.Initialize(dialog.FileName, projection, defaultVertexColor);
                Drawer.Redraw(drawer, sun);
            }
        }
        void LoadImageEvent(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                BitmapImage myImage = new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute));
                drawer.ProcessImage(myImage);
                Drawer.Redraw(drawer, sun);
                useImageOnButton.IsChecked = true;
            }
        }
        void UseImageEvent(object sender, RoutedEventArgs e)
        {
            drawer.isExtImageSet = true;
            drawer.UpdateVerticesColor();
            Drawer.Redraw(drawer, sun);
        }
        void NotUseImageEvent(object sender, RoutedEventArgs e)
        {
            drawer.isExtImageSet = false;
            drawer.UpdateVerticesColor();
            Drawer.Redraw(drawer, sun);
        }        
        void LoadNormalMapEvent(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dialog.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                BitmapImage myImage = new BitmapImage(new Uri(dialog.FileName, UriKind.Absolute));
                drawer.ProcessNormalMap(myImage);
                Drawer.Redraw(drawer, sun);
                useNormalMapOnButton.IsChecked = true;
            }
        }
        void UseNormalMapEvent(object sender, RoutedEventArgs e)
        {
            drawer.isNormalMapSet = true;
            drawer.UpdateVerticesNormalVector();
            Drawer.Redraw(drawer, sun);
        }
        void NotUseNormalMapEvent(object sender, RoutedEventArgs e)
        {
            drawer.isNormalMapSet = false;
            drawer.UpdateVerticesNormalVector();
            Drawer.Redraw(drawer, sun);
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
        void delaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => drawer.sleepTime = (int)delaySlider.Value;
        void zSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sun.z = (int)zSlider.Value;
            Drawer.Redraw(drawer, sun);
        }
        void SunSimulationEvent(object sender, RoutedEventArgs e)
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
        void ObjColorChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            if (drawer.objParser == null) return;
            defaultVertexColor = (Color)(objColors.SelectedItem as PropertyInfo).GetValue(null, null);
            drawer.defaultVertexColor = defaultVertexColor;
            drawer.UpdateVerticesColor();
            Drawer.Redraw(drawer, sun);
        }
        void SunColorChangedEvent(object sender, SelectionChangedEventArgs e)
        {
            defaultSunColor = (Color)(sunColors.SelectedItem as PropertyInfo).GetValue(null, null);
            sun.color = defaultSunColor;
            Drawer.Redraw(drawer, sun);
        }
        void InterpolateDrawEvent(object sender, RoutedEventArgs e)
        {
            drawOption = DrawOption.interpolate;
            drawer.drawOption = drawOption;
            Drawer.Redraw(drawer, sun);
        }
        void DesignateDrawEvent(object sender, RoutedEventArgs e)
        {
            drawOption = DrawOption.designate;
            drawer.drawOption = drawOption;
            Drawer.Redraw(drawer, sun);
        }
        void UseMeshEvent(object sender, RoutedEventArgs e)
        {
            foreach(Line line in drawer.mesh)
            {
                canvas.Children.Add(line);
            }
        }
        void NotUseMeshEvent(object sender, RoutedEventArgs e)
        {
            List<UIElement> itemstoremove = new List<UIElement>();
            foreach (UIElement ui in canvas.Children)
            {
                if (ui is Line) itemstoremove.Add(ui);
            }
            foreach (UIElement ui in itemstoremove)
            {
                canvas.Children.Remove(ui);
            }
        }
    }
}
