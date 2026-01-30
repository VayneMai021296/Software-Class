using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licencse
{
    class EncryptData
    {
        public static string Encode(string? input, string signature = "myapp001NeverKnowLicense")
        {
            if (input == string.Empty) return "";
            int lenInput = input.Length;
            int lenSig = signature.Length;
            StringBuilder builder = new StringBuilder(lenInput);
            for(int i =0 , j = 0; i < lenInput; i++, j++)
            {

                char inp = input[i];
                if(j == lenSig)
                    j = 0;           

                char key = signature[j];
                inp += key;

                if(inp > 127)
                {
                    inp = (char)(inp - 127 +32);
                }

                builder.Append(inp);
            }

            return builder.ToString();
        }
        public static string Decode(string? input, string signature = "myapp001NeverKnowLicense")
        {
            int lenInput = input.Length;

            StringBuilder builder = new StringBuilder(lenInput);
            int lenSig = signature.Length;

            for (int i = 0, j = 0; i < lenInput; i++, j++)
            {
                char inp = input[i];
                if (j == lenSig)
                    j = 0;
                char key = signature[j];
                inp -= key;

                char decodeInput = inp;

                if (inp < 32 || inp >127)
                {
                    decodeInput = (char)(inp - 32 + 127);
                }
                builder.Append(decodeInput);
            }
            return builder.ToString();
        }
    }
}
