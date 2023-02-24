using EasySave_G8_UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasySave_G8_UI.View_Models
{
    public class View_Model
    {
        //VM Startup Initialisation
        public void VM_Init() 
        {
            Model_COMMON ModelCOMMON = new Model_COMMON("en");
            ModelCOMMON.Init(ModelCOMMON);
        }

        //VM Classic Save
        public void VM_Classic(string Name, string Source, string Destination, bool Type, object sender)
        {
            Model_PRE ModelPRE = new Model_PRE(Name, Source, Destination, Type);
            ModelPRE.Exec(sender);
        }

        //VM Check if a Work exists
        public bool VM_Work_Exist(string Name)
        {
            string fileName = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\EasySave\works_config.json";
            if (File.Exists(fileName))
            {
                Model_Works ModelWorks = new Model_Works();
                List<Model_PRE>? obj_list = (ModelWorks.Get_Work(Name, false));
                if (!obj_list.Any()) { return false; } //Check if there is content in the list, if not that means not object with the given name was found
                else { return true; }
            }
            return false;
        }

        //VM Returns a list of Work objects
        public List<Model_PRE> VM_Work_Show(string? Name, bool AllWorks)
        {
            Model_Works ModelWorks = new Model_Works();
            List<Model_PRE>? obj_list = (ModelWorks.Get_Work(Name, AllWorks));
            return (obj_list);
        } 

        //VM Create a new work
        public void VM_Work_New(string Name, string Source, string Destination, bool Type)
        {
            Model_PRE ModelPRE = new Model_PRE(Name, Source, Destination, Type);
            ModelPRE.Save();
        }

        //VM Run a single or all works
        public void VM_Work_Run(string Name, object sender)
        {
            Model_Works ModelWorks = new Model_Works();
            List<Model_PRE>? obj_list = (ModelWorks.Get_Work(Name, false)); //Use Get_Work to get Work data
            foreach (Model_PRE obj in obj_list) //Loop throught every works in list and execute them
            {
                Model_PRE ModelPRE = new Model_PRE(obj.Name, obj.Source, obj.Destination, obj.Type);
                ModelPRE.Exec(sender);
            }
        }

        //VM Delete a Work
        public void VM_Work_Delete(string Name)
        {
            Model_Works ModelWorks = new Model_Works();
            ModelWorks.Delete_Work(Name);
        }

        //VM Change app_config Language
        public void VM_Change_Language(string lang)
        {
            Model_LANG ModelLANG = new Model_LANG(lang);
            ModelLANG.ChangeLanguage();
        }

        //VM Get app_config Language
        public void VM_Update_Language()
        {
            Model_LANG ModelLANG = new Model_LANG(null);
            ModelLANG.UpdateLanguage();
        }

        //VM Translate string from resx files
        public static string VM_GetString_Language(string trsl_string)
        {
            string rtrn_string = Model_LANG.GetString(trsl_string);
            return rtrn_string;
        }

        //MV Show the daily log files content
        public List<Model_AFT> MV_Look_Logs(string Date)
        {
            Model_Logs ModelLogs = new Model_Logs();
            List<Model_AFT> list = ModelLogs.Get_Logs(Date);
            return list;
        }

        // This method returns a list of Model_StateLogs by calling the Get_StateLogs method of Model_Logs class
        public List<Model_StateLogs> MV_Look_StateLogs()
        {
            // Create an instance of Model_Logs class
            Model_Logs ModelLogs = new Model_Logs();
            // Call the Get_StateLogs method of Model_Logs class to retrieve a list of Model_StateLogs
            List<Model_StateLogs> list = ModelLogs.Get_StateLogs();
            // Return the retrieved list of Model_StateLogs
            return list;
        }

        public bool VM_BlackListTest()
        {
            Model_BLACKLIST ModelBLACKLIST = new Model_BLACKLIST(); // create a new instance of the Model_BLACKLIST class
            return ModelBLACKLIST.BlacklistTest(); // call the BlacklistTest method from the Model_BLACKLIST class and return its result
        }

        // This function adds a process name to the blacklist
        public void VM_BlackListAdd(string ProcessName)
        {
            Model_BLACKLIST ModelBLACKLIST = new Model_BLACKLIST();
            ModelBLACKLIST.BlacklistAdd(ProcessName);
        }

        public void VM_BlackListRemove(string ProcessNameRm)
        {
            Model_BLACKLIST ModelBLACKLIST = new Model_BLACKLIST();
            ModelBLACKLIST.BlacklistRemove(ProcessNameRm);
        }

        public List<string> MV_Blacklist()
        {
            Model_BLACKLIST ModelBLACKLIST = new Model_BLACKLIST();
            return ModelBLACKLIST.BlacklistReturn();
        }

        public bool VM_StateLogsExists(string Name)
        {
            Model_Logs ModelLOGS = new Model_Logs();
            return ModelLOGS.StatelogExists(Name);
        }

        public void VM_PriorityListAdd(string ProcessName,int Index)
        {
            Model_PRIORITY ModelPRIORITY = new Model_PRIORITY();
            ModelPRIORITY.priorityAdd(ProcessName,Index);
        }

        public void VM_PriorityListRemove(string ProcessNameRm)
        {
            Model_PRIORITY ModelPRIORITY = new Model_PRIORITY();
            ModelPRIORITY.priorityRemove(ProcessNameRm);
        }

        public List<string> MV_PriorityListRe()
        {
            Model_PRIORITY ModelPRIORITY = new Model_PRIORITY();
            return ModelPRIORITY.priorityReturn();
        }


        public void VM_ExtensionListAdd(string CSExt)
        {
            Model_EXTENSION ModelEXTENSION = new Model_EXTENSION();
            ModelEXTENSION.ExtensionAdd(CSExt);
        }
        public void Extensionlist_rm_btn_Click2(string CSExt)
        {
            Model_EXTENSION ModelEXTENSION = new Model_EXTENSION();
            ModelEXTENSION.ExtensionRemove(CSExt);
        }
        public List<string> MV_ExtensionListRe()
        {
            Model_EXTENSION ModelEXTENSION = new Model_EXTENSION();
            return ModelEXTENSION.ExtensionReturn();
        }

        public double MV_NbKoReturn()
        {
            Model_NBKO modelNbko = new Model_NBKO();
            double nbKo = modelNbko.NbKoReturn();
            return nbKo;
        }
        public void VM_NbKoSet(double nbko)
        {
            Model_NBKO modelNbko = new Model_NBKO();
            modelNbko.NbKoSet(nbko);
        }


        //Pauses/continue a specific thread
        public void VM_PauseThreads()
        {
            Model_AFT._semahporeAFTObjects.Wait();
            try
            {
                foreach (Model_AFT obj in Model_AFT.AFTObjects)
                {
                    obj.PauseSpecificThread();
                }
            }
            finally { Model_AFT._semahporeAFTObjects.Release(); }

        }

        //Pauses/continue a specific thread
        public void VM_PauseSpecificThread(string WorkName)
        {
            Model_AFT._semahporeAFTObjects.Wait();
            try
            {
                foreach (Model_AFT obj in Model_AFT.AFTObjects)
                {
                    if (obj.Name == WorkName) { obj.PauseSpecificThread(); }
                }
            }
            finally { Model_AFT._semahporeAFTObjects.Release(); }
        }

        //Stop all threads
        public void VM_StopThreads()
        {
            Model_AFT._semahporeAFTObjects.Wait();
            try
            {
                foreach (Model_AFT obj in Model_AFT.AFTObjects)
                {
                    obj.StopSpecificThread();
                }
            }
            finally { Model_AFT._semahporeAFTObjects.Release(); }
        }

        //Stop a specific thread
        public void VM_StopSpecificThread(string WorkName)
        {
            Model_AFT._semahporeAFTObjects.Wait();
            try
            {
                foreach (Model_AFT obj in Model_AFT.AFTObjects)
                {
                    if (obj.Name == WorkName) { obj.StopSpecificThread(); }
                }
            }
            finally { Model_AFT._semahporeAFTObjects.Release(); }
        }

        //Forces Pause for all works
        public void VM_ForcePause()
        {
            Model_AFT.BlacklistPauseThreads();
        }

        //Get work status
        public bool VM_SaveOngoing()
        {
            return (Model_AFT.AFTObjects.Count != 0);
        }

        public void VM_RemoteLaunch()
        {
            Model_COMMON modelCommon = new Model_COMMON(null);
            modelCommon.LaunchRemoteServ();
        }
    }
}