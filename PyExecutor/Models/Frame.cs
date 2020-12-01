using System.Drawing;

namespace PyExecutor.Models
{
    public class Frame
    {
        public byte[] BitmapBytes { get; set; }
        public Bitmap BitmapImage { get; set; }
        public short Index { get; set; }

        public Frame(byte[] BitmapBytes, short Index)
        {
            this.BitmapBytes = BitmapBytes;
            this.Index = Index;
        }

        public Frame(Bitmap BitmapImage, short Index)
        {
            this.BitmapImage = BitmapImage;
            this.Index = Index;
        }
    }
}