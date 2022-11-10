using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static WpfApp1.ObjParser;
using System.Windows.Media.Imaging;
using System.Windows;

namespace WpfApp1
{
    static class ObjFunctions
    {
        //TODO 
        public static void DrawObj(ObjParser objParser, double kd, double ks, int m, Color sunColor,
            Vertex3D bigSunPos, WriteableBitmap bitmap, byte[,,] pixels, byte[] pixels1d, Int32Rect rect, int stride)
        {
            var watch = new Stopwatch();
            watch.Start();

            SetVerticesColor(objParser, kd, ks, m, sunColor, bigSunPos, pixels);


            /*watch.Stop();
            Debug.WriteLine(watch.Elapsed);*/

            int n = pixels.GetLength(0);
            List<Edge>[] ET = new List<Edge>[n];
            for (int i = 0; i < n; i++)
            {
                ET[i] = new List<Edge>();
            }
            foreach (Polygon polygon in objParser.polygons)
            {
                ScanLineFill(polygon, kd, ks, m, sunColor, bigSunPos, pixels, ET);
            }

            /*watch.Stop();
            Debug.WriteLine(watch.Elapsed);*/

            /*var watch = new Stopwatch();
            watch.Start();*/

            RedrawBitmap(pixels, rect, stride, pixels1d, bitmap);

            watch.Stop();
            Debug.WriteLine(watch.Elapsed);
        }
        public static void ScanLineFill(Polygon polygon, double kd, double ks, int m, Color sunColor,
            Vertex3D bigSunPos, byte[,,] pixels, List<Edge>[] ET)
        {
            double ymin = double.MaxValue, ymax = double.MinValue;
            foreach (Vertex3D v in polygon.vertices)
            {
                if (v.y < ymin) ymin = v.y;
                if (v.y > ymax) ymax = v.y;
            }

            Vertex3D v1 = polygon.vertices[0], v2 = polygon.vertices[1], v3 = polygon.vertices[2];
            double P = crossProduct2D(v2.x - v1.x, v2.y - v1.y, v3.x - v1.x, v3.y - v1.y);

            CreateET(ET, polygon.edges);
            List<Edge> AET = new List<Edge>();
            int y = (int)ymin;
            //while(AET.Count > 0 || ET.Any(list => list.Count > 0))
            while (y <= (int)ymax)
            {
                while (ET[y].Count > 0)
                {
                    AET.Add(ET[y][0]);
                    ET[y].Remove(ET[y][0]);
                }
                AET.RemoveAll(e => (int)e.yu == y);
                AET.Sort();

                for (int i = 0; i < AET.Count; i += 2)
                {
                    foreach (int x in Enumerable.Range((int)Math.Ceiling(AET[i].x), (int)Math.Ceiling(AET[i + 1].x - AET[i].x)))
                    //foreach (int x in Enumerable.Range((int)Math.Ceiling(AET[i].x), Math.Max((int)(Math.Floor(AET[i + 1].x) - Math.Ceiling(AET[i].x)), 0)))
                    {
                        SetPixel(v1, v2, v3, P, x, y, pixels);
                    }
                }
                y++;
                for (int i = 0; i < AET.Count; i++)
                {
                    AET[i].x += AET[i].w;
                }
            }

            foreach (Edge e in polygon.edges) e.x = e._x;

            double crossProduct2D(double x1, double y1, double x2, double y2) => Math.Abs(x1 * y2 - y1 * x2);
        }
        static void SetPixel(Vertex3D v1, Vertex3D v2, Vertex3D v3, double P, int x, int y, byte[,,] pixels)
        {
            /*Vertex3D v = new Vertex3D(x, y, 0), v1 = polygon.vertices[0], v2 = polygon.vertices[1], v3 = polygon.vertices[2];
            double P = PolygonArea(polygon.vertices);            
            double a = PolygonArea(new List<Vertex3D>() { v, v1, v2 }) / P;
            double b = PolygonArea(new List<Vertex3D>() { v, v1, v3 }) / P;
            double c = PolygonArea(new List<Vertex3D>() { v, v2, v3 }) / P;

            pixels[x, y, 2] = (byte)(c * v1.paintColor.R + b * v2.paintColor.R + a * v3.paintColor.R);
            pixels[x, y, 1] = (byte)(c * v1.paintColor.G + b * v2.paintColor.G + a * v3.paintColor.G);
            pixels[x, y, 0] = (byte)(c * v1.paintColor.B + b * v2.paintColor.B + a * v3.paintColor.B);*/

            Vertex3D v = new Vertex3D(x, y, 0);

            double a = crossProduct2D(v1.x - v.x, v1.y - v.y, v2.x - v.x, v2.y - v.y) / P;
            double b = crossProduct2D(v1.x - v.x, v1.y - v.y, v3.x - v.x, v3.y - v.y) / P;
            double c = 1 - a - b;

            pixels[x, y, 2] = (byte)(c * v1.paintColor.R + b * v2.paintColor.R + a * v3.paintColor.R);
            pixels[x, y, 1] = (byte)(c * v1.paintColor.G + b * v2.paintColor.G + a * v3.paintColor.G);
            pixels[x, y, 0] = (byte)(c * v1.paintColor.B + b * v2.paintColor.B + a * v3.paintColor.B);

            double crossProduct2D(double x1, double y1, double x2, double y2) => Math.Abs(x1 * y2 - y1 * x2);
        }

        static double PolygonArea(List<Vertex3D> vertices)
        {
            double l1 = getLength(vertices[0], vertices[1]);
            double l2 = getLength(vertices[1], vertices[2]);
            double l3 = getLength(vertices[2], vertices[0]);
            double s = (l1 + l2 + l3) / 2;
            return Math.Sqrt(s * (s - l1) * (s - l2) * (s - l3));

            double getLength(Vertex3D v1, Vertex3D v2) => Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y));
        }

        static void RedrawBitmap(byte[,,] pixels, Int32Rect rect, int stride, byte[] pixels1d, WriteableBitmap bitmap)
        {
            Buffer.BlockCopy(pixels, 0, pixels1d, 0, pixels.Length);

            bitmap.WritePixels(rect, pixels1d, stride, 0);
        }
        static List<Edge>[] CreateET(List<Edge>[] buckets, List<Edge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                buckets[(int)edges[i].yl].Add(edges[i]);
            }

            for (int i = 0; i < buckets.Length; i++)
            {
                buckets[i].Sort();
            }

            return buckets;
        }
        public static void SetVerticesColor(ObjParser objParser, double kd, double ks, int m, Color sunColor,
            Vertex3D bigSunPos, byte[,,] pixels)
        {
            Normal3D V = new Normal3D(0, 0, 1);
            foreach (Vertex3D v in objParser.vertices)
            {
                Normal3D L = new Normal3D(bigSunPos.x - v.x, bigSunPos.y - v.y, bigSunPos.z - v.z);
                L.Normalize();
                double cosNL = Math.Max(Normal3D.DotProdcut(v.N, L), 0);
                Normal3D R = 2 * cosNL * v.N - L;
                R.Normalize();
                double cosVR = Math.Max(Normal3D.DotProdcut(V, R), 0);

                double sCR = sunColor.R / 255, sCG = (double)sunColor.G / 255, sCB = (double)sunColor.B / 255;
                double vCR = (double)v.baseColor.R / 255, vCG = (double)v.baseColor.G / 255, vCB = (double)v.baseColor.B / 255;

                v.paintColor.R = (byte)(Math.Min((kd * sCR * vCR * cosNL + ks * sCR * vCR * Math.Pow(cosVR, m)) * 255, 255));
                v.paintColor.G = (byte)(Math.Min((kd * sCG * vCG * cosNL + ks * sCG * vCG * Math.Pow(cosVR, m)) * 255, 255));
                v.paintColor.B = (byte)(Math.Min((kd * sCB * vCB * cosNL + ks * sCB * vCB * Math.Pow(cosVR, m)) * 255, 255));

                /*if (v.x < 0) v.x = 0;
                if (v.x >= pixels.GetLength(0)) v.x = pixels.GetLength(0) - 1;
                if (v.y < 0) v.y = 0;
                if (v.y >= pixels.GetLength(1)) v.y = pixels.GetLength(1) - 1;*/

                pixels[(int)v.x, (int)v.y, 2] = v.paintColor.R;
                pixels[(int)v.x, (int)v.y, 1] = v.paintColor.G;
                pixels[(int)v.x, (int)v.y, 0] = v.paintColor.B;
            }
        }
        public static void SetVertexColor(Vertex3D v, double kd, double ks, int m, Color sunColor,
            Vertex3D bigSunPos, byte[,,] pixels)
        {
            Normal3D V = new Normal3D(0, 0, 1);

            Normal3D L = new Normal3D(bigSunPos.x - v.x, bigSunPos.y - v.y, bigSunPos.z - v.z);
            L.Normalize();
            double cosNL = Math.Max(Normal3D.DotProdcut(v.N, L), 0);
            Normal3D R = 2 * cosNL * v.N - L;
            R.Normalize();
            double cosVR = Math.Max(Normal3D.DotProdcut(V, R), 0);

            double sCR = sunColor.R / 255, sCG = sunColor.G / 255, sCB = sunColor.B / 255;
            double vCR = v.baseColor.R / 255, vCG = v.baseColor.G / 255, vCB = v.baseColor.B / 255;

            v.paintColor.R = (byte)(Math.Min((kd * sCR * vCR * cosNL + ks * sCR * vCR * Math.Pow(cosVR, m)) * 255, 255));
            v.paintColor.G = (byte)(Math.Min((kd * sCG * vCG * cosNL + ks * sCG * vCG * Math.Pow(cosVR, m)) * 255, 255));
            v.paintColor.B = (byte)(Math.Min((kd * sCB * vCB * cosNL + ks * sCB * vCB * Math.Pow(cosVR, m)) * 255, 255));

            /*if (v.x < 0) v.x = 0;
            if (v.x >= pixels.GetLength(0)) v.x = pixels.GetLength(0) - 1;
            if (v.y < 0) v.y = 0;
            if (v.y >= pixels.GetLength(1)) v.y = pixels.GetLength(1) - 1;*/

            pixels[(int)v.x, (int)v.y, 2] = v.paintColor.R;
            pixels[(int)v.x, (int)v.y, 1] = v.paintColor.G;
            pixels[(int)v.x, (int)v.y, 0] = v.paintColor.B;
        }
    }
}
