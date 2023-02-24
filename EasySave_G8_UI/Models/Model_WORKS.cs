using EasySave_G8_UI.View_Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasySave_G8_UI.Models
{
    public class Model_Works
    {
        public List<Model_PRE> Get_Work(string? Name, bool AllBool) //Get Work data from work_conf and returns it
        {
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\works_config.json";
            List<Model_PRE>? obj_list = new List<Model_PRE>(); //Create the list named obj_list to hold the return list
            List<Model_PRE>? values = new List<Model_PRE>(); //Create the list named values to hold deserialized data

            if (File.Exists(fileName))
            {
                string fileContent = File.ReadAllText(fileName); //Bring content of filename in filecontent
                values = JsonConvert.DeserializeObject<List<Model_PRE>>(fileContent); //Deserialialize the data in JSON form

                foreach (Model_PRE obj in values) //Loop throught every objects in the deserialized data
                {
                    if (!AllBool && obj.Name == Name) //If we find the work we are looking for in a single work execution
                    {
                        obj_list.Add(obj); //Store the object into a list for return
                        return (obj_list);
                    }
                    else if (AllBool) //If we want to execute all works
                    {
                        obj_list.Add(obj);
                    }
                }
            }
            return (obj_list); //Return empty list if the work is not found
        }

        public void Delete_Work(string Name) //Delete Work from work_conf
        {
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\works_config.json";
            string fileContent = File.ReadAllText(fileName); //Bring content of filename in filecontent
            bool found_bool = false; //Boolean to check if save exists
            List<Model_PRE>? values = new List<Model_PRE>(); //Create the list named values to hold deserialized data
            List<Model_PRE>? obj_list = new List<Model_PRE>(); //Create the list named obj_list to hold the return list

            if (File.Exists(fileName))
            {
                values = JsonConvert.DeserializeObject<List<Model_PRE>>(fileContent); //Deserialialize the data in JSON form

                foreach (Model_PRE obj in values) //Loop throught every objects in the deserialized data
                {
                    if (obj.Name != Name) //If we find the work we are looking for in a single work execution
                    {
                        obj_list.Add(obj);
                    }
                    else { found_bool = true; }
                }
                if (found_bool)
                {
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(obj_list, Newtonsoft.Json.Formatting.Indented); //Serialialize the data in JSON form
                    File.WriteAllText(fileName, jsonString); //Write json file
                }
            }
        }
    }
}