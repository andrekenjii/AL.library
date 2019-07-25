using System.IO;
using System.IO.Compression;
using System.Text;

namespace AL.library.Utility.Tools
{
    public static class ByteTools
    {
        public static byte[] Decompress(this byte[] arrByte)
        {
            using (GZipStream stream = new GZipStream(new
            MemoryStream(arrByte), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        public static string GetStringFromByte(this byte[] arrByte)
        {
            return Encoding.Default.GetString(arrByte);
        }
    }
}
