using System;
using System.Collections.Generic;
using System.Text;

namespace Ipfs.Manager.Tools
{
    static class ByteTools
    {
        public static List<byte> GetByteFromFile(string path)
        {
            List<byte> bytes = new List<byte>();
            try
            {
                using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    bool isEndOfStream = false;
                    while (!isEndOfStream)
                    {
                        int b = fs.ReadByte();
                        if (b != -1)
                        {
                            bytes.Add((byte)b);
                        }
                        else
                        {
                            isEndOfStream = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new AggregateException("Unknown error encountered", e);
            }
            return bytes;
        }
    }
}
