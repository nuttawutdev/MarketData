using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MarketData.Helper
{
    public class Utility
    {
        public enum MonthEnum
        {
            Undefined, // Required here even though it's not a valid month
            January,
            February,
            March,
            April,
            May,
            June,
            July,
            August,
            September,
            October,
            November,
            December
        }

        public static DateTime GetDateNowThai()
        {
            var dateUtc = DateTime.UtcNow;
            return dateUtc.AddHours(7);
        }

        public static string Encrypt(string plainText)
        {
            string passPhrase = "avbxwD5j4Ku4NEvlakuTp1MAR";
            string salt = "GHTYUILSFSgN4=@RTY";
            string initVector = "HGT@+_OYTV!FDRTY";

            //Create instance of RijndaelManaged
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };

            //iterations to derive key
            int keyIterations = 10000;

            //Convert the cipher text to UTF Bytes
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            //Hash the user passphrase with the salt provided to create a key
            byte[] keyBytes = new Rfc2898DeriveBytes(passPhrase, Encoding.ASCII.GetBytes(salt), keyIterations).GetBytes(256 / 8);

            //Starts the Encryption using the Key and Inititalisation Vector 
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(initVector));

            byte[] cipherTextBytes;

            //Creates a MemoryStream to do the encryption in
            using (var memoryStream = new MemoryStream())
            {
                //Creates the new Cryptology Stream --> Outputs to Memory Stream
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    //Writes the string in the Cryptology Stream
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    //Takes the MemoryStream and puts it to an array
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            //Converts the array from Base 64 to a string and returns
            return Convert.ToBase64String(cipherTextBytes);
        }
    }
}
