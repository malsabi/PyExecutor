using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace PyExecutor.Utilities
{
    public static class BitmapExtensions
    {
        public static void JpegCompress(this Bitmap bmp, string filename)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
            bmp.Save(filename, GetEncoder(ImageFormat.Jpeg), encoderParameters);
        }

        public static void JpegCompress(this Bitmap bmp, Stream stream)
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 50L);
            bmp.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        public static Bitmap CaptureScreen()
        {
            Bitmap bmpScreenCapture = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmpScreenCapture))
            {
                g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, bmpScreenCapture.Size, CopyPixelOperation.SourceCopy);
            }
            return bmpScreenCapture;
        }

        public static void InternalRotateImage(this Bitmap originalBitmap, Bitmap rotatedBitmap, int rotationAngle)
        {
            int newWidth = rotatedBitmap.Width;
            int newHeight = rotatedBitmap.Height;

            int originalWidth = originalBitmap.Width;
            int originalHeight = originalBitmap.Height;

            int newWidthMinusOne = newWidth - 1;
            int newHeightMinusOne = newHeight - 1;

            BitmapData originalData = originalBitmap.LockBits(new Rectangle(0, 0, originalWidth, originalHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            BitmapData rotatedData = rotatedBitmap.LockBits(new Rectangle(0, 0, rotatedBitmap.Width, rotatedBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            unsafe
            {
                int* originalPointer = (int*)originalData.Scan0.ToPointer();
                int* rotatedPointer = (int*)rotatedData.Scan0.ToPointer();

                switch (rotationAngle)
                {
                    case 90:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = newWidthMinusOne - y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = x;
                                int destinationPosition = (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] = originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 180:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationY = (newHeightMinusOne - y) * newWidth;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationX = newWidthMinusOne - x;
                                int destinationPosition = (destinationX + destinationY);
                                rotatedPointer[destinationPosition] = originalPointer[sourcePosition];
                            }
                        }
                        break;
                    case 270:
                        for (int y = 0; y < originalHeight; ++y)
                        {
                            int destinationX = y;
                            for (int x = 0; x < originalWidth; ++x)
                            {
                                int sourcePosition = (x + y * originalWidth);
                                int destinationY = newHeightMinusOne - x;
                                int destinationPosition = (destinationX + destinationY * newWidth);
                                rotatedPointer[destinationPosition] = originalPointer[sourcePosition];
                            }
                        }
                        break;
                }
                originalBitmap.UnlockBits(originalData);
                rotatedBitmap.UnlockBits(rotatedData);
            }
        }

        public static string ImageToBase64(this Bitmap Src)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Src.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                var SigBase64 = Convert.ToBase64String(byteImage);
                return SigBase64;
            }
        }

        public static Bitmap Base64ToImage(this string ImgBase64)
        {
            byte[] ImageBytes = Convert.FromBase64String(ImgBase64);
            using (MemoryStream ms = new MemoryStream(ImageBytes))
            {
                Bitmap Src = (Bitmap)Image.FromStream(ms);
                return Src;
            }
        }
    }
}
