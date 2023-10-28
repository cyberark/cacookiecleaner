using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

class CACookieCleaner
{
    class HarFile
    {
        string outPutFile;
        FileInfo inputFile;
        JObject data;
        JToken entries;
        List<string> headerFilters; 

        public HarFile(FileInfo inputFile)
        {
            this.inputFile = inputFile;
            var fileContent = File.ReadAllText(inputFile.FullName);
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
                    modifyEntryCookies(entry["request"]);
                    modifyEntryHeader(entry["request"]);
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
                            changeValue(cookie["value"], "*********");
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
                            cookies[cookieIndex] = $"{cookieName}=*********";
                        }
                        else
                        {
                            cookies[cookieIndex] = "*********";
                        }
                    }

                }

            }
            if (header["value"] is JValue)
            {
                string cuurentValue = string.Join(";", cookies);
                changeValue(header["value"], cuurentValue);
                //JValue jValuePtr = (JValue)header["value"];
                //jValuePtr.Value = result;
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
        
    }

    static bool InputValidation(FileInfo FilePath)
    {
        if (FilePath.Exists)
        {
            return FilePath.Extension.Equals(".har", StringComparison.InvariantCultureIgnoreCase);
        }       
        return false;
    }

    static void Main(string[] args)
    {
        try
        {
            Console.Write("Enter the full path to the input HAR file: ");
            var filePath = Console.ReadLine();
            
            var inputFile = new FileInfo(filePath);

            if (!InputValidation(inputFile))
            {
                System.Environment.Exit(2);
            }

            var harFile = new HarFile(inputFile);
            harFile.ProcessFile();
            harFile.GenerateOutputFile();

            Console.WriteLine("Success, Press enter any key to exit");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed, Reason: " + ex.Message);
        }
    }

}
