using System;
using System.IO;
using System.Runtime.InteropServices;

namespace StartArgs
{
    class ArgsEcho
    {
        static void Main(string[] args)
        {
            CryptoSoft(args[0]);
        }
        private static double CryptoSoft(string args)
        {
            using (var fin = new FileStream(args, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //using (var fin2 = new FileStream(args, FileMode.Truncate))
            //using (var fout = new FileStream(Destination, FileMode.Truncate))
            {
                byte[] buffer = new byte[4096];
                DateTime TimeStart = DateTime.Now; //Get starting time
                while (true)
                {
                    int bytesRead = fin.Read(buffer); //Read the file bit by bit
                    if (bytesRead == 0)
                        break;
                    fin.SetLength(0);
                    EncryptBytes(buffer, bytesRead); //Encrypt the file bit by bit
                    fin.Write(buffer, 0, bytesRead);
                }
                DateTime TimeEnd = DateTime.Now; //Get finish time
                TimeSpan Duration = TimeStart.Subtract(TimeEnd); //Get the duration of the encrypting
                double msDuration = Duration.TotalMilliseconds; //and transform it in milliseconds

                return msDuration;
            }
        }

        private static long getKey(string filePath) //Will get the key by reading the file created by EasySave
        {
            string inputString;

            using (StreamReader reader = new StreamReader(filePath)) //Open the file for reading
            {
                inputString = reader.ReadToEnd(); //Read the entire contents of the file as a string
            }

            long key;
            if (!long.TryParse(inputString, out key))
            {
                throw new ArgumentException("The input file contains invalid characters.");
            }

            return key;
        }
        private static long cipherKey = getKey(@"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\cipherkey.txt");

        private static void EncryptBytes(byte[] buffer, int count)
        {

            for (int i = 0; i < count; i++)
                buffer[i] = (byte)(buffer[i] ^ cipherKey); //XOR operation bit by bit
        }
    }
}

