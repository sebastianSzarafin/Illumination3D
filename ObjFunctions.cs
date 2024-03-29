﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static WpfApp1.ObjParser;

namespace WpfApp1
{
    static class ObjFunctions
    {
        public static void DrawObj(Drawer drawer, Sun sun)
        {
            if (drawer.objParser == null) return;

            /*var watch = new Stopwatch();
            watch.Start();*/

            foreach (Vertex3D v in drawer.objParser.vertices)
            {
                SetVertexColor(v, drawer, sun);
            }

            /*watch.Stop();
            Debug.WriteLine(watch.Elapsed);*/

            int n = drawer.pixels.GetLength(0);
            List<Edge>[] ET = new List<Edge>[n];
            for (int i = 0; i < n; i++)
            {
                ET[i] = new List<Edge>();
            }
            foreach (Polygon polygon in drawer.objParser.polygons)
            {
                ScanLineFill(polygon, drawer, sun, ET);
            }

            /*watch.Stop();
            Debug.WriteLine(watch.Elapsed);*/

            /*var watch = new Stopwatch();
            watch.Start();*/

            RedrawBitmap(drawer);

            /*watch.Stop();
            Debug.WriteLine(watch.Elapsed);*/

            Thread.Sleep(drawer.sleepTime);

            
        }
        static void ScanLineFill(Polygon polygon, Drawer drawer, Sun sun, List<Edge>[] ET)
        {
            double ymin = double.MaxValue, ymax = double.MinValue;
            foreach (Vertex3D v in polygon.vertices)
            {
                if (v.y < ymin) ymin = v.y;
                if (v.y > ymax) ymax = v.y;
            }

            Vertex3D v1 = polygon.vertices[0], v2 = polygon.vertices[1], v3 = polygon.vertices[2];

            CreateET(ET, polygon.edges);
            List<Edge> AET = new List<Edge>();
            int y = (int)ymin;
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
                    {
                        switch (drawer.drawOption)
                        {
                            case MainWindow.DrawOption.interpolate:
                                SetPixelByInterpolation(v1, v2, v3, x, y, drawer.pixels);
                                break;
                            case MainWindow.DrawOption.designate:
                                SetPixelExplicitly(v1, v2, v3, x, y, drawer, sun);
                                break;
                        }
                    }
                }
                y++;
                for (int i = 0; i < AET.Count; i++)
                {
                    AET[i].x += AET[i].w;
                }
            }

            foreach (Edge e in polygon.edges) e.x = e._x;
        }
        static void SetPixelByInterpolation(Vertex3D v1, Vertex3D v2, Vertex3D v3, int x, int y, byte[,,] pixels)
        {
            double P1 = CrossProduct2D(v1.x - x, v1.y - y, v2.x - x, v2.y - y);
            double P2 = CrossProduct2D(v1.x - x, v1.y - y, v3.x - x, v3.y - y);
            double P3 = CrossProduct2D(v2.x - x, v2.y - y, v3.x - x, v3.y - y);

            double a = P1 / (P1 + P2 + P3);
            double b = P2 / (P1 + P2 + P3);
            double c = P3 / (P1 + P2 + P3);

            pixels[y, x, 2] = (byte)(c * v1.paintColor.R + b * v2.paintColor.R + a * v3.paintColor.R);
            pixels[y, x, 1] = (byte)(c * v1.paintColor.G + b * v2.paintColor.G + a * v3.paintColor.G);
            pixels[y, x, 0] = (byte)(c * v1.paintColor.B + b * v2.paintColor.B + a * v3.paintColor.B);
        }
        static void SetPixelExplicitly(Vertex3D v1, Vertex3D v2, Vertex3D v3, int x, int y, Drawer drawer, Sun sun)
        {
            double P1 = CrossProduct2D(v1.x - x, v1.y - y, v2.x - x, v2.y - y);
            double P2 = CrossProduct2D(v1.x - x, v1.y - y, v3.x - x, v3.y - y);
            double P3 = CrossProduct2D(v2.x - x, v2.y - y, v3.x - x, v3.y - y);

            double a = P1 / (P1 + P2 + P3);
            double b = P2 / (P1 + P2 + P3);
            double c = P3 / (P1 + P2 + P3);

            double z = c * v1.z + b * v2.z + a * v3.z;
            Normal3D N = c * v1.N + b * v2.N + a * v3.N;

            if(drawer.isNormalMapSet)
            {
                Normal3D Nt = new Normal3D(
                        ((double)drawer.extNormalMapPixels[(int)y, (int)x, 2] - 127) / 128,
                        ((double)drawer.extNormalMapPixels[(int)y, (int)x, 1] - 127) / 128,
                        (double)drawer.extNormalMapPixels[(int)y, (int)x, 0] / 255
                        );
                Normal3D B = N.x == 0 && N.y == 0 && N.z == 1 ? new Normal3D(0, 1, 0) : Nt * new Normal3D(0, 0, 1);
                Normal3D T = B * N;

                Normal3D oldN = new Normal3D(N.x, N.y, N.z);
                N = new Normal3D(
                    T.x * Nt.x + B.x * Nt.y + oldN.x * Nt.z,
                    T.y * Nt.x + B.y * Nt.y + oldN.y * Nt.z,
                    T.z * Nt.x + B.z * Nt.y + oldN.z * Nt.z);
            }

            SetPixelColor(x, y, z, N, drawer, sun);
        }
        static double CrossProduct2D(double x1, double y1, double x2, double y2) => Math.Abs(x1 * y2 - y1 * x2);
        static void RedrawBitmap(Drawer drawer)
        {
            Buffer.BlockCopy(drawer.pixels, 0, drawer.pixels1d, 0, drawer.pixels.Length);

            drawer.bitmap.WritePixels(drawer.rect, drawer.pixels1d, Drawer.stride, 0);
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
        static void SetVertexColor(Vertex3D v, Drawer drawer, Sun sun)
        {
            if (drawer.objParser == null) return;

            Normal3D V = new Normal3D(0, 0, 1);

            Normal3D L = new Normal3D(sun.x - v.x, sun.y - v.y, sun.z - v.z);
            L.Normalize();
            double cosNL = Math.Max(Normal3D.DotProdcut(v.N, L), 0);
            Normal3D R = 2 * cosNL * v.N - L;
            R.Normalize();
            double cosVR = Math.Max(Normal3D.DotProdcut(V, R), 0);

            double cosVRtoM = Math.Pow(cosVR, drawer.m);

            v.paintColor.R = (byte)Math.Min((drawer.kd * sun.CR * v.CR * cosNL + drawer.ks * sun.CR * v.CR * cosVRtoM) * 255, 255);
            v.paintColor.G = (byte)Math.Min((drawer.kd * sun.CG * v.CG * cosNL + drawer.ks * sun.CG * v.CG * cosVRtoM) * 255, 255);
            v.paintColor.B = (byte)Math.Min((drawer.kd * sun.CB * v.CB * cosNL + drawer.ks * sun.CB * v.CB * cosVRtoM) * 255, 255);

            drawer.pixels[(int)v.y, (int)v.x, 2] = v.paintColor.R;
            drawer.pixels[(int)v.y, (int)v.x, 1] = v.paintColor.G;
            drawer.pixels[(int)v.y, (int)v.x, 0] = v.paintColor.B;
        }
        static void SetPixelColor(double x, double y, double z, Normal3D N, Drawer drawer, Sun sun)
        {
            if (drawer.objParser == null) return;

            double cR, cG, cB;
            if(drawer.isExtImageSet)
            {
                cR = (double)drawer.extImagePixels[(int)y, (int)x, 2] / 255;
                cG = (double)drawer.extImagePixels[(int)y, (int)x, 1] / 255;
                cB = (double)drawer.extImagePixels[(int)y, (int)x, 0] / 255;
            }
            else
            {
                cR = (double)drawer.defaultVertexColor.R / 255;
                cG = (double)drawer.defaultVertexColor.G / 255;
                cB = (double)drawer.defaultVertexColor.B / 255;
            }

            Normal3D V = new Normal3D(0, 0, 1);
            
            Normal3D L = new Normal3D(sun.x - x, sun.y - y, sun.z - z);
            L.Normalize();
            double cosNL = Math.Max(Normal3D.DotProdcut(N, L), 0);
            Normal3D R = 2 * cosNL * N - L;
            R.Normalize();
            double cosVR = Math.Max(Normal3D.DotProdcut(V, R), 0);

            double cosVRtoM = Math.Pow(cosVR, drawer.m);

            cR = Math.Min((drawer.kd * sun.CR * cR * cosNL + drawer.ks * sun.CR * cR * cosVRtoM) * 255, 255);
            cG = Math.Min((drawer.kd * sun.CG * cG * cosNL + drawer.ks * sun.CG * cG * cosVRtoM) * 255, 255);
            cB = Math.Min((drawer.kd * sun.CB * cB * cosNL + drawer.ks * sun.CB * cB * cosVRtoM) * 255, 255);

            drawer.pixels[(int)y, (int)x, 2] = (byte)cR;
            drawer.pixels[(int)y, (int)x, 1] = (byte)cG;
            drawer.pixels[(int)y, (int)x, 0] = (byte)cB;
        }
    }
}
