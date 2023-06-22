using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorServiceManager
{
    internal class Decryptor
    {
        internal static byte[] DecryptData(byte[] data)
        {
            var result = new byte[data.Length];

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ 255);
            }

            return result;
        }
    }
}
