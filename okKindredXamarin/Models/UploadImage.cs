using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace okKindredXamarin.Models
{
    public class UploadImage
    {
        public UploadImage(int index, string path)
        {
            this.Index = index;
            this.FilePath = path;
            this.FileName = Path.GetFileName(path);
            this.MimeType = $"image/{Path.GetExtension(path)}";
        }

        public UploadImage(int index, Stream stream, string path)
        {
            byte[] data;

            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                data = ms.ToArray();
            }

            this.Index = index;
            this.Data = Convert.ToBase64String(data);
            this.FileName = Path.GetFileName(path);
            this.MimeType = $"image/{Path.GetExtension(path)}";
        }

        public UploadImage(int index, byte[] data, string filename)
        {
            this.Index = index;
            this.Data = Convert.ToBase64String(data);
            this.FileName = filename;
            this.MimeType = $"image/{Path.GetExtension(filename)}";
        }

        public int Index { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string Data { get; set; }

        public string MimeType { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Data))
            {
                return $"{{ index:{Index}, path:'{FilePath}', fileName:'{FileName}', mimeType:'{MimeType}' }}";
            }
            else
            {
                return $"{{ base64Image: 'data:{MimeType};base64,{Data}', index:{Index}, path:'{FilePath}', fileName:'{FileName}', mimeType:'{MimeType}' }}";
            }            
        }
    }
}
