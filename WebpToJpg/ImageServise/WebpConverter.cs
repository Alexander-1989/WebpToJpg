using System.IO;
using Imazen.WebP;
using System.Drawing;
using System.Drawing.Imaging;

namespace WebpToJpg.ImageServise
{
    class WebpConverter
    {
        private readonly SimpleDecoder decoder = new SimpleDecoder();
        private readonly SimpleEncoder encoder = new SimpleEncoder();

        public void ConvertWebpToJpg(string sourse, string destination)
        {
            byte[] buffer = File.ReadAllBytes(sourse);
            using (Bitmap bitmap = decoder.DecodeFromBytes(buffer, buffer.Length))
            {
                bitmap.Save(destination, ImageFormat.Jpeg);
            }
        }

        public void ConvertJpgToWebp(string sourse, string destination)
        {
            using (Bitmap bitmap = new Bitmap(sourse))
            {
                using (FileStream fileStream = new FileStream(destination, FileMode.Create, FileAccess.Write))
                {
                    encoder.Encode(bitmap, fileStream, 100);
                }
            }
        }
    }
}