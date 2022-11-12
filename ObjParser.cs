using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
using static WpfApp1.ObjParser;
using System.Windows;
using System.Windows.Ink;
using System.Printing.IndexedProperties;
//using System.Windows.Shapes;
//using System.Windows.Shapes;

namespace WpfApp1
{
    internal class ObjParser
    {
        public List<Vertex3D> vertices;
        public List<Normal3D> normals;
        public List<Face> faces;
        public List<Polygon> polygons;
        public List<Edge> edges;
        public ObjParser()
        {
            vertices = new List<Vertex3D>();
            normals = new List<Normal3D>();
            faces = new List<Face>();
            polygons = new List<Polygon>();
            edges = new List<Edge>();
        }
        public void LoadObj(string fileName, MainWindow.Projection projection)
        {
            string[] reader = File.ReadAllLines(fileName);
            foreach(string line in reader)
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    switch (parts[0])
                    {
                        case "v":
                            Vertex3D v = new Vertex3D(0, 0, 0);
                            switch (projection)
                            {
                                case MainWindow.Projection.XY:
                                    v = new Vertex3D(Convert.ToDouble(parts[1], CultureInfo.InvariantCulture), Convert.ToDouble(parts[2], CultureInfo.InvariantCulture), Convert.ToDouble(parts[3], CultureInfo.InvariantCulture));
                                    break;
                                case MainWindow.Projection.XZ:
                                    v = new Vertex3D(Convert.ToDouble(parts[1], CultureInfo.InvariantCulture), Convert.ToDouble(parts[3], CultureInfo.InvariantCulture), Convert.ToDouble(parts[2], CultureInfo.InvariantCulture));
                                    break;
                            }                            
                            vertices.Add(new Vertex3D(v.x * Drawer.objWidth / 2 + Drawer.objWidth / 2 + Drawer.offsetX, v.y * Drawer.objHeight / 2 + Drawer.objHeight / 2 + Drawer.offsetY, v.z * Drawer.objHeight / 2 + Drawer.objHeight / 2 + Drawer.offsetY));
                            break;
                        case "vn":
                            Normal3D vector = new Normal3D(0, 0, 0);
                            switch (projection)
                            {
                                case MainWindow.Projection.XY:
                                    vector = new Normal3D(Convert.ToDouble(parts[1], CultureInfo.InvariantCulture), Convert.ToDouble(parts[2], CultureInfo.InvariantCulture), Convert.ToDouble(parts[3], CultureInfo.InvariantCulture));
                                    break;
                                case MainWindow.Projection.XZ:
                                    vector = new Normal3D(Convert.ToDouble(parts[1], CultureInfo.InvariantCulture), Convert.ToDouble(parts[3], CultureInfo.InvariantCulture), Convert.ToDouble(parts[2], CultureInfo.InvariantCulture));
                                    break;
                            }
                            normals.Add(vector);
                            break;
                        case "f":
                            int[] vertexIndex = new int[3], normalVectorIndex = new int[3];
                            for(int i = 1; i < parts.Length; i++)
                            {
                                string[] faceParts = parts[i].Split("//");
                                vertexIndex[i - 1] = Convert.ToInt32(faceParts[0], CultureInfo.InvariantCulture) - 1;
                                normalVectorIndex[i - 1] = Convert.ToInt32(faceParts[1], CultureInfo.InvariantCulture) - 1;
                            }

                            faces.Add(new Face(vertexIndex, normalVectorIndex));
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach(Face f in faces)
            {
                List<Vertex3D> _vertices = new List<Vertex3D>();
                List<Edge> _edges = new List<Edge>();
                for (int i = 0; i < f.vertexIndex.Length; i++)
                {
                    vertices[f.vertexIndex[i]].N = normals[f.normalVectorIndex[i]];
                    _vertices.Add(vertices[f.vertexIndex[i]]);
                    _edges.Add(new Edge(vertices[f.vertexIndex[i]], vertices[f.vertexIndex[(i + 1) % f.vertexIndex.Length]]));
                }
                polygons.Add(new Polygon(_vertices, _edges));
            }
        }

        internal class Vertex3D
        {
            public double x;
            public double y;
            public double z;
            public Normal3D N;
            public System.Windows.Media.Color baseColor = Colors.Coral;
            public System.Windows.Media.Color paintColor = Colors.Coral;
            public bool isPixelSet = false;

            public Vertex3D(double _x, double _y, double _z) { x = _x; y = _y; z = _z; N = new Normal3D(0, 0, 0); }
            public static Vertex3D operator -(Vertex3D v1, Vertex3D v2) => new Vertex3D(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }
        internal class Normal3D
        {
            public double x;
            public double y;
            public double z;
            public Normal3D(double _x, double _y, double _z) 
            {
                x = _x; 
                y = _y; 
                z = _z;
                double sum = Math.Abs(_x) + Math.Abs(_y) + Math.Abs(_z);
                if(sum != 0)
                {
                    x /= sum;
                    y /= sum;
                    z /= sum;
                }
            }
            public static double DotProdcut(Normal3D n1, Normal3D n2) => n1.x * n2.x + n1.y * n2.y + n1.z * n2.z;
            public static Normal3D operator *(double a, Normal3D n) => new Normal3D(a * n.x, a * n.y, a * n.z);
            public static Normal3D operator -(Normal3D n1, Normal3D n2) => new Normal3D(n1.x - n2.x, n1.y - n2.y, n1.z - n2.z);
            public static Normal3D operator +(Normal3D n1, Normal3D n2) => new Normal3D(n1.x + n2.x, n1.y + n2.y, n1.z + n2.z);
        }
        /*internal class Vector3D : Normal3D
        {
            public Vector3D(double _x, double _y, double _z) : base(_x, _y, _z) { }
            public Vector3D(Vertex3D v1, Vertex3D v2) : base(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z) { }
        }*/
        internal class Face
        {
            public int[] vertexIndex;
            public int[] normalVectorIndex;
            public Face(int[] _vertexIndex, int[] _normalVectorIndex) { vertexIndex = _vertexIndex; normalVectorIndex = _normalVectorIndex; }
        }

        internal class Edge : IComparable
        {
            public Vertex3D u;
            public Vertex3D v;

            public double yu, yl, x, _x, xl, w;

            public Edge(Vertex3D _u, Vertex3D _v)
            {
                if (_u.y < _v.y)
                {
                    u = _u;
                    v = _v;
                }
                else
                {
                    u = _v;
                    v = _u;
                }

                yu = v.y;
                yl = u.y;
                x = u.x;
                _x = x;
                w = (v.x - u.x) / (v.y - u.y);
            }

            public int CompareTo(object? obj)
            {
                if (obj == null) return 1;
                Edge? e = obj as Edge;

                if (e != null)
                {
                    /*if (yl < e.yl) return -3;
                    if (yl > e.yl) return 3;*/

                    if (x < e.x) return -2;
                    if (x > e.x) return 2;

                    if (w < e.w) return -1;
                    if (w > e.w) return 1;

                    return 0;
                }
                else throw new ArgumentException("Object is not an Edge");
            }
        }

        internal class Polygon
        {
            public List<Vertex3D> vertices;
            public List<Edge> edges;
            public Polygon(List<Vertex3D> _vertices, List<Edge> _edges) { vertices = _vertices; edges = _edges; }
        }
    }
}
