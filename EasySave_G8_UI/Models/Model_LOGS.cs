using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace EasySave_G8_UI.Models
{
    public class Model_StateLogs : Model_AFT
    {
        public int progression { get; set; }
        public string State { get; set; }
        public double CryptoTime { get; set; }

        public Model_StateLogs(string Name, string Source, string Destination, bool Type, int file_remain) : base(Name, Source, Destination, Type)
        {
            this.Name = Name;
            this.Source = Source;
            this.Destination = Destination;
            this.Type = Type;
            this.Size = 0;
            this.file_remain = file_remain;
            this.total_files = Directory.GetFiles(Source, "*.*", SearchOption.AllDirectories).Length;
            this.progression = 0;
            this.CryptoTime = 0;
            this.State = "STARTED";
        }
    }

    public class Model_Logs
    {
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public void StateLog(Model_StateLogs statelog) //Write backup's state logs
        {
            _semaphore.Wait();
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\StateLog.json";
            if (File.Exists(fileName))  //Test if log file exists, else it creates it
            {
                string fileContent = File.ReadAllText(fileName); // Bring content of filename in filecontent            

                List<Model_StateLogs>? values = new List<Model_StateLogs>(); // Create the list named values
                List<Model_StateLogs>? new_values = new List<Model_StateLogs>();
                values = JsonConvert.DeserializeObject<List<Model_StateLogs>>(fileContent); //Deserialialize the data in JSON form

                foreach (Model_StateLogs obj in values) //Loop throught every objects in the deserialized data
                {
                    if (!(obj.Name == statelog.Name)) //If we find the save we are looking for in a single work execution
                    { 
                        new_values.Add(obj);
                    }
                }

                new_values.Add(statelog);
                var jsonString = JsonConvert.SerializeObject(new_values, Formatting.Indented); //Serialialize the data in JSON form

                File.WriteAllText(fileName, jsonString); // Write json file
            }
            else if (!File.Exists(fileName))
            {
                List<Model_StateLogs> values = new List<Model_StateLogs>(); // create the list named values
                values.Add(statelog);// Add object Model_StateLogs in the list values
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(values, Newtonsoft.Json.Formatting.Indented); //Serialialize the data in JSON form
                File.WriteAllText(fileName, jsonString); // Write json file
            }
            _semaphore.Release();
        }

        public List<Model_StateLogs> Get_StateLogs()
        {
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\StateLog.json";

            List<Model_StateLogs>? obj_list = new List<Model_StateLogs>(); //Create the list named obj_list to hold the return list

            if (File.Exists(fileName))
            {
                string fileContent = null;
                _semaphore.Wait();
                try { fileContent = File.ReadAllText(fileName); } //Bring content of filename in filecontent
                finally { _semaphore.Release(); }
                obj_list = JsonConvert.DeserializeObject<List<Model_StateLogs>>(fileContent); //Deserialialize the data in JSON form
                return (obj_list);
            }
            return (obj_list); //Return empty list if statelogs are not found
        }

        public bool StatelogExists(string Name)
        {
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\StateLog.json";
            string fileContent = File.ReadAllText(fileName); // Bring content of filename in filecontent

            List<Model_StateLogs>? values = new List<Model_StateLogs>(); // Create the list named values
            values = JsonConvert.DeserializeObject<List<Model_StateLogs>>(fileContent); //Deserialialize the data in JSON form
            foreach (Model_StateLogs obj in values) //Loop throught every objects in the deserialized data
            {
                if (obj.Name == Name) //If we find the save we are looking for in a single work execution
                {
                    return (true);
                }
            }
            return false;
        }

        public List<Model_AFT> Get_Logs(string Date) //Retrieve log file content
        {
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\logs\JSON\" + Date + ".json";

            List<Model_AFT>? obj_list = new List<Model_AFT>(); //Create the list named obj_list to hold the return list
            List<Model_AFT>? values = new List<Model_AFT>(); //Create the list named values to hold deserialized data

            if (File.Exists(fileName))
            {
                string fileContent = File.ReadAllText(fileName);

                values = JsonConvert.DeserializeObject<List<Model_AFT>>(fileContent); //Deserialialize the data in JSON form
                foreach (Model_AFT obj in values) //Loop throught every objects in the deserialized data
                {
                    obj_list.Add(obj); //Store the object into a list for return
                }
                return (obj_list);
            }
            return (obj_list); //Return empty list if today's log is not found
        }
    }
}
