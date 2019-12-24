using System.IO;
using System.Net;
using static System.Net.Mime.MediaTypeNames;

namespace Crawler.Models
{
    public class ImageHelper
    {
        public string Url { get; set; }
        public ImageHelper(string url)
        {
            Url = url;
        }

        public byte[] GetContent()
        {
            byte[] contents;
            var req = (HttpWebRequest)WebRequest.Create(Url);
            using (var res = req.GetResponse())
            using (var reader = new BinaryReader(res.GetResponseStream()))
            {
                contents = reader.ReadBytes(100000);
            }
            return contents;
        }

        public int Rescale(int width, int height, int newHeight)
        {
            var scale = System.Convert.ToDouble(newHeight) / System.Convert.ToDouble(height);
            return System.Convert.ToInt32(width * scale);
        }

        // public string GetRescaledImage()
        // {
        //     var img = GetContent();
        //     Image original
        // }
    }
}