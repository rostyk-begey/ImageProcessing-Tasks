using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Equalization_and_Filters.Infrastructure
{
    public class Filters
    {
        public static Bitmap Equalize(Bitmap bmp)
        {
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * bmp.Height;
            byte[] values = new byte[bytes];
            Marshal.Copy(ptr, values, 0, bytes);

            for (int c = 0; c < 3; c++)
            {
                int[] R = new int[256];
                byte[] N = new byte[256];
                for (int i = 0; i < values.Length / 3; ++i)
                {
                    ++R[values[3 * i + c]];
                }
                int z = 0;
                int Hint = 0;
                int Havg = values.Length / 3 / R.Length;
                for (int j = 0; j < R.Length; ++j)
                {
                    if (z > 255) N[j] = 255;
                    else N[j] = (byte) z;
                    Hint += R[j];
                    while (Hint > Havg)
                    {
                        Hint -= Havg;
                        z++;
                    }
                }
                for (int i = 0; i < values.Length / 3; ++i)
                {
                    values[3 * i + c] = N[values[3 * i + c]];
                }
            }

            Marshal.Copy(values, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Bitmap HistEq(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            BitmapData sd = img.LockBits(new Rectangle(0, 0, w, h),
            ImageLockMode.ReadOnly,
            PixelFormat.Format32bppArgb);
            int bytes = sd.Stride * sd.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(sd.Scan0, buffer, 0, bytes);
            img.UnlockBits(sd);

            int current = 0;
            double[] pn = new double[256];
            for (int p = 0; p < bytes; p += 4)
            {
                pn[buffer[p]]++;
            }
            for (int prob = 0; prob < pn.Length; prob++)
            {
                pn[prob] /= (w * h);
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    current = y * sd.Stride + x * 4;
                    double sum = 0;
                    for (int i = 0; i < buffer[current]; i++)
                    {
                        sum += pn[i];
                    }
                    for (int c = 0; c < 3; c++)
                    {
                        result[current + c] = (byte) Math.Floor(255 * sum);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap res = new Bitmap(w, h);
            BitmapData rd = res.LockBits(new Rectangle(0, 0, w, h),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, rd.Scan0, bytes);
            res.UnlockBits(rd);
            return res;
        }

        public static Bitmap Roberts(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width,
            original.Height,
            PixelFormat.Format32bppArgb);
            int width = original.Width;
            int height = original.Height;

            int[,] h1 = new int[,]
            {
                { 0, -1 },
                { 1, 0 }
            };
            int[,] h2 = new int[,]
            {
                { -1, 0 },
                { 0, 1 }
            };

            int[,] R = new int[width, height];
            int[,] G = new int[width, height];
            int[,] B = new int[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    R[i, j] = original.GetPixel(i, j).R;
                    G[i, j] = original.GetPixel(i, j).G;
                    B[i, j] = original.GetPixel(i, j).B;
                }
            }

            int Rx = 0, Ry = 0;
            int Gx = 0, Gy = 0;
            int Bx = 0, By = 0;
            int RChannel, GChannel, BChannel;
            for (int i = 1; i < original.Width - 1; ++i)
            {
                for (int j = 1; j < original.Height - 1; ++j)
                {
                    Rx = 0;
                    Ry = 0;
                    Gx = 0;
                    Gy = 0;
                    Bx = 0;
                    By = 0;

                    for (int x = -1; x < 1; ++x)
                    {
                        for (int y = -1; y < 1; ++y)
                        {
                            RChannel = R[i + y, j + x];
                            Rx += h1[x + 1, y + 1] * RChannel;
                            Ry += h2[x + 1, y + 1] * RChannel;

                            GChannel = G[i + y, j + x];
                            Gx += h1[x + 1, y + 1] * GChannel;
                            Gy += h2[x + 1, y + 1] * GChannel;

                            BChannel = B[i + y, j + x];
                            Bx += h1[x + 1, y + 1] * BChannel;
                            By += h2[x + 1, y + 1] * BChannel;
                        }
                    }
                    result.SetPixel(i,
                    j,
                    Color.FromArgb(IncreaseSat(Rx + Ry),
                    IncreaseSat(Gx + Gy),
                    IncreaseSat(Bx + By)));
                }
            }

            return result;
        }

        public static Bitmap Previt(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            int width = original.Width;
            int height = original.Height;

            int[,] GX =
            {
                { 1, 0, -1 },
                { 1, 0, -1 },
                { 1, 0, -1 }
            };
            int[,] GY =
            {
                { -1, -1, -1 },
                { 0, 0, 0 },
                { 1, 1, 1 }
            };

            int[,] R = new int[width, height];
            int[,] G = new int[width, height];
            int[,] B = new int[width, height];
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    R[i, j] = original.GetPixel(i, j).R;
                    G[i, j] = original.GetPixel(i, j).G;
                    B[i, j] = original.GetPixel(i, j).B;
                }
            }

            int Rx = 0, Ry = 0;
            int Gx = 0, Gy = 0;
            int Bx = 0, By = 0;
            int RChannel, GChannel, BChannel;
            for (int i = 1; i < original.Width - 1; ++i)
            {
                for (int j = 1; j < original.Height - 1; ++j)
                {
                    Rx = 0;
                    Ry = 0;
                    Gx = 0;
                    Gy = 0;
                    Bx = 0;
                    By = 0;

                    for (int x = -1; x < 2; ++x)
                    {
                        for (int y = -1; y < 2; ++y)
                        {
                            RChannel = R[i + y, j + x];
                            Rx += GX[x + 1, y + 1] * RChannel;
                            Ry += GY[x + 1, y + 1] * RChannel;

                            GChannel = G[i + y, j + x];
                            Gx += GX[x + 1, y + 1] * GChannel;
                            Gy += GY[x + 1, y + 1] * GChannel;

                            BChannel = B[i + y, j + x];
                            Bx += GX[x + 1, y + 1] * BChannel;
                            By += GY[x + 1, y + 1] * BChannel;
                        }
                    }
                    result.SetPixel(i, j, Color.FromArgb(IncreaseSat(Rx + Ry), IncreaseSat(Gx + Gy), IncreaseSat(Bx + By)));
                }
            }
            return result;
        }

        public static Bitmap Sobel(Bitmap original)
        {
            Bitmap result = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
            int width = original.Width;
            int height = original.Height;

            int[,] GX =
            {
                { 1, 0, -1 },
                { 2, 0, -2 },
                { 1, 0, -1 }
            };
            int[,] GY =
            {
                { -1, -2, -1 },
                { 0, 0, 0 },
                { 1, 2, 1 }
            };

            int[,] R = new int[width, height];
            int[,] G = new int[width, height];
            int[,] B = new int[width, height];

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    R[i, j] = original.GetPixel(i, j).R;
                    G[i, j] = original.GetPixel(i, j).G;
                    B[i, j] = original.GetPixel(i, j).B;
                }
            }

            int Rx = 0, Ry = 0;
            int Gx = 0, Gy = 0;
            int Bx = 0, By = 0;
            int RChannel, GChannel, BChannel;
            for (int i = 1; i < original.Width - 1; ++i)
            {
                for (int j = 1; j < original.Height - 1; ++j)
                {
                    Rx = 0;
                    Ry = 0;
                    Gx = 0;
                    Gy = 0;
                    Bx = 0;
                    By = 0;

                    for (int x = -1; x < 2; ++x)
                    {
                        for (int y = -1; y < 2; ++y)
                        {
                            RChannel = R[i + y, j + x];
                            Rx += GX[x + 1, y + 1] * RChannel;
                            Ry += GY[x + 1, y + 1] * RChannel;

                            GChannel = G[i + y, j + x];
                            Gx += GX[x + 1, y + 1] * GChannel;
                            Gy += GY[x + 1, y + 1] * GChannel;

                            BChannel = B[i + y, j + x];
                            Bx += GX[x + 1, y + 1] * BChannel;
                            By += GY[x + 1, y + 1] * BChannel;
                        }
                    }
                    result.SetPixel(i, j, Color.FromArgb(IncreaseSat(Rx + Ry), IncreaseSat(Gx + Gy), IncreaseSat(Bx + By)));
                }
            }
            return result;
        }

        private static int IncreaseSat(double number)
        {
            number *= 10;
            if (number < 0)
            {
                return 0;
            }
            if (number > 255)
            {
                return 255;
            }
            return (int) number;
        }
    }
}
