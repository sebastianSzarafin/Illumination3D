﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using static WpfApp1.ObjParser;
using System.ComponentModel.Design;
using static WpfApp1.MainWindow;
using System.Windows.Documents;
using System.IO.Packaging;
using System.Windows.Shapes;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    internal class Drawer
    {
        public ObjParser? objParser;
        public double kd, ks;
        public int m;
        public static int bitmapWidth = 600, bitmapHeight = 600;
        public static int objWidth = 500, objHeight = 500;
        public static int offsetX = (bitmapWidth - objWidth) / 2, offsetY = (bitmapHeight - objHeight) / 2;
        public WriteableBitmap bitmap;
        public byte[,,] pixels;
        public byte[] pixels1d;
        public Int32Rect rect;
        public static int stride = 4 * bitmapWidth;
        public Image bitmapImage;
        public DrawOption drawOption;
        public Color defaultVertexColor;
        public bool isExtImageSet = false;

        public Drawer(double _kd, double _ks, int _m, DrawOption _drawOption, Color _defaultVertexColor)
        {
            kd = _kd;
            ks = _ks;
            m = _m;
            bitmap = new WriteableBitmap(bitmapWidth, bitmapHeight, 96, 96, PixelFormats.Bgra32, null);
            pixels = new byte[bitmapHeight, bitmapWidth, 4];
            pixels1d = new byte[bitmapHeight * bitmapWidth * 4];
            rect = new Int32Rect(0, 0, bitmapWidth, bitmapHeight);
            bitmapImage = new Image();
            bitmapImage.Stretch = Stretch.None;
            bitmapImage.Margin = new Thickness(0);
            bitmapImage.Source = bitmap;
            drawOption = _drawOption;
            defaultVertexColor = _defaultVertexColor;
        }

        public static void Redraw(Drawer drawer, Sun sun)
        {
            if (drawer.objParser != null)
                ObjFunctions.DrawObj(drawer, sun);
        }
        public void Initialize(string fileName, Projection projection, Color defaultVertexColor)
        {
            for (int row = 0; row < bitmapHeight; row++)
            {
                for (int col = 0; col < bitmapWidth; col++)
                {
                    for (int i = 0; i <= 3; i++)
                        pixels[row, col, i] = 0;
                   pixels[row, col, 3] = 255;
                }
            }
            objParser = new ObjParser();
            objParser.LoadObj(fileName, projection, defaultVertexColor);
        }
        public void ProcessImage(BitmapImage bitmapImage)
        {
            int bitmapImageStride = bitmapImage.PixelWidth * 4;
            int bitmapImageSize = bitmapImage.PixelHeight * bitmapImageStride;
            byte[] bitmapImagePixels = new byte[bitmapImageSize];
            bitmapImage.CopyPixels(bitmapImagePixels, bitmapImageStride, 0);

            for (int row = 0; row < bitmapImage.PixelHeight; row++)
            {
                for (int col = 0; col < bitmapImage.PixelWidth; col++)
                {
                    int index = row * bitmapImageStride + 4 * col;
                    pixels[row + offsetY, col + offsetX, 0] = bitmapImagePixels[index];
                    pixels[row + offsetY, col + offsetX, 1] = bitmapImagePixels[index + 1];
                    pixels[row + offsetY, col + offsetX, 2] = bitmapImagePixels[index + 2];
                    pixels[row + offsetY, col + offsetX, 3] = bitmapImagePixels[index + 3];
                }
            }

            if(objParser != null)
            {
                foreach (Vertex3D v in objParser.vertices)
                {
                    v.baseColor.R = pixels[(int)v.y, (int)v.x, 2];
                    v.baseColor.G = pixels[(int)v.y, (int)v.x, 1];
                    v.baseColor.B = pixels[(int)v.y, (int)v.x, 0];
                }
            }
            
            isExtImageSet = true;
        }
    }
}
