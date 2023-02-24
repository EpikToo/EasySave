using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows;
using System.Reflection.PortableExecutable;

namespace EasySave_G8_UI.Models
{
    public class Model_AFT : Model_PRE
    {
        public double Size { get; set; }
        public string utcDateString { get; set; }
        private DateTime utcDateDateTime { get; set; }
        private DateTime utcDateStart { get; set; }
        private DateTime utcDateFinish { get; set; }
        private TimeSpan Duration { get; set; }
        public int total_files { get; set; }
        public int file_remain { get; set; }
        public double millisecondsDuration { get; set; }
        public double Total_CryptoTime { get; set; }
        private double ActualSize2 = 0;
        private Model_Logs ModelLogs = new Model_Logs();

        private bool PauseCurrentThread = false;
        private static bool BlacklistPauseCurrentThread = false;
        private bool StopCurrentThread = false;

        public static List<Model_AFT> AFTObjects = new List<Model_AFT>();

        private static SemaphoreSlim _semaphorejson = new SemaphoreSlim(1);
        private static SemaphoreSlim _semaphorexml = new SemaphoreSlim(1);
        public static SemaphoreSlim _semahporeAFTObjects = new SemaphoreSlim(1);

        public Model_AFT () { }
        public Model_AFT(string Name, string Source, string Destination, bool Type) : base(Name, Source, Destination, Type)
        {
            this.Name = Name;
            this.Source = Source;
            this.Destination = Destination;
            this.Type = Type;
            this.Size = 0;
            this.utcDateDateTime = DateTime.Now;
            this.utcDateString = utcDateDateTime.ToString("dd/MM/yyyy HH:mm:ss");
            this.Duration = TimeSpan.Zero;
            this.millisecondsDuration=0;
            this.total_files = Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories).Length;
            ModelLogs = new Model_Logs();
        }

        public void Run(object? sender) //Run a backup
        {
            
            Total_CryptoTime = 0;
            _semahporeAFTObjects.Wait();
            try { AFTObjects.Add(this); }
            finally { _semahporeAFTObjects.Release();}

            int percentage = 0;
                
            Model_StateLogs ModelStateLogs = new Model_StateLogs(this.Name, this.Source, this.Destination, this.Type, this.total_files); //init statelogs

            BackgroundWorker localworker = sender as BackgroundWorker; //localworker initialize
            localworker.WorkerReportsProgress = true; //allow localworker to report progress
            localworker.ReportProgress(0, Name); //report inital progress

            Model_PRIORITY model_PRIORITY = new Model_PRIORITY(); // create a new Model_Priority in order to have a priority list
            List<string> priorityList = model_PRIORITY.priorityReturn();

            Model_EXTENSION Model_Ext = new Model_EXTENSION(); // create a new Model_Priority in order to have a priority list
            List<string> CSExtNameList = Model_Ext.ExtensionReturn();

            Model_NBKO modelNbKo = new Model_NBKO();

            if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
            if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

            if (File.Exists(Source)) //If source is a file only
            {
                utcDateStart = DateTime.Now; //Initialize the date
                ModelStateLogs.progression = 0; // actualize the progression attribute on ModelStateLogs with actual percentage
                ModelStateLogs.file_remain = 1; // actualize the file remain attribute on ModelStateLogs with actual percentage
                ModelLogs.StateLog(ModelStateLogs); // Write the json state logs with new infos (it changes at each iteration)

                File.Copy(Source, Destination, true); //Run the save

                foreach (string ext in CSExtNameList)
                {
                    if (Path.GetExtension(Source) == ext)
                    {
                        Total_CryptoTime += Cryptosoft(Destination); //Encrypt the file and sum up
                    }
                }
                Size = new FileInfo(Source).Length;
                utcDateFinish = DateTime.Now;

                localworker.ReportProgress(100, Name); //report progress to actualize progressbar
                ModelStateLogs.CryptoTime = Total_CryptoTime;
                ModelStateLogs.progression = 100; // actualize the progression attribute on ModelStateLogs with actual percentage
                ModelStateLogs.file_remain = 0; // actualize the file remain attribute on ModelStateLogs with actual percentage
                ModelLogs.StateLog(ModelStateLogs); // Write the json state logs with new infos (it changes at each iteration)
            }

            else if (Directory.Exists(Source)) //If it's a folder
            {
                var files = Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories); //Get folders and files in the source directory
                List<string> files_NoPriority = new List<string>(files); // Convert Tab in List (in order to use Remove Method)
                List<string> files_Priority = new List<string>(); //Create list of priorityfiles
                List<string> files_LessPriority = new List<string>(); // Convert Tab in List (in order to use Remove Method)
                string Destination2 = Destination + @"\" + Path.GetFileName(Source); //Combine the destination directory with the file name of the source 
                string targetFile;

                utcDateStart = DateTime.Now;
                file_remain = total_files;

                Directory.CreateDirectory(Destination); //Create the destination directory if it doesn't exist
                Directory.CreateDirectory(Destination2); //Create the destination directory                    
                    
                foreach (var file in files) //Loop throught every files and add them to Pirority List
                {
                    if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                    if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                    Size = Size + new FileInfo(file).Length; //Increment size with each file
                    targetFile = file.Replace(Source, Destination2);
                    foreach (string ext in priorityList)
                    {
                        if (Path.GetExtension(file) == ext)
                        {
                            files_Priority.Add(file); // Add file in priority file
                            files_NoPriority.Remove(file); // Remove file from the all files in the list files_NoPriority (in order to have only no priority files)
                        }
                    }

                    if((new FileInfo(file).Length > modelNbKo.NbKoReturn()) && (modelNbKo.NbKoReturn() !=0))
                    {
                        Trace.WriteLine("Lengh du file en cours" + new FileInfo(file).Length + "retourne val nbko" + modelNbKo.NbKoReturn());
                        files_NoPriority.Remove(file); // Remove file from the all files in the list files_NoPriority (in order to have only no priority files)
                        files_Priority.Remove(file); // Remove file from the all files in the list files_Priority (in order to have only no priority files)
                        files_LessPriority.Add(file);
                    }
                }
                ModelStateLogs.Size = Size;

                foreach (var file in files_Priority)
                {
                    if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                    if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                    targetFile = file.Replace(Source, Destination2);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile)); // Create a directory

                    File.Copy(file, targetFile, true);  // Do the copy of priority Files
                    foreach (string ext in CSExtNameList)
                    {
                        if (Path.GetExtension(file) == ext)
                        {
                            Total_CryptoTime += Cryptosoft(targetFile); //Encrypt the file and sum up
                        }
                    }
                    ActualSize2 = ActualSize2 + new FileInfo(file).Length;//Increment size with each file
                    percentage = (int)(((double)ActualSize2 / (double)Size) * 100);//progression's percentage of the save
                    file_remain--; //File remain decrease when a file copy have been done

                    localworker.ReportProgress(percentage, Name); // report progress
                    ModelStateLogs.CryptoTime = Total_CryptoTime; // actualize cryptotime
                    ModelStateLogs.progression = percentage; // actualize the progression attribute on ModelStateLogs with actual percentage
                    ModelStateLogs.file_remain = file_remain; // actualize the file remain attribute on ModelStateLogs with actual percentage
                    ModelLogs.StateLog(ModelStateLogs); // Write the json state logs with new infos (it changes at each iteration)
                }

                foreach (var file in files_NoPriority) //Loop throught every files and copy them
                {
                    if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                    if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                    targetFile = file.Replace(Source, Destination2);
                    ActualSize2 = ActualSize2 + new FileInfo(file).Length;//Increment size with each file
                    percentage = (int)(((double)ActualSize2 / (double)Size) * 100);//progression's percentage of the save
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile)); // Create a directory

                    File.Copy(file, targetFile, true);  // Do the copy
                    foreach (string ext in CSExtNameList)
                    {
                        if (Path.GetExtension(file) == ext)
                        {
                            Total_CryptoTime += Cryptosoft(targetFile); //Encrypt the file and sum up
                        }
                    }
                    file_remain-- ; // File remain decrease when a file copy have been done

                    localworker.ReportProgress(percentage, Name); //report progress
                    ModelStateLogs.progression = percentage; // actualize the progression attribute on ModelStateLogs with actual percentage
                    ModelStateLogs.file_remain = file_remain; // actualize the file remain attribute on ModelStateLogs with actual percentage
                    ModelStateLogs.CryptoTime= Total_CryptoTime;
                    ModelLogs.StateLog(ModelStateLogs); // Write the json state logs with new infos (it changes at each iteration)
                }

                foreach (var file in files_LessPriority) 
                {
                    if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                    if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; ; }

                    targetFile = file.Replace(Source, Destination2);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile)); // Create a directory

                    File.Copy(file, targetFile, true);  // Do the copy of priority Files
                    foreach (string ext in CSExtNameList)
                    {
                        if (Path.GetExtension(file) == ext)
                        {
                            Total_CryptoTime += Cryptosoft(targetFile); //Encrypt the file and sum up
                        }
                    }
                    long sizeee = new FileInfo(file).Length;
                    ActualSize2 = ActualSize2 + sizeee;//Increment size with each file

                    percentage = (int)(((double)ActualSize2 / (double)Size) * 100);//progression's percentage of the save
                    file_remain--; //File remain decrease when a file copy have been done

                    localworker.ReportProgress(percentage, Name); // report progress
                    ModelStateLogs.CryptoTime = Total_CryptoTime; // actualize cryptotime
                    ModelStateLogs.progression = percentage; // actualize the progression attribute on ModelStateLogs with actual percentage
                    ModelStateLogs.file_remain = file_remain; // actualize the file remain attribute on ModelStateLogs with actual percentage
                    ModelLogs.StateLog(ModelStateLogs); // Write the json state logs with new infos (it changes at each iteration)
                }

                ModelStateLogs.file_remain = file_remain;
                utcDateFinish = DateTime.Now;
            }
            _semahporeAFTObjects.Wait();
            try { AFTObjects.Remove(this); }
            finally { _semahporeAFTObjects.Release(); }

            Duration = utcDateFinish.Subtract(utcDateStart);  // Calculation of the result of the arrival date - the departure date to obtain a duration, it's in TimeSpan, it is the result of the subtraction of two DataTime
            millisecondsDuration = Duration.TotalMilliseconds; // Convert Duration in milliseconds
            localworker.ReportProgress(100, Name); // Report end of operation to background worker
            ModelStateLogs.State = "ENDED";
            ModelStateLogs.progression = 100;
            ModelStateLogs.millisecondsDuration = millisecondsDuration; //add millisecondsDuration to the object ModelStateLogs
            ModelLogs.StateLog(ModelStateLogs);// Write the JSon State Logs with all info 
        }

        public void RunDiff(object? sender) //Execute a differential backup
        {
            {
                _semahporeAFTObjects.Wait();
                try { AFTObjects.Add(this); }
                finally { _semahporeAFTObjects.Release(); }

                int percentage = 0;
                Total_CryptoTime = 0;

                Model_StateLogs ModelStateLogs = new Model_StateLogs(this.Name, this.Source, this.Destination, this.Type, this.total_files); //init statelogs

                Model_EXTENSION Model_Ext = new Model_EXTENSION(); // create a new Model_Priority in order to have a priority list
                List<string> CSExtNameList = Model_Ext.ExtensionReturn();

                Model_NBKO modelNbKo = new Model_NBKO();

                BackgroundWorker localworker = sender as BackgroundWorker; //localworker initialize
                localworker.WorkerReportsProgress = true; //allow localworker to report progress
                localworker.ReportProgress(0, Name); //report inital progress

                Model_PRIORITY model_PRIORITY = new Model_PRIORITY(); // create a new Model_Priority in order to have a priority list
                List<string> priorityList = model_PRIORITY.priorityReturn();

                if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                if (!Directory.Exists(Destination)) { Directory.CreateDirectory(Destination); } //Create the destination directory if it doesn't exist
                if (Directory.Exists(Source))
                {
              
                    string[] sourceFiles = Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories); // Get the list of files in the source directory
                    string Destination2 = Destination + @"\" + Path.GetFileName(Source); //Combine the destination directory with the file name of the source 

                    utcDateStart = DateTime.Now;
                    Directory.CreateDirectory(Destination2); //Create the destination directory
                    file_remain = total_files;

                    List<string> files_NoPriority = new List<string>(sourceFiles); // Convert Tab in List (in order to use Remove Method)
                    List<string> files_Priority = new List<string>(); //Create list of priorityfiles
                    List<string> files_LessPriority = new List<string>(sourceFiles); // Convert Tab in List (in order to use Remove Method)

                    string targetFile;

                    foreach (var file in sourceFiles) //Loop throught every files and copy them
                    {
                        if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                        if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                        Size = Size + new System.IO.FileInfo(file).Length;//Increment size with each file
                        targetFile = file.Replace(Source, Destination2);

                        foreach (string ext in priorityList)
                        {
                            if (Path.GetExtension(file) == ext)
                            {
                                files_Priority.Add(file); // Add file in priority file
                                files_NoPriority.Remove(file); // Remove file from the all files in the list files_NoPriority (in order to have only no priority files)
                            }
                        }
                        if ((new FileInfo(file).Length > modelNbKo.NbKoReturn()) && (modelNbKo.NbKoReturn() != 0))
                        {
                            Trace.WriteLine("Lengh du file en cours" + new FileInfo(file).Length + "retourne val nbko" + modelNbKo.NbKoReturn());
                            files_NoPriority.Remove(file); // Remove file from the all files in the list files_NoPriority (in order to have only no priority files)
                            files_Priority.Remove(file); // Remove file from the all files in the list files_Priority (in order to have only no priority files)
                            files_LessPriority.Add(file);
                        }
                    }
                    ModelStateLogs.Size = Size;
                    string destinationFile;

                    foreach (string sourceFile in files_Priority) // Browse each file in the source directory
                    {
                        if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                        if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                        destinationFile = sourceFile.Replace(Source, Destination2); // Create a destination path for the file
                        ActualSize2 = ActualSize2 + new System.IO.FileInfo(sourceFile).Length;//Increment size with each file
                        percentage = (int)(((double)ActualSize2 / (double)Size) * 100);

                        localworker.ReportProgress(percentage, Name);
                        ModelStateLogs.progression = percentage;
                        file_remain --;

                        if (File.Exists(destinationFile)) // Check if the file already exists in the backup directory
                        {
                            FileInfo sourceFileInfo = new FileInfo(sourceFile); // Get information about source and destination files
                            FileInfo destinationFileInfo = new FileInfo(destinationFile);
                            
                            if (sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime) // Check if the file has been modified in the source directory
                            {
                                File.Copy(sourceFile, destinationFile, true); //Copy the modified file to the backup directory
                                foreach (string ext in CSExtNameList)
                                {
                                    if (Path.GetExtension(sourceFile) == ext)
                                    {
                                        Total_CryptoTime += Cryptosoft(destinationFile); //Encrypt the file and sum up
                                    }
                                }
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));// Copy the file that does not exist to the backup directory
                            File.Copy(sourceFile, destinationFile, false);
                            foreach (string ext in CSExtNameList)
                            {
                                if (Path.GetExtension(sourceFile) == ext)
                                {
                                    Total_CryptoTime += Cryptosoft(destinationFile); //Encrypt the file and sum up
                                }
                            }
                        }
                        ModelStateLogs.file_remain = file_remain;
                        ModelStateLogs.CryptoTime = Total_CryptoTime; // actualize cryptotime
                        ModelStateLogs.progression = percentage; // actualize the progression attribute on ModelStateLogs with actual percentage
                        ModelLogs.StateLog(ModelStateLogs);
                    }
                    foreach (string sourceFile in files_NoPriority) // Browse each file in the source directory
                    {
                        if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                        if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                        destinationFile = sourceFile.Replace(Source, Destination2); // Create a destination path for the file
                        ActualSize2 = ActualSize2 + new System.IO.FileInfo(sourceFile).Length;//Increment size with each file
                        percentage = (int)(((double)ActualSize2 / (double)Size) * 100);
                        localworker.ReportProgress(percentage, Name);
                        ModelStateLogs.progression = percentage;
                        file_remain--;

                        if (File.Exists(destinationFile)) // Check if the file already exists in the backup directory
                        {
                            FileInfo sourceFileInfo = new FileInfo(sourceFile); // Get information about source and destination files
                            FileInfo destinationFileInfo = new FileInfo(destinationFile);

                            if (sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime) // Check if the file has been modified in the source directory
                            {
                                File.Copy(sourceFile, destinationFile, true); //Copy the modified file to the backup directory
                                foreach (string ext in CSExtNameList)
                                {
                                    if (Path.GetExtension(sourceFile) == ext)
                                    {
                                        Total_CryptoTime += Cryptosoft(destinationFile); //Encrypt the file and sum up
                                    }
                                }
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));// Copy the file that does not exist to the backup directory
                            File.Copy(sourceFile, destinationFile, false);
                            foreach (string ext in CSExtNameList)
                            {
                                if (Path.GetExtension(sourceFile) == ext)
                                {
                                    Total_CryptoTime += Cryptosoft(destinationFile); //Encrypt the file and sum up
                                }
                            }
                        }
                        ModelStateLogs.file_remain = file_remain;
                        ModelStateLogs.CryptoTime = Total_CryptoTime; // actualize cryptotime
                        ModelStateLogs.progression = percentage; // actualize the progression attribute on ModelStateLogs with actual percentage
                        ModelLogs.StateLog(ModelStateLogs);
                    }
                    foreach (var sourceFile in files_LessPriority)
                    {
                        if (PauseCurrentThread || BlacklistPauseCurrentThread) { ThreadPause(); }
                        if (StopCurrentThread) { localworker.ReportProgress(100, Name); return; }

                        destinationFile = sourceFile.Replace(Source, Destination2); // Create a destination path for the file
                        ActualSize2 = ActualSize2 + new System.IO.FileInfo(sourceFile).Length;//Increment size with each file
                        percentage = (int)(((double)ActualSize2 / (double)Size) * 100);

                        localworker.ReportProgress(percentage, Name);
                        ModelStateLogs.progression = percentage;
                        file_remain--;

                        if (File.Exists(destinationFile)) // Check if the file already exists in the backup directory
                        {
                            FileInfo sourceFileInfo = new FileInfo(sourceFile); // Get information about source and destination files
                            FileInfo destinationFileInfo = new FileInfo(destinationFile);

                            if (sourceFileInfo.LastWriteTime > destinationFileInfo.LastWriteTime) // Check if the file has been modified in the source directory
                            {
                                File.Copy(sourceFile, destinationFile, true); //Copy the modified file to the backup directory
                                foreach (string ext in CSExtNameList)
                                {
                                    if (Path.GetExtension(sourceFile) == ext)
                                    {
                                        Total_CryptoTime += Cryptosoft(destinationFile); //Encrypt the file and sum up
                                    }
                                }
                            }
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));// Copy the file that does not exist to the backup directory
                            File.Copy(sourceFile, destinationFile, false);
                            foreach (string ext in CSExtNameList)
                            {
                                if (Path.GetExtension(sourceFile) == ext)
                                {
                                    Total_CryptoTime += Cryptosoft(destinationFile); //Encrypt the file and sum up
                                }
                            }
                        }
                        ModelStateLogs.file_remain = file_remain;
                        ModelStateLogs.CryptoTime = Total_CryptoTime; // actualize cryptotime
                        ModelLogs.StateLog(ModelStateLogs);
                       
                    }
                    utcDateFinish = DateTime.Now;
                }
                localworker.ReportProgress(100, Name); // Report end of operation to background worker

                _semahporeAFTObjects.Wait();
                try { AFTObjects.Remove(this); }
                finally { _semahporeAFTObjects.Release(); }

                Duration = utcDateFinish.Subtract(utcDateStart); // Calculation of the result of the arrival date - the departure date to obtain a duration, it's in TimeSpan, it is the result of the subtraction of two DataTime
                millisecondsDuration = Duration.TotalMilliseconds; // Convert Duration in milliseconds
                ModelStateLogs.CryptoTime = Total_CryptoTime; // actualize cryptotime
                ModelStateLogs.millisecondsDuration = millisecondsDuration; //add millisecondsDuration to the object ModelStateLogs
                ModelStateLogs.State = "ENDED"; // Uptadte status of the save in order to write it in Json state logs
                ModelLogs.StateLog(ModelStateLogs); // Write the JSon State Logs with all info 
            }
        }

        private double Cryptosoft(string destination)
        {
            string appPath = Directory.GetCurrentDirectory() + @"\cryptosoft.exe";
            DateTime TimeStartCS = DateTime.Now; //Get starting time
            Process appProcess = new Process(); //Create the process

            destination = "\"" + destination + "\"";

            appProcess.StartInfo.FileName = appPath; //Starting CryptoSoft
            appProcess.StartInfo.Arguments = destination; //Pass the argument
            appProcess.StartInfo.CreateNoWindow= true;
            appProcess.Start(); //Start the process
            appProcess.WaitForExit(); //Wait for the app to complete
            appProcess.Close();  //Close the process

            DateTime TimeEndCS = DateTime.Now; //Get finish time
            TimeSpan DurationCS = TimeEndCS.Subtract(TimeStartCS); //Get the duration of the encrypting
            double msDurationCS = DurationCS.TotalMilliseconds; //and transform it in milliseconds
            return msDurationCS;
            
        }

        public void Logs() // Method to write backup logs
        {
            // Get the current UTC date and format it to allow serialization
            string utcDateOnly = utcDateDateTime.ToString("dd/MM/yyyy");
            utcDateOnly = utcDateOnly.Replace("/", "-"); //Format the date to allow serializing
            
            // Create file paths for the JSON and XML logs
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\logs\JSON\" + utcDateOnly + ".json";
            string fileName2 = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\logs\XML\" + utcDateOnly + ".xml";

            if (File.Exists(fileName)) // If the JSON log file already exists, update it with the current log data
            {
                string fileContent = null; // Read the current JSON log file and deserialize it into a list of Model_AFT objects

                _semaphorejson.Wait();
                try { fileContent = File.ReadAllText(fileName); } //Bring content of filename in filecontent
                finally { _semaphorejson.Release(); }

                List<Model_AFT> ?values = new List<Model_AFT>(); //Create the list named values
                values = JsonConvert.DeserializeObject<List<Model_AFT>>(fileContent); //Deserialialize the data in JSON form
                values?.Add(this); //Add object ModelAFT in the list values
                
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.Indented); //Serialialize the data in JSON form

                // Write the updated JSON log file and the XML log file
                _semaphorejson.Wait();
                try { File.WriteAllText(fileName, jsonString); }
                finally { _semaphorejson.Release(); }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Model_AFT>));
                _semaphorexml.Wait();
                try {
                    StreamWriter writer = new StreamWriter(fileName2);
                    serializer.Serialize(writer, values);
                    writer.Close();
                }
                finally { _semaphorexml.Release(); }
            }

            // If the JSON log file does not exist, create it and write the current log data to it
            else if (!File.Exists(fileName))
            {
                List<Model_AFT> values = new List<Model_AFT>(); // Create a new list of Model_AFT objects and add the current log data to it
                values.Add(this);// Add object ModelAFT in the list values
                var jsonString = JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.Indented); //Serialialize the data in JSON form
                
                _semaphorejson.Wait();
                try { File.WriteAllText(fileName, jsonString); } // Write json file
                finally { _semaphorejson.Release(); }

                XmlSerializer serializer = new XmlSerializer(typeof(List<Model_AFT>));
                
                _semaphorexml.Wait();
                try
                {
                    StreamWriter writer = new StreamWriter(fileName2);
                    serializer.Serialize(writer, values);
                    writer.Close();
                }
                finally { _semaphorexml.Release(); }
            }
        }

        //####################################################################################################################################
        //Threads Pause Play Stop
        private void ThreadPause()
        {
            while ((PauseCurrentThread || BlacklistPauseCurrentThread) && !StopCurrentThread) 
            { 
                Thread.Sleep(100);
            }
        }

        public void PauseSpecificThread()
        {
            if (PauseCurrentThread) { PauseCurrentThread = false; }
            else { PauseCurrentThread = true; }
        }

        public static void BlacklistPauseThreads() 
        {
            if (BlacklistPauseCurrentThread) { BlacklistPauseCurrentThread = false; }
            else { BlacklistPauseCurrentThread = true; }
        }

        public void StopSpecificThread()
        {
            StopCurrentThread = true;
        }
        //####################################################################################################################################
    }
}