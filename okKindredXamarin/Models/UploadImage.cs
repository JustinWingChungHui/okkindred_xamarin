using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace okKindredXamarin.Models
{
    public class UploadImage
    {
        public UploadImage(string path, string type)
        {
            var data = File.ReadAllBytes(path);
            this.Data = Convert.ToBase64String(data);
            this.FileName = Path.GetFileName(path);
            this.MimeType = type; // $"image/{Path.GetExtension(path)}";
        }

        public UploadImage(byte[] data, string filename, string type)
        {
            this.Data = Convert.ToBase64String(data);
            this.FileName = filename;
            this.MimeType = type; // $"image/{Path.GetExtension(path)}";
        }

        public string FileName { get; set; }

        public string Data { get; set; }

        public string MimeType { get; set; }

        public override string ToString()
        {
            return $"{{ base64Image: 'data:{MimeType};base64,{Data}', fileName: '{FileName}', mimeType: '{MimeType}' }}";
        }
    }
}
