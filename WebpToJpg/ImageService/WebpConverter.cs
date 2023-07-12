using System.IO;
using Imazen.WebP;
using System.Drawing;
using System.Drawing.Imaging;

namespace WebpToJpg.ImageService
{
    public enum PictureFormat
    {
        jpg,
        webp
    }

    class WebpConverter
    {
        private readonly SimpleDecoder decoder = new SimpleDecoder();
        private readonly SimpleEncoder encoder = new SimpleEncoder();

        public void ConvertWebpToJpg(string sourseFile, string destinationFile)
        {
            byte[] buffer = File.ReadAllBytes(sourseFile);
            using (Bitmap bitmap = decoder.DecodeFromBytes(buffer, buffer.Length))
            {
                bitmap.Save(destinationFile, ImageFormat.Jpeg);
            }
        }

        public void ConvertJpgToWebp(string sourseFile, string destinationFile)
        {
            using (Bitmap bitmap = new Bitmap(sourseFile))
            {
                using (FileStream fileStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
                {
                    encoder.Encode(bitmap, fileStream, 100);
                }
            }
        }
    }
}