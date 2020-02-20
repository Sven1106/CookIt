using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
        public string GetOrSetScaledImage(string url, int scaleToPercentage)
        {
            string key = url + "ScaledToPercentage=" + scaleToPercentage;
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
                                image.Mutate(x =>
                                {
                                    x.Resize((image.Width / 100) * scaleToPercentage, (image.Height / 100) * scaleToPercentage);
                                });
                                base64 = image.ToBase64String(JpegFormat.Instance); // Automatic encoder selected based on extension.
                            }
                        }
                    }
                    catch (Exception)
                    {


                    }
                }
                keyValuePair = new KeyValuePair<string, string>(key, base64);
                _KvpDb.SetKvp(keyValuePair);
            }
            return keyValuePair.Value;
        }
    }
}
