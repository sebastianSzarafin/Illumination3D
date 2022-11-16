using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApp1
{
    internal class Sun
    {
        public Color color;
        public double CR { get => (double)color.R / 255; }
        public double CG { get => (double)color.G / 255; }
        public double CB { get => (double)color.B / 255; }
        public int centreX, centreY;
        public double x, y, z;
        public DispatcherTimer timer;
        public List<(int x, int y)> trajectory;
        public int trI = 0, direction = 1;
        public Drawer drawer;

        public Sun(Color _color, double _x, double _y, double _z, Drawer _drawer)
        {
            color = _color;
            centreX = (int)_x;
            centreY = (int)_y;
            x = _x;
            y = _y;
            z = _z;
            drawer = _drawer;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += SunTimerEvent;
            trajectory = GetTrajectory(0.05, 15, 15);
        }

        void SunTimerEvent(object sender, EventArgs e)
        {
            x = trajectory[trI].x;
            y = trajectory[trI].y;
            if (trI == trajectory.Count - 1) 
                direction = -1;
            if (trI == 0) direction = 1;
            trI += direction;

            //Debug.WriteLine($"x: {x}, y: {y}");

            Drawer.Redraw(drawer, this);
        }
        // Trajectory with moving on spiral
        List<(int x, int y)> GetTrajectory(double scale, double delta, double revolutions)
        {
            List<(int x, int y)> trajectory = new List<(int x, int y)>();
            double X = centreX;
            double Y = centreY;
            trajectory.Add(((int)X, (int)Y));
            double theta = 0;

            double radius = 0;
            while (theta <= (revolutions * 360))
            {
                theta += delta;

                radius = (Math.Pow(theta / 180 * Math.PI, Math.E)) * scale;

                X = (radius * Math.Cos(theta / 180 * Math.PI)) + centreX;
                Y = (radius * Math.Sin(theta / 180 * Math.PI)) + centreY;

                trajectory.Add(((int)X, (int)Y));
            }
            trajectory = trajectory.Distinct().ToList();

            return trajectory;
        }

        // Trajectory with moving on only 
        /*List<(int x, int y)> GetTrajectory(double scale, double delta, double revolutions)
        {
            List<(int x, int y)> trajectory = new List<(int x, int y)>();
            double X = centreX;
            double Y = centreY;
            double theta = 0;
            double radius = 500;
            int loops = 10;
            while(theta <= (loops + revolutions) * 360)
            {
                theta += delta;

                X = (radius * Math.Cos(theta / 180 * Math.PI)) + centreX;
                Y = (radius * Math.Sin(theta / 180 * Math.PI)) + centreY;

                trajectory.Add(((int)X, (int)Y));
            }

            x = trajectory[0].x;
            y = trajectory[0].y;

            return trajectory;
        }*/
    }
}
