using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;

namespace ImageScalerLib
{
    public class ImageService
    {
        private readonly KvpDb _KvpDb;
        public ImageService(KvpDb kvpDb)
        {
            _KvpDb = kvpDb;
        }
        public string GetOrSetScaledImage(string url, int width, int height)
        {
            if (width == 0 && height == 0)
            {
                width = 100;
                height = 100;
            }

            string key = url + "&width=" + width + "&height=" + height;
            KeyValuePair<string, string> keyValuePair = _KvpDb.GetKvp(key);
            if (keyValuePair.Equals(default(KeyValuePair<string, string>)))
            {
                string base64 = "";
                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    try
                    {
                        using (Stream stream = webClient.OpenRead(url))
                        {
                            using (Image<Rgba32> image = Image.Load<Rgba32>(stream))
                            {
                                int newHeight = height == 0 ? (int)(width / (double)image.Width * image.Height) : height;
                                int newWidth = width == 0 ? (int)(height / (double)image.Height * image.Width) : width;
                                image.Mutate(x =>
                                {
                                    x.Resize(new ResizeOptions
                                    {
                                        Size = new Size(width, newHeight),
                                        Mode = ResizeMode.Crop
                                    });
                                });
                                base64 = image.ToBase64String(JpegFormat.Instance); // Automatic encoder selected based on extension.
                            }
                        }
                    }
                    catch (Exception)
                    {
                        string placeholderUrl = "";

                        placeholderUrl = "http://via.placeholder.com/" + width + "x" + height;

                        using (Stream stream = webClient.OpenRead(placeholderUrl))
                        {
                            using (Image<Rgba32> image = Image.Load<Rgba32>(stream))
                            {
                                base64 = image.ToBase64String(JpegFormat.Instance); // Automatic encoder selected based on extension.
                            }
                        }

                    }
                }
                keyValuePair = new KeyValuePair<string, string>(key, base64);
                _KvpDb.SetKvp(keyValuePair);
            }
            return keyValuePair.Value;
        }
    }
}
