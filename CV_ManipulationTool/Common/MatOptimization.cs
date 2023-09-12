using CV_ManipulationTool.Model;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CV_ManipulationTool.Common
{
    public class MatOptimization
    {
        public List<Mat> Segmentation(Mat SrcImage, int BlockNums)
        {
            List<Mat> list = new List<Mat>();
            int width = SrcImage.Width;
            int height = SrcImage.Height;
            int num = width / BlockNums;
            int num2 = height / BlockNums;
            for (int i = 0; i < BlockNums; i++)
            {
                for (int j = 0; j < BlockNums; j++)
                {
                    int x = i * num;
                    int y = j * num2;
                    Rect roi = new Rect(x, y, num, num2);
                    Mat item = SrcImage.Clone(roi);
                    list.Add(item);
                }
            }

            return list;
        }

        public List<Bitmap> Segmentation(Bitmap SrcImage, int BlockNums)
        {
            List<Bitmap> list = new List<Bitmap>();
            int width = SrcImage.Width;
            int height = SrcImage.Height;
            int num = width / BlockNums;
            int num2 = height / BlockNums;
            for (int i = 0; i < BlockNums; i++)
            {
                for (int j = 0; j < BlockNums; j++)
                {
                    int x = i * num;
                    int y = j * num2;
                    Rectangle rect = new Rectangle(x, y, num, num2);
                    Bitmap item = SrcImage.Clone(rect, PixelFormat.Format8bppIndexed);
                    list.Add(item);
                }
            }

            return list;
        }

        public Mat Composition(List<Mat> mats, int BlockNums)
        {
            Mat mat = new Mat();
            for (int i = 0; i < BlockNums; i++)
            {
                Mat mat2 = new Mat();
                for (int j = 0; j < BlockNums; j++)
                {
                    if (mat2.Empty())
                    {
                        mat2 = mats[i * BlockNums + j];
                    }
                    else
                    {
                        Cv2.VConcat(mat2, mats[i * BlockNums + j], mat2);
                    }
                }

                if (mat.Empty())
                {
                    mat = mat2;
                }
                else
                {
                    Cv2.HConcat(mat, mat2, mat);
                }
            }

            return mat;
        }

        public Bitmap Composition(List<Bitmap> mats, int BlockNums)
        {
            int width = mats[0].Width;
            int height = mats[0].Height;
            Bitmap bitmap = new Bitmap(width * BlockNums, height * BlockNums);
            for (int i = 0; i < BlockNums; i++)
            {
                for (int j = 0; j < BlockNums; j++)
                {
                    for (int k = 0; k < mats[i * BlockNums + j].Width; k++)
                    {
                        for (int l = 0; l < mats[i * BlockNums + j].Height; l++)
                        {
                            bitmap.SetPixel(k + i * width, l + j * height, mats[i * BlockNums + j].GetPixel(k, l));
                        }
                    }
                }
            }

            return bitmap;
        }

        public Mat GetQRCode(Mat SrcImage, QRCodePosition PositionInfo)
        {
            Rect roi = new Rect(PositionInfo.Origin, PositionInfo.Size);
            return SrcImage.Clone(roi);
        }

        public Bitmap GetQRCode(Bitmap SrcImage, QRCodePositionB PositionInfo)
        {
            Rectangle rect = new Rectangle(PositionInfo.Origin, PositionInfo.Size);
            return SrcImage.Clone(rect, PixelFormat.Format8bppIndexed);
        }

        public Mat CaptureImageByPosition(Mat SrcImage, Rect Roi)
        {
            return SrcImage.Clone(Roi);
        }

        public Bitmap CaptureImageByPosition(Bitmap SrcImage, Rectangle Roi)
        {
            return SrcImage.Clone(Roi, PixelFormat.Format8bppIndexed);
        }

        public Mat CaptureImageByPosition(Mat SrcImage, List<OpenCvSharp.Point> Points)
        {
            Rect roi = new Rect(size: new OpenCvSharp.Size(Points[1].X - Points[0].X, Points[1].Y - Points[0].Y), location: Points[0]);
            return SrcImage.Clone(roi);
        }

        public Bitmap CaptureImageByPosition(Bitmap SrcImage, List<System.Drawing.Point> Points)
        {
            System.Drawing.Size size = new System.Drawing.Size(Points[1].X - Points[0].X, Points[1].Y - Points[0].Y);
            Rectangle rect = new Rectangle(Points[0], size);
            return SrcImage.Clone(rect, PixelFormat.Format8bppIndexed);
        }

        public Mat CaptureImageByPosition(Mat SrcImage, OpenCvSharp.Point Origin, int Width, int Hight)
        {
            OpenCvSharp.Size size = new OpenCvSharp.Size(Width, Hight);
            Rect roi = new Rect(Origin, size);
            return SrcImage.Clone(roi);
        }

        public Bitmap CaptureImageByPosition(Bitmap SrcImage, System.Drawing.Point Origin, int Width, int Hight)
        {
            System.Drawing.Size size = new System.Drawing.Size(Width, Hight);
            Rectangle rect = new Rectangle(Origin, size);
            return SrcImage.Clone(rect, PixelFormat.Format8bppIndexed);
        }

        public Mat HistogramEqualization_TY(Mat SrcImage)
        {
            int[] array = DistributionFunction_TY(SrcImage);
            Mat mat = new Mat(SrcImage.Rows, SrcImage.Cols, MatType.CV_8UC1);
            for (int i = 0; i < SrcImage.Cols; i++)
            {
                for (int j = 0; j < SrcImage.Rows; j++)
                {
                    mat.At<byte>(j, i) = (byte)array[SrcImage.At<byte>(j, i)];
                }
            }

            return mat;
        }

        public Bitmap HistogramEqualization_TY(Bitmap SrcImage)
        {
            int[] array = DistributionFunction_TY(SrcImage);
            Bitmap bitmap = new Bitmap(SrcImage.Height, SrcImage.Width, PixelFormat.Format8bppIndexed);
            for (int i = 0; i < SrcImage.Width; i++)
            {
                for (int j = 0; j < SrcImage.Height; j++)
                {
                    int num = array[SrcImage.GetPixel(i, j).R];
                    bitmap.SetPixel(i, j, Color.FromArgb(num, num, num));
                }
            }

            return bitmap;
        }

        public int[] GetHistogram_TY(Mat SrcImage)
        {
            int[] array = new int[256];
            for (int i = 0; i < SrcImage.Cols; i++)
            {
                for (int j = 0; j < SrcImage.Rows; j++)
                {
                    byte b = SrcImage.At<byte>(j, i);
                    array[b]++;
                }
            }

            return array;
        }

        public int[] GetHistogram_TY(Bitmap SrcImage)
        {
            int[] array = new int[256];
            for (int i = 0; i < SrcImage.Width; i++)
            {
                for (int j = 0; j < SrcImage.Height; j++)
                {
                    byte r = SrcImage.GetPixel(i, j).R;
                    array[r]++;
                }
            }

            return array;
        }

        public int[] DistributionFunction_TY(Mat SrcImage)
        {
            int[] array = new int[256];
            int[] histogram_TY = GetHistogram_TY(SrcImage);
            int num = SrcImage.Cols * SrcImage.Rows;
            int num2 = 0;
            for (int i = 0; i < 256; i++)
            {
                num2 += histogram_TY[i];
                array[i] = (int)(1.0 * (double)num2 / (double)num * 255.0);
            }

            return array;
        }

        public int[] DistributionFunction_TY(Bitmap SrcImage)
        {
            int[] array = new int[256];
            int[] histogram_TY = GetHistogram_TY(SrcImage);
            int num = SrcImage.Width * SrcImage.Height;
            int num2 = 0;
            for (int i = 0; i < 256; i++)
            {
                num2 += histogram_TY[i];
                array[i] = (int)(1.0 * (double)num2 / (double)num * 255.0);
            }

            return array;
        }

        public Mat Otsu_TY(Mat SrcImage, BinaryType Type)
        {
            int num = 0;
            int[] histogram_TY = GetHistogram_TY(SrcImage);
            double[] array = new double[256];
            double[] array2 = new double[256];
            double[] array3 = new double[256];
            double num2 = SrcImage.Cols * SrcImage.Rows;
            double num3 = 0.0;
            double num4 = 0.0;
            for (int i = 0; i < 256; i++)
            {
                array[i] = (double)histogram_TY[i] / num2;
                array2[i] = num3 + array[i];
                num3 = array2[i];
                array3[i] = num4 + (double)i * array[i];
                num4 = array3[i];
            }

            double num5 = 0.0;
            for (int j = 0; j < 256; j++)
            {
                if ((array3[255] * array2[j] - array3[j]) * (array3[255] * array2[j] - array3[j]) / (array2[j] * (1.0 - array2[j])) > num5)
                {
                    num5 = (array3[255] * array2[j] - array3[j]) * (array3[255] * array2[j] - array3[j]) / (array2[j] * (1.0 - array2[j]));
                    num = j;
                }
            }

            Mat mat = SrcImage.Clone();
            for (int k = 0; k < mat.Cols; k++)
            {
                for (int l = 0; l < mat.Rows; l++)
                {
                    if (Type == BinaryType.Binary)
                    {
                        if (mat.At<byte>(l, k) > num)
                        {
                            mat.At<byte>(l, k) = byte.MaxValue;
                        }
                        else
                        {
                            mat.At<byte>(l, k) = 0;
                        }
                    }
                    else if (mat.At<byte>(l, k) < num)
                    {
                        mat.At<byte>(l, k) = byte.MaxValue;
                    }
                    else
                    {
                        mat.At<byte>(l, k) = 0;
                    }
                }
            }

            return mat;
        }

        public Bitmap Otsu_TY(Bitmap SrcImage, BinaryType Type)
        {
            int num = 0;
            int[] histogram_TY = GetHistogram_TY(SrcImage);
            double[] array = new double[256];
            double[] array2 = new double[256];
            double[] array3 = new double[256];
            double num2 = SrcImage.Width * SrcImage.Height;
            double num3 = 0.0;
            double num4 = 0.0;
            for (int i = 0; i < 256; i++)
            {
                array[i] = (double)histogram_TY[i] / num2;
                array2[i] = num3 + array[i];
                num3 = array2[i];
                array3[i] = num4 + (double)i * array[i];
                num4 = array3[i];
            }

            double num5 = 0.0;
            for (int j = 0; j < 256; j++)
            {
                if ((array3[255] * array2[j] - array3[j]) * (array3[255] * array2[j] - array3[j]) / (array2[j] * (1.0 - array2[j])) > num5)
                {
                    num5 = (array3[255] * array2[j] - array3[j]) * (array3[255] * array2[j] - array3[j]) / (array2[j] * (1.0 - array2[j]));
                    num = j;
                }
            }

            Bitmap bitmap = new Bitmap(SrcImage);
            for (int k = 0; k < bitmap.Width; k++)
            {
                for (int l = 0; l < bitmap.Height; l++)
                {
                    if (Type == BinaryType.Binary)
                    {
                        if (bitmap.GetPixel(k, l).R > num)
                        {
                            bitmap.SetPixel(k, l, Color.White);
                        }
                        else
                        {
                            bitmap.SetPixel(k, l, Color.Black);
                        }
                    }
                    else if (bitmap.GetPixel(k, l).R < num)
                    {
                        bitmap.SetPixel(k, l, Color.White);
                    }
                    else
                    {
                        bitmap.SetPixel(k, l, Color.Black);
                    }
                }
            }

            return bitmap;
        }

        public double GetMeanValue(Mat SrcImage)
        {
            int num = 0;
            for (int i = 0; i < SrcImage.Cols; i++)
            {
                for (int j = 0; j < SrcImage.Rows; j++)
                {
                    byte b = SrcImage.At<byte>(j, i);
                    num += b;
                }
            }

            return (double)num * 1.0 / (double)(SrcImage.Cols * SrcImage.Rows);
        }

        public double GetMeanValue(Bitmap SrcImage)
        {
            int num = 0;
            for (int i = 0; i < SrcImage.Width; i++)
            {
                for (int j = 0; j < SrcImage.Height; j++)
                {
                    byte r = SrcImage.GetPixel(i, j).R;
                    num += r;
                }
            }

            return (double)num * 1.0 / (double)(SrcImage.Width * SrcImage.Height);
        }

        public T Decode<T>(Mat SrcImage) where T : class
        {
            object obj = null;
            return obj as T;
        }

        public T Decode<T>(Bitmap SrcImage) where T : class
        {
            object obj = null;
            return obj as T;
        }

        public double GetContoursArea(OpenCvSharp.Point[] points)
        {
            double num = 0.0;
            for (int i = 0; i < points.Length; i++)
            {
                if (i > 1)
                {
                    double num2 = 0.5 * (double)(points[0].X * points[i - 1].Y + points[i - 1].X * points[i].Y + points[i].X * points[0].Y - points[0].X * points[i].Y - points[i - 1].X * points[0].Y - points[i].X * points[i - 1].Y);
                    num += num2;
                }
            }

            return num;
        }

        public double GetContoursArea(System.Drawing.Point[] points)
        {
            double num = 0.0;
            for (int i = 0; i < points.Length; i++)
            {
                if (i > 1)
                {
                    double num2 = 0.5 * (double)(points[0].X * points[i - 1].Y + points[i - 1].X * points[i].Y + points[i].X * points[0].Y - points[0].X * points[i].Y - points[i - 1].X * points[0].Y - points[i].X * points[i - 1].Y);
                    num += num2;
                }
            }

            return num;
        }

        public int FindMax(double[] array)
        {
            int result = 0;
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] > array[i + 1])
                {
                    double num = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = num;
                    result = i;
                }
            }

            return result;
        }

        public int FindMin(double[] array)
        {
            int result = 0;
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array[i] < array[i + 1])
                {
                    double num = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = num;
                    result = i;
                }
            }

            return result;
        }

        public double[] Convolution(Mat SrcImage, double[][] Kernel)
        {
            double[] array = new double[SrcImage.Rows * SrcImage.Cols];
            SrcImage.GetArray<byte>(out var data);
            int num = Kernel[0].Length;
            int num2 = Kernel.Length;
            for (int i = 0; i < SrcImage.Rows - num + 1; i++)
            {
                for (int j = 0; j < SrcImage.Cols - num2 + 1; j++)
                {
                    double num3 = 0.0;
                    for (int k = 0; k < Kernel.Length; k++)
                    {
                        for (int l = 0; l < Kernel[0].Length; l++)
                        {
                            double num4 = 1.0 * (double)(int)data[(i + k) * SrcImage.Rows + (j + l)] * Kernel[k][l];
                            num3 += num4;
                        }
                    }

                    array[(i + num / 2) * SrcImage.Rows + (j + num2 / 2)] = num3;
                }
            }

            return array;
        }

        public double[] Convolution(Bitmap SrcImage, double[][] Kernel)
        {
            double[] array = new double[SrcImage.Height * SrcImage.Width];
            int num = Kernel[0].Length;
            int num2 = Kernel.Length;
            for (int i = 0; i < SrcImage.Width - num2 + 1; i++)
            {
                for (int j = 0; j < SrcImage.Height - num + 1; j++)
                {
                    double num3 = 0.0;
                    for (int k = 0; k < num2; k++)
                    {
                        for (int l = 0; l < num; l++)
                        {
                            double num4 = 1.0 * (double)(int)SrcImage.GetPixel(k + i, l + j).R * Kernel[k][l];
                            num3 += num4;
                        }
                    }

                    array[(i + num / 2) * SrcImage.Height + (j + num2 / 2)] = num3;
                }
            }

            return array;
        }

        public List<ImageInfo> GetFlagPosition(List<ImageInfo> imageInfos)
        {
            List<ImageInfo> list = new List<ImageInfo>();
            list.Add(imageInfos[imageInfos.Count - 1]);
            for (int i = 1; i < imageInfos.Count; i++)
            {
                bool flag = true;
                for (int j = 0; j < list.Count; j++)
                {
                    int num = Math.Abs(list[j].Center.X - imageInfos[imageInfos.Count - i - 1].Center.X);
                    int num2 = Math.Abs(list[j].Center.Y - imageInfos[imageInfos.Count - i - 1].Center.Y);
                    if (num > 100 && num2 < 3)
                    {
                        flag = flag;
                        if (list[j].Center.X > imageInfos[imageInfos.Count - i - 1].Center.X)
                        {
                            OpenCvSharp.Point leftTop = new OpenCvSharp.Point(list[j].LeftTop.X - 104, list[j].LeftTop.Y);
                            imageInfos[imageInfos.Count - i - 1].LeftTop = leftTop;
                            OpenCvSharp.Point center = new OpenCvSharp.Point(list[j].Center.X - 104, list[j].Center.Y);
                            imageInfos[imageInfos.Count - i - 1].Center = center;
                            OpenCvSharp.Point rightBottom = new OpenCvSharp.Point(list[j].RightBottom.X - 104, list[j].RightBottom.Y);
                            imageInfos[imageInfos.Count - i - 1].RightBottom = rightBottom;
                        }
                        else
                        {
                            OpenCvSharp.Point leftTop2 = new OpenCvSharp.Point(list[j].LeftTop.X + 104, list[j].LeftTop.Y);
                            imageInfos[imageInfos.Count - i - 1].LeftTop = leftTop2;
                            OpenCvSharp.Point center2 = new OpenCvSharp.Point(list[j].Center.X + 104, list[j].Center.Y);
                            imageInfos[imageInfos.Count - i - 1].Center = center2;
                            OpenCvSharp.Point rightBottom2 = new OpenCvSharp.Point(list[j].RightBottom.X + 104, list[j].RightBottom.Y);
                            imageInfos[imageInfos.Count - i - 1].RightBottom = rightBottom2;
                        }
                    }
                    else if (num2 > 100 && num < 3)
                    {
                        flag = flag;
                        if (list[j].Center.Y > imageInfos[imageInfos.Count - i - 1].Center.Y)
                        {
                            OpenCvSharp.Point leftTop3 = new OpenCvSharp.Point(list[j].LeftTop.X, list[j].LeftTop.Y - 104);
                            imageInfos[imageInfos.Count - i - 1].LeftTop = leftTop3;
                            OpenCvSharp.Point center3 = new OpenCvSharp.Point(list[j].Center.X, list[j].Center.Y - 104);
                            imageInfos[imageInfos.Count - i - 1].Center = center3;
                            OpenCvSharp.Point rightBottom3 = new OpenCvSharp.Point(list[j].RightBottom.X, list[j].RightBottom.Y - 104);
                            imageInfos[imageInfos.Count - i - 1].RightBottom = rightBottom3;
                        }
                        else
                        {
                            OpenCvSharp.Point leftTop4 = new OpenCvSharp.Point(list[j].LeftTop.X, list[j].LeftTop.Y + 104);
                            imageInfos[imageInfos.Count - i - 1].LeftTop = leftTop4;
                            OpenCvSharp.Point center4 = new OpenCvSharp.Point(list[j].Center.X, list[j].Center.Y + 104);
                            imageInfos[imageInfos.Count - i - 1].Center = center4;
                            OpenCvSharp.Point rightBottom4 = new OpenCvSharp.Point(list[j].RightBottom.X, list[j].RightBottom.Y + 104);
                            imageInfos[imageInfos.Count - i - 1].RightBottom = rightBottom4;
                        }
                    }
                    else
                    {
                        if (num2 <= 100 || num <= 100)
                        {
                            flag = false;
                            break;
                        }

                        flag = flag;
                    }
                }

                if (flag)
                {
                    list.Add(imageInfos[imageInfos.Count - i - 1]);
                }

                if (list.Count == 3)
                {
                    break;
                }
            }

            return list;
        }

        public Rect GetCodePosition(List<ImageInfo> FlagInfos)
        {
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            list.Add(FlagInfos[0].LeftTop.X);
            list.Add(FlagInfos[0].RightBottom.X);
            list2.Add(FlagInfos[0].LeftTop.Y);
            list2.Add(FlagInfos[0].RightBottom.Y);
            list.Add(FlagInfos[1].LeftTop.X);
            list.Add(FlagInfos[1].RightBottom.X);
            list2.Add(FlagInfos[1].LeftTop.Y);
            list2.Add(FlagInfos[1].RightBottom.Y);
            list.Add(FlagInfos[2].LeftTop.X);
            list.Add(FlagInfos[2].RightBottom.X);
            list2.Add(FlagInfos[2].LeftTop.Y);
            list2.Add(FlagInfos[2].RightBottom.Y);
            list.Sort();
            list2.Sort();
            int width = list[list.Count - 1] - list[0] + 1;
            int height = list2[list2.Count - 1] - list2[0] + 1;
            return new Rect(list[0], list2[0], width, height);
        }

        public Mat GaussianBlur_TY(Mat SrcImage)
        {
            Mat mat = SrcImage.Clone();
            double[][] array = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                array[i] = new double[3];
                for (int j = 0; j < 3; j++)
                {
                    array[i][j] = 0.1111111111111111;
                }
            }

            for (int k = 0; k < SrcImage.Rows - 2; k++)
            {
                for (int l = 0; l < SrcImage.Cols - 2; l++)
                {
                    double num = 0.0;
                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            double num2 = array[m][n] * (double)(int)SrcImage.At<byte>(k + m, l + n);
                            num += num2;
                        }
                    }

                    int num3 = (int)num;
                    mat.At<byte>(k + 1, l + 1) = (byte)num3;
                }
            }

            return mat;
        }

        public Bitmap GaussianBlur_TY(Bitmap SrcImage)
        {
            Bitmap bitmap = new Bitmap(SrcImage);
            double[][] array = new double[3][];
            for (int i = 0; i < 3; i++)
            {
                array[i] = new double[3];
                for (int j = 0; j < 3; j++)
                {
                    array[i][j] = 0.1111111111111111;
                }
            }

            for (int k = 0; k < SrcImage.Height - 2; k++)
            {
                for (int l = 0; l < SrcImage.Width - 2; l++)
                {
                    double num = 0.0;
                    for (int m = 0; m < 3; m++)
                    {
                        for (int n = 0; n < 3; n++)
                        {
                            double num2 = array[m][n] * (double)(int)SrcImage.GetPixel(k + m, l + n).R;
                            num += num2;
                        }
                    }

                    int num3 = (int)num;
                    bitmap.SetPixel(k + 1, l + 1, Color.FromArgb((byte)num3, (byte)num3, (byte)num3));
                }
            }

            return bitmap;
        }

        public Mat BitwiseNot_TY(Mat SrcImage)
        {
            Mat mat = SrcImage.Clone();
            for (int i = 0; i < SrcImage.Rows; i++)
            {
                for (int j = 0; j < SrcImage.Cols; j++)
                {
                    if (SrcImage.At<byte>(i, j) == 0)
                    {
                        mat.At<byte>(i, j) = byte.MaxValue;
                    }
                    else
                    {
                        mat.At<byte>(i, j) = 0;
                    }
                }
            }

            return mat;
        }

        public Bitmap BitwiseNot_TY(Bitmap SrcImage)
        {
            Bitmap bitmap = new Bitmap(SrcImage);
            for (int i = 0; i < SrcImage.Width; i++)
            {
                for (int j = 0; j < SrcImage.Height; j++)
                {
                    if (SrcImage.GetPixel(i, j) == Color.Black)
                    {
                        bitmap.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }

            return bitmap;
        }

        public string XmlSerialize<T>(T obj)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(stringWriter, obj);
                stringWriter.Close();
                return stringWriter.ToString();
            }
        }

        public T XmlDeserializer<T>(string strXML) where T : class
        {
            try
            {
                using (StringReader textReader = new StringReader(strXML))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    return xmlSerializer.Deserialize(textReader) as T;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
