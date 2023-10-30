using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

class CACookieCleaner
{
    class HarFile
    {
        /* the value you see at the edited HAR file */
        const string HIDE_COOKIE_VALUE = "*********";
        /* recommended to use a constant path to prevent Path Traversal attack, this command validate the current directory */
        const string ALLOWED_DIRECTORY = "C:\\CACookieCleaner\\";
        
        string outPutFile;
        FileInfo inputFile;
        JObject data;
        JToken entries;
        List<string> headerFilters; 

        public HarFile(FileInfo inputFile)
        {
            this.inputFile = inputFile;
            var fileContent = GetFileContent();
            data = JObject.Parse(fileContent);
            entries = data["log"]["entries"];
            outPutFile = Path.Combine(Environment.CurrentDirectory, $"output_{DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss")}.har");
            headerFilters = new List<string>() { "Cookie", "Set-Cookie" };
        } 
        public void ProcessFile()
        {
            foreach (var entry in entries)
            {
                if(entry["request"] != null)
                {
                    modifyEntryHeader(entry["request"]);
                    modifyEntryCookies(entry["request"]);
                }
                if(entry["response"] != null)
                {
                    modifyEntryHeader(entry["response"]);
                    modifyEntryCookies(entry["response"]);
                }    
            }
        }
        private void modifyEntryHeader(JToken entry)
        {
            
            var headers = entry["headers"];
            
            if (headers != null)
            {
                var updatedHeaders = new JArray();
                
                foreach (var header in headers)
                {
                    var name = header["name"].ToString();

                    if(isFilterFound(name))
                    {
                        editValueProperty(header);
                        updatedHeaders.Add(header);
                    }
                    else
                    {
                        updatedHeaders.Add(header);
                    }              
                }
                
                entry["headers"] = updatedHeaders;
            }
        } 
        private void modifyEntryCookies(JToken entry)
        {
            JArray entrytCookies = entry["cookies"] as JArray;
            List<string> excludedProperties = new List<string>() { "username" };
            
            if (entrytCookies != null)
            {
                foreach (var ignore in excludedProperties)
                {
                    foreach (var cookie in entrytCookies)
                    {
                        if (!cookie["name"].ToString().ToLower().Contains(ignore))
                        {
                            changeValue(cookie["value"], HIDE_COOKIE_VALUE);
                        }

                    }
                }

            }
        }
        private bool isFilterFound(string fieldsName)
        {        
            foreach (var filter in headerFilters)
            {
                if (fieldsName != null && (fieldsName.Contains(filter) || fieldsName == filter || fieldsName.ToLower() == filter.ToLower()))
                {
                    return true;
                }
            }
            
            return false;
        }
        private void editValueProperty(JToken header)
        {
           /*
            * If you want to exclude some properties please insert here, 
            * in this example user is excluded username
            */

            List<string> excludedProperties = new List<string>() { "username" };

            var cookies = header["value"].ToString().Split(';');

            for (var cookieIndex = 0; cookieIndex < cookies.Length; cookieIndex++)
            {
                foreach (var ignore in excludedProperties)
                {
                    if (!cookies[cookieIndex].ToLower().Contains(ignore))
                    {
                        if (cookies[cookieIndex].Contains("="))
                        {
                            var cookieName = cookies[cookieIndex].Split('=')[0];
                            cookies[cookieIndex] = $"{cookieName}={HIDE_COOKIE_VALUE}";
                        }
                        else
                        {
                            cookies[cookieIndex] = HIDE_COOKIE_VALUE;
                        }
                    }

                }

            }
            if (header["value"] is JValue)
            {
                string cuurentValue = string.Join(";", cookies);
                changeValue(header["value"], cuurentValue);
            }
        }
        private void changeValue(JToken original, string current)
        {
            JValue jValuePtr = (JValue)original;
            jValuePtr.Value = current;
        }    
        public void GenerateOutputFile()
        {
            File.WriteAllText(outPutFile, data.ToString());
        }
        private string GetFileContent()
        {
            using (FileStream fs = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                    return sr.ReadToEnd();
            }
        }

        public static string GetAllowedDir()
        {
            return ALLOWED_DIRECTORY;
        }

    }

    static bool InputValidation(string FileName, string AllowedDir)
    {
        //in order to avoid Path Traversal attack
        var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
        if (AllowedDir.Equals(baseFolder, StringComparison.InvariantCultureIgnoreCase))
        {
            if (!FileName.Contains("..") && !FileName.Contains("/"))
            {
                var filedir = Path.Combine(baseFolder, FileName);

                if (File.Exists(filedir))
                {
                    return Path.GetExtension(filedir).Equals(".har", StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }
      
        return false;
    }

    static void Main(string[] args)
    {
        try
        {     
            Console.Write("Enter the file name (.HAR) from the current directory: ");
            var fileName = Console.ReadLine();

            if (!InputValidation(fileName,HarFile.GetAllowedDir()))
            {
                System.Environment.Exit(2);
            }
            
            var inputFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
            var harFile = new HarFile(inputFile);
            harFile.ProcessFile();
            harFile.GenerateOutputFile();

            Console.WriteLine("New Har created successfully, Press enter any key to exit");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed, Reason: " + ex.Message);
        }

    }
       
}

