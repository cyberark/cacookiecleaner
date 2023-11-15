using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

class CACookieCleaner
{
    class HarFile
    {
        /* the value you see at the edited HAR file */
        const string HIDE_COOKIE_VALUE = "*********";

        string outPutFile;
        FileInfo inputFile;
        JObject data;
        JToken entries;
        List<string> headerFilters;
        Regex regex;

        public HarFile(FileInfo inputFile, string[] pattern)
        {
            this.inputFile = inputFile;
            var fileContent = GetFileContent();
            data = JObject.Parse(fileContent);
            entries = data["log"]["entries"];
            outPutFile = Path.Combine(Environment.CurrentDirectory, $"output_{DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss")}.har");
            headerFilters = new List<string>() { "Cookie", "Set-Cookie" };
            regex = new Regex(handleInputRegex(pattern), RegexOptions.IgnoreCase);
        
        }
        public void ProcessFile()
        {
            foreach (var entry in entries)
            {
                if (entry["request"] != null)
                {
                    modifyEntryHeader(entry["request"]);
                    modifyEntryCookies(entry["request"]);
                    
                    if (entry["request"]["method"].ToString() == "POST")
                    {
                        if(entry["request"]["postData"] != null)
                        {
                            if (entry["request"]["postData"]["text"] != null)
                            {
                                handlePostDataEntry(entry["request"]["postData"]);
                            }
                        }

                    }
                }
                if (entry["response"] != null)
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

                    if (isFilterFound(name))
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
        private string handleInputRegex(string[] pattern)
        {
            var defaultPatterns = new List<string> { "session", "token", "pass", "id" };
            if(pattern.Length > 0)
            {
                for (var index = 0; index < pattern.Length; index++)
                {
                    pattern[index] = pattern[index].Trim().ToLower();
                    if(pattern[index] == "." || pattern[index] == "")
                    {
                        continue;
                    }
                    if(pattern[index] == "*")
                    {
                        return @".*";
                    }
                    if (!defaultPatterns.Contains(pattern[index]))
                    {
                        defaultPatterns.Add(pattern[index]);
                    }
                }
            }

            for(var index = 0; index < defaultPatterns.Count; index++)
            {
                defaultPatterns[index] = defaultPatterns[index].Trim();
                defaultPatterns[index] = $".*{defaultPatterns[index]}.*";
            }
            return $"{string.Join("|", defaultPatterns)}";
        }

        private void handlePostDataEntry(JToken entry)
        {
            if (entry["text"] is JValue)
            {
                if (entry["text"].Type == JTokenType.String)
                {
                    search_and_modify(entry);

                }
            }
        }

        private void search_and_modify(JToken postData)
        {
            if (postData["text"] is JValue && postData["text"].Type == JTokenType.String)
            {
                
                var jsonString = postData["text"].Value<string>();
                var jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
                List<string> keysToModify = new List<string>();
                foreach (var item in jsonDict)
                {
                    if (item.Value != null)
                    {
                        if (item.Value is JArray)
                        {
                            JArray jsonArray = (JArray)item.Value;

                            foreach (JToken arrayItem in jsonArray)
                            {
                                if (arrayItem is JObject arrayObject)
                                {
                                    foreach (var property in arrayObject.Properties())
                                    {
                                        var propertyKey = property.Name;
                                        if (matchCustomerRegexInput(property.Name))
                                        {
                                            arrayItem[propertyKey] = HIDE_COOKIE_VALUE;
                                        }
                                    }
                                }
                                else if(arrayItem is JValue arrayJValue)
                                {
                                    if (matchCustomerRegexInput(arrayJValue.Value.ToString()))
                                    {
                                        changeValue(arrayJValue,HIDE_COOKIE_VALUE);
                                    }
                                }


                            }

                        }
                        else if (item.Value is string)
                        {
                            if (matchCustomerRegexInput(item.Key))
                            {
                                keysToModify.Add(item.Key);                            }
                        }
                    }
                }

                foreach(var key in keysToModify)
                {
                    jsonDict[key] = HIDE_COOKIE_VALUE;
                }

                if (keysToModify.Count > 0)
                {
                    var jsonObject = JsonConvert.SerializeObject(jsonDict);
                    postData["text"] = jsonObject;
                }
            }

        }

        private bool matchCustomerRegexInput(string property)
        {
            MatchCollection matches = regex.Matches(property);
            foreach (Match match in matches)
            {
                return true;
            }

            return false;
        }
    }

    static bool InputValidation(string filepath)
    {
        if (!filepath.Contains("..") && !filepath.Contains("/"))
        {
            if (File.Exists(filepath))
            {
                return Path.GetExtension(filepath).Equals(".har", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        return false;
    }

    static void Main(string[] args)
    {
        try
        {
            Console.Write("Enter the full file path directory (.HAR) and pattern for serach\n");
            var filepath = Console.ReadLine();
            Console.Write("Enter the properties you want to mask, not mandatory. you can insert more than one and saperate it with ','\n");
            var inputRegex = Console.ReadLine().Split(',');

            if (!InputValidation(filepath))
            {
                System.Environment.Exit(2);
            }

            var inputFile = new FileInfo(filepath);
            var harFile = new HarFile(inputFile, inputRegex);
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


