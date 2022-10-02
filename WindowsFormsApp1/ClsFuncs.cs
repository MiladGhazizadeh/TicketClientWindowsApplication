using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketClientApp
{
    internal class ClsFuncs
    {
        const string DESKey = "ERTYHJKI";
        const string DESIV = "PLUDUMBG";

        public static string DESDecrypt(string stringToDecrypt)//Decrypt the content
        {

            byte[] key;
            byte[] IV;

            byte[] inputByteArray;
            try
            {

                key = Convert2ByteArray(DESKey);

                IV = Convert2ByteArray(DESIV);

                int len = stringToDecrypt.Length; inputByteArray = Convert.FromBase64String(stringToDecrypt);


                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                MemoryStream ms = new MemoryStream();

                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);

                cs.FlushFinalBlock();

                Encoding encoding = Encoding.UTF8; return encoding.GetString(ms.ToArray());
            }

            catch
            {
                //return ex.Message;
                return "ExeptionError";
                //throw ex;
            }


        }

        public static string DESEncrypt(string stringToEncrypt)// Encrypt the content
        {

            byte[] key;
            byte[] IV;

            byte[] inputByteArray;
            try
            {

                key = Convert2ByteArray(DESKey);

                IV = Convert2ByteArray(DESIV);

                inputByteArray = Encoding.UTF8.GetBytes(stringToEncrypt);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                MemoryStream ms = new MemoryStream(); CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);

                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }

            catch (System.Exception ex)
            {

                throw ex;
            }

        }

        static byte[] Convert2ByteArray(string strInput)
        {
            int intCounter; char[] arrChar;
            arrChar = strInput.ToCharArray();

            byte[] arrByte = new byte[arrChar.Length];

            for (intCounter = 0; intCounter <= arrByte.Length - 1; intCounter++)
                arrByte[intCounter] = Convert.ToByte(arrChar[intCounter]);

            return arrByte;
        }


        public static void WriteTotextFile(TicketWebService.UserAuthenticateRet authenticate_ret)
        {

            string fileName = Application.StartupPath + "\\temp.txt";
            FileInfo fi = new FileInfo(fileName);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }


                // Create a new file     
                //using (FileStream fs = fi.Create())
                using (FileStream fs = File.Create(fileName))
                {
                    Byte[] txt1 = new UTF8Encoding(true).GetBytes(ClsFuncs.DESEncrypt(authenticate_ret.user_san) + "\n");
                    fs.Write(txt1, 0, txt1.Length);

                    Byte[] txt2 = new UTF8Encoding(true).GetBytes(ClsFuncs.DESEncrypt(authenticate_ret.user_cn) + "\n");
                    fs.Write(txt2, 0, txt2.Length);

                    Byte[] txt3 = new UTF8Encoding(true).GetBytes(authenticate_ret.token + "\n");
                    fs.Write(txt3, 0, txt3.Length);

                    Byte[] img = new UTF8Encoding(true).GetBytes(authenticate_ret.user_image + "\n");
                    fs.Write(img, 0, img.Length);

                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }

        public static void DeleteFiles_for_BeforeInstalation()
        {

            string fileName = Application.StartupPath + "\\config.txt";
            FileInfo fi = new FileInfo(fileName);

            // Check if file already exists. If yes, delete it.     
            if (fi.Exists)
            {
                string fileName2 = Application.StartupPath + "\\temp.txt";
                FileInfo fi2 = new FileInfo(fileName2);
                if (fi2.Exists)
                    fi2.Delete();


                fi.Delete();
            }


        }
    }
}
