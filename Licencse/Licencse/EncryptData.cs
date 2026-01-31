using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licencse
{
    class EncryptData
    {
        public static string Encode(string? input, string signature = "~yapp001NeverKnowLicense")
        {
            if (input == string.Empty) return "";
            int lenInput = input.Length;
            int lenSig = signature.Length;
            StringBuilder builder = new StringBuilder(lenInput);
            for (int i = 0; i < input.Length; i++)
            {
                int inp = input[i];
                int sig = signature[i % signature.Length];
                int shift = sig % 95;

                int encode = 32 + ((inp - 32 + shift) % 95);
                builder.Append((char)encode);
            }

            return builder.ToString();
        }
        public static string Decode(string? input, string signature = "~yapp001NeverKnowLicense")
        {
            int lenInput = input.Length;

            StringBuilder builder = new StringBuilder(lenInput);
            int lenSig = signature.Length;

            for(int i = 0; i < input.Length; i++)
        {
                int inp = input[i];

                int sig = signature[i % signature.Length];
                int shift = sig % 95;

                int decode = 32 + ((inp - 32 - shift + 95) % 95);
                builder.Append((char)decode);
            }
            return builder.ToString();
        }
    }
}
