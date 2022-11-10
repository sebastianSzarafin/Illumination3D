using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
//using static WpfApp1.ObjParser;
//using static WpfApp1.ObjDrawer;

namespace WpfApp1
{
    /*internal class ObjDrawer
    {
        public List<Vertex3D> vertices3D;
        public List<Vertex2D> vertices;
        public List<Normal2D> normals;
        public List<Polygon> polygons;
        public List<Edge> edges;

        public ObjDrawer(ObjParser objParser, Canvas canvas, MainWindow.Projection projection)
        {
            vertices = new List<Vertex2D>();
            vertices3D = new List<Vertex3D>();
            double xC = canvas.ActualWidth / 2;
            double yC = canvas.ActualHeight / 2;
            foreach (Vertex3D v in objParser.vertices)
            {
                switch (projection)
                {
                    case MainWindow.Projection.XY:
                        vertices.Add(new Vertex2D(v.x, v.y));
                        break;
                    case MainWindow.Projection.XZ:
                        vertices.Add(new Vertex2D(v.x, v.z));
                        break;
                    default:
                        break;
                }
                vertices3D.Add(new Vertex3D(v.x * xC + xC, v.y * yC + yC, v.z * yC + yC));
                vertices3D.Last().N = v.N;
            }
            normals = new List<Normal2D>();
            foreach(Normal3D n in objParser.normals)
            {
                switch (projection)
                {
                    case MainWindow.Projection.XY:
                        normals.Add(new Normal2D(n.x, n.y));
                        break;
                    case MainWindow.Projection.XZ:
                        normals.Add(new Normal2D(n.x, n.z));
                        break;
                    default:
                        break;
                }
            }
            polygons = new List<Polygon>();
            foreach (Face f in objParser.faces)
            {
                Polygon polygon = new Polygon();
                polygon.Points = new PointCollection();
                //polygon.Fill = Brushes.Black;

                foreach (int i in f.vertexIndex)
                {
                    double x = (vertices[i].x * xC) + xC;
                    double y = (vertices[i].y * yC) + yC;

                    polygon.Points.Add(new System.Windows.Point(x, y));
                }

                polygons.Add(polygon);
            }
            edges = new List<Edge>();
            foreach(Polygon p in polygons)
            {
                for(int i = 0; i < p.Points.Count; i++)
                {
                    edges.Add(new Edge(p.Points[i], p.Points[(i + 1) % p.Points.Count]));
                }
            }
        }


        internal class Vertex2D
        {
            public double x;
            public double y;
            public Vertex2D(double _x, double _y) { x = _x; y = _y; }
        }
        internal class Normal2D
        {
            public double x;
            public double y;
            public Normal2D(double _x, double _y) { x = _x; y = _y; }
        }
        internal class Edge
        {
            public System.Windows.Point u;
            public System.Windows.Point v;

            public int yu, x;
            public double w;

            public Edge(System.Windows.Point _u, System.Windows.Point _v) 
            { 
                if(_u.Y < _v.Y)
                {
                    u = _u;
                    v = _v;
                }
                else
                {
                    u = _v;
                    v = _u;
                }

                yu = (int)u.Y;
                x = (int)u.Y;
                w = (u.X - v.X) * (u.Y - v.Y);
            }
        }
    }*/

    
}
