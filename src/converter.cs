using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using utils;
using wevtutilCS;


namespace converter
{
    class test
    {
        static public void tset(ref string kakikae){
            kakikae = "changeTest";
        }
    }

    class EvtxToJsonConverter
    {
        public static void ConvertEvtxToJson(string evtxFilePath)
        {
            /*
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                Console.WriteLine("[ERROR]>> You must have administrative privileges to run this application.");
                return;
            }
            */

            EventLogQuery query = new EventLogQuery(evtxFilePath, PathType.FilePath);

            try
            {
                // Create EventLogReader
                using (EventLogReader reader = new EventLogReader(query))
                {
                    string jsonFilePath = Path.Combine(Path.GetDirectoryName(evtxFilePath), $"{Path.GetFileNameWithoutExtension(evtxFilePath)}-{DateTime.Now:yyyyMMdd_HHmmss}.json");
                    // var jsonFilePath = Path.ChangeExtension(evtxFilePath, "json");
                    // InitializeFile(jsonFilePath);
                    Encoding enc = Encoding.GetEncoding("UTF-8");
                    /*
                    for (EventRecord eventRecord = reader.ReadEvent(); eventRecord != null; eventRecord = reader.ReadEvent())
                    {
                        Console.WriteLine($"Event ID: {eventRecord.Id}");
                        Console.WriteLine($"Timestamp: {eventRecord.TimeCreated}");
                        Console.WriteLine($"Level: {eventRecord}");

                        string test = eventRecord.ToXml();
                        string jsonTemp = JsonConvert.SerializeObject(eventRecord);
                        using (StreamWriter writer = new StreamWriter(jsonFilePath, true, enc)){
                            writer.WriteLine(jsonTemp);
                        }
                        Console.WriteLine("--------------------------------------------------------------------");

                        try
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(test);
                            JsonSerializerSettings settings = new JsonSerializerSettings
                            {
                                Converters = new List<JsonConverter> { new XmlNodeConverter() }
                            };
                            string json = JsonConvert.SerializeXmlNode(xmlDoc.DocumentElement, Newtonsoft.Json.Formatting.None, true);
                            // Console.WriteLine(json);
                            Encoding enc = Encoding.GetEncoding("UTF-8");
                            using (StreamWriter writer = new StreamWriter(jsonFilePath, true, enc))
                            {
                                writer.WriteLine(json);
                            }
                            Console.WriteLine("--------------------------------------------------------------------");
                        }
                        catch (EventLogNotFoundException e)
                        {
                            Console.WriteLine("Event log not found: {0}", e.Message);
                            continue;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error reading the event log: {0}", e.Message);
                            if (e.Message.Contains("is an invalid character."))
                            {
                                string pattern = @"[\x00-\x08\x0B\x0C]";
                                string escapedXmlString = Regex.Replace(test, pattern, "");
                                // string escapedXmlString = SecurityElement.Escape(test);
                                Console.WriteLine(escapedXmlString);
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(escapedXmlString);
                                JsonSerializerSettings settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new XmlNodeConverter() }
                                };
                                string json = JsonConvert.SerializeXmlNode(xmlDoc.DocumentElement, Newtonsoft.Json.Formatting.None, true);
                                // Console.WriteLine(json);
                                Encoding enc = Encoding.GetEncoding("UTF-8");
                                using (StreamWriter writer = new StreamWriter(jsonFilePath, true, enc))
                                {
                                    writer.WriteLine(json);
                                }
                                Console.WriteLine("--------------------------------------------------------------------");
                            }
                        }
                    }
                    */

                    if(GlobalSetuper.isFormat)
                    {
                        utils.ConsoleWriter.WriteLineWithColor($"[D] format-on",ConsoleColor.Yellow);
                        using (StreamWriter writer = new StreamWriter(jsonFilePath, false, enc))
                        {
                            writer.WriteLine("[");
                            int i = 0;

                            for (EventRecord eventRecord = reader.ReadEvent(); eventRecord != null; eventRecord = reader.ReadEvent(),i++)
                            {
                                // Console.WriteLine($"Event ID: {eventRecord.Id}");
                                // Console.WriteLine($"Timestamp: {eventRecord.TimeCreated}");
                                // Console.WriteLine($"Record ID: {eventRecord.EventRecordID}");

                                // Record -> XML
                                // string test = eventRecord.ToXml();
                                string escapedXmlString = Regex.Replace(eventRecord.ToXml(), @"[\x00-\x08\x0B\x0C]", "");
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(escapedXmlString);
                                // XML -> JSON
                                JsonSerializerSettings settings = new JsonSerializerSettings
                                {
                                    Converters = new List<JsonConverter> { new XmlNodeConverter() }
                                };
                                string json = JsonConvert.SerializeXmlNode(xmlDoc, GlobalSetuper.indentSetting, true);

                                JObject jsonObject = JObject.Parse(json);
                                JToken eventData = jsonObject["EventData"]; // ?? jsonObject["UserData"] //Maybe Bug

                                if (eventData != null && eventData.HasValues)
                                {
                                    JArray data = null;
                                    if (eventData is JObject eventDataObject)
                                    {
                                        data = eventDataObject["Data"] as JArray;
                                    }
                                    else if (eventData is JValue eventDataValue && eventDataValue.Value is JObject eventDataValueObject)
                                    {
                                        data = eventDataValueObject["Data"] as JArray;
                                    }

                                    if (data != null)
                                    {
                                        JObject newEventData = new JObject();
                                        foreach (JToken item in data)
                                        {
                                            string name = item["@Name"]?.ToString();
                                            string value = item["#text"]?.ToString();
                                            if (name != null && value != null)
                                            {
                                                newEventData.Add(name, value);
                                            }
                                        }
                                        eventData.Replace(newEventData);
                                        if(i > 0)
                                        {
                                            writer.WriteLine(",");
                                        }
                                        writer.Write(jsonObject.ToString(GlobalSetuper.indentSetting));
                                        // Console.WriteLine(jsonObject.ToString());

                                        // writer.WriteLine(jsonObject.ToString()+",");
                                    }
                                }
                                else
                                {
                                    if(i > 0)
                                    {
                                        writer.WriteLine(",");
                                    }
                                    writer.Write(json.ToString());
                                    // writer.WriteLine(json.ToString()+",");
                                }
                                
                                // Console.WriteLine("--------------------------------------------------------------------");
                            }
                            writer.WriteLine("]");
                        }
                    }
                    else
                    {
                        utils.ConsoleWriter.WriteLineWithColor($"[D] format-off",ConsoleColor.Yellow);
                        using (StreamWriter writer = new StreamWriter(jsonFilePath, false, enc))
                        {
                            writer.WriteLine("[");
                            int i = 0;

                            for (EventRecord eventRecord = reader.ReadEvent(); eventRecord != null; eventRecord = reader.ReadEvent(),i++)
                            {

                                
                                // Record -> XML
                                string escapedXmlString = Regex.Replace(eventRecord.ToXml(), @"[\x00-\x08\x0B\x0C]", "");
                                // string escapedXmlString = RemoveInvalidXmlCharacters(eventRecord.ToXml());
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.LoadXml(escapedXmlString);
                                // XML -> JSON
                                string json = JsonConvert.SerializeXmlNode(xmlDoc, GlobalSetuper.indentSetting, true);
                                if(i > 0)
                                {
                                    writer.WriteLine(",");
                                }
                                writer.Write(json);
                               

                                /*
                                if(i > 0)
                                {
                                    writer.WriteLine(",");
                                }
                                string json = JsonConvert.SerializeObject(eventRecord,GlobalSetuper.indentSetting);
                                Console.WriteLine(json);
                                writer.Write(json.ToString());
                                */

                            }
                            writer.WriteLine("]");
                        }
                    }

                    Console.WriteLine($"Done! >> {jsonFilePath}");
                }
            }
            catch (EventLogNotFoundException e)
            {
                Console.WriteLine("Event log not found2: {0}", e.Message);
            }
            catch (Exception e)
            {
                // Console.WriteLine();
                utils.ConsoleWriter.WriteLineWithColor($"Error reading the event log: {e.ToString()} {e.Message}", ConsoleColor.Red);
            }
        }

        static JObject ConvertXmlNodeToJsonObject(JToken dataToken)
        {
            JObject newDataObject = new JObject();
            foreach (JProperty property in ((JObject)dataToken).Properties())
            {
                newDataObject.Add(property.Name, property.Value["@Name"]);
                newDataObject.Add(property.Value["@Name"].ToString(), property.Value["#text"]);
            }
            return newDataObject;
        }

        static JArray ConvertXmlNodeToJsonArray(JToken dataToken)
        {
            JArray newDataArray = new JArray();
            foreach (JObject dataObject in dataToken.Children<JObject>())
            {
                JObject newDataObject = new JObject();
                foreach (JProperty property in dataObject.Properties())
                {
                    newDataObject.Add(property.Name, property.Value["@Name"]);
                    newDataObject.Add(property.Value["@Name"].ToString(), property.Value["#text"]);
                }
                newDataArray.Add(newDataObject);
            }
            return newDataArray;
        }

        /*
        private static string RemoveInvalidXmlCharacters(string text)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (IsValidXmlChar(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static bool IsValidXmlChar(int character)
        {
            return character == 0x9 || character == 0xA || character == 0xD ||
                (character >= 0x20 && character <= 0xD7FF) ||
                (character >= 0xE000 && character <= 0xFFFD) ||
                (character >= 0x10000 && character <= 0x10FFFF);
        }
        */
    }




    class EvtxToXMLConverter{
        public static Task ConvertEvtxToXml(string evtxFilePath){
            string xmlFilePath = @Path.GetDirectoryName(evtxFilePath) + @"\" + Path.GetFileNameWithoutExtension(evtxFilePath) + "-" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";
            // byte[] evtxFileBytes = File.ReadAllBytes(evtxFilePath);
            // long evtxFileLength = evtxFileBytes.Length;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "wevtutil.exe",
                Arguments = $"qe \"{evtxFilePath}\" /lf:true /f:XML /e:root",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                
                // Read Process Output
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();
                process.OutputDataReceived += (sender, e) => {
                    if (!string.IsNullOrEmpty(e.Data)) {
                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) => {
                    if (!string.IsNullOrEmpty(e.Data)) {
                        error.AppendLine(e.Data);
                    }
                };
                
                process.Start();
                
                // wait process Exit
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                // File.WriteAllText(xmlFilePath, xmlContent);
                string escapedXmlString = Regex.Replace(output.ToString(), @"[\x00-\x08\x0B\x0C]", "");// Remove strings that cannot be used in XML
                
                
                var task = FileWriter.WriteToFileAsync(xmlFilePath, escapedXmlString);
                _ = task.ContinueWith(completedTask =>
                {
                    if (completedTask.Status == TaskStatus.RanToCompletion)
                    {
                        Console.WriteLine($"Done! >> {xmlFilePath}");
                    }
                });
                return task;
            }

        }

        public static void ConvertEvtxToXml_LogReader(string evtxFilePath){
            EventLogQuery query = new EventLogQuery(evtxFilePath, PathType.FilePath);

            try
            {
                using (EventLogReader reader = new EventLogReader(query))
                {
                    // var xmlFilePath = Path.ChangeExtension(evtxFilePath, "xml");
                    // string xmlFilePath = @Path.GetDirectoryName(evtxFilePath) + @"\" + Path.GetFileNameWithoutExtension(evtxFilePath) + "-" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xml";
                    string xmlFilePath = Path.Combine(Path.GetDirectoryName(evtxFilePath), $"{Path.GetFileNameWithoutExtension(evtxFilePath)}-{DateTime.Now:yyyyMMdd_HHmmss}.xml");
                    Encoding enc = Encoding.GetEncoding("UTF-8");


                    using (StreamWriter writer = new StreamWriter(xmlFilePath, false, enc))
                    {
                        writer.WriteLine("<root>");
                        for (EventRecord eventRecord = reader.ReadEvent(); eventRecord != null; eventRecord = reader.ReadEvent())
                        {
                            // Console.WriteLine($"Event ID: {eventRecord.Id}");
                            // Console.WriteLine($"Timestamp: {eventRecord.TimeCreated}");
                            // Console.WriteLine($"Level: {eventRecord}");

                            // Record -> XML
                            string escapedString = Regex.Replace(eventRecord.ToXml(), @"[\x00-\x08\x0B\x0C]", "");
                            // XmlDocument xmlDoc = new XmlDocument();
                            // xmlDoc.LoadXml(escapedString);
                            // Remove XML declaration
                            // escapedString = Regex.Replace(escapedString, @"<\?xml.*\?>", "");
                            // xmlDoc.Save(writer);
                            writer.WriteLine($"{escapedString}");

                            // Console.WriteLine("--------------------------------------------------------------------");
                        }
                        writer.WriteLine("</root>");
                    }
                    Console.WriteLine($"Done! >> {xmlFilePath}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading the event log: {0}", e.Message);
            }
        }
    }


    class XmlToJsonConverter
    {
        public static int ConvertXmlToJson(string xmlFilePath)
        {
            string jsonFilePath = Path.ChangeExtension(xmlFilePath, "json");
            var eventsList = new List<Dictionary<string, object>>();

            using (XmlReader reader = XmlReader.Create(xmlFilePath, GlobalSetuper.XmlReaderSettings))
            {
                bool isEventDataNode = false;
                bool isUserDataNode = false;
                bool isSystemNode = false;
                var systemDict = new Dictionary<string, object>();
                var dataDict = new Dictionary<string, object>();
                string data = "";

                try
                {
                    reader.Read();
                    if(reader.NodeType == XmlNodeType.Element)
                    {
                        string rootName = reader.LocalName;
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    string localName = reader.Name;
                                    if (isSystemNode)
                                    {
                                        var dict = new Dictionary<string, string>();
                                        if (reader.HasAttributes)
                                        {
                                            while (reader.MoveToNextAttribute())
                                            {
                                                var name = reader.Name;
                                                string value = Regex.Replace(reader.Value, @"[\x00-\x08\x0B\x0C]", "");
                                                dict[name] = value;
                                            }
                                            systemDict[localName] = dict;
                                        }
                                        else
                                        {
                                            string value = Regex.Replace(reader.ReadString(), @"[\x00-\x08\x0B\x0C]", "");
                                            systemDict[localName] = value;
                                        }
                                    }
                                    else if (isEventDataNode || isUserDataNode)
                                    {
                                        if (reader.HasAttributes)
                                        {
                                            while (reader.MoveToNextAttribute())
                                            {
                                                string name = reader.Value;
                                                string value = Regex.Replace(reader.ReadString(), @"[\x00-\x08\x0B\x0C]", "");
                                                dataDict[name] = value;
                                            }
                                        }
                                        else
                                        {
                                            string name = reader.Name;
                                            string value = Regex.Replace(reader.ReadString(), @"[\x00-\x08\x0B\x0C]", "");
                                            dataDict[name] = value;
                                        }
                                    }
                                    else if (reader.Name == "System")
                                    {
                                        isSystemNode = true;
                                        systemDict = new Dictionary<string, object>();
                                    }
                                    else if (reader.Name == "EventData" || reader.Name == "UserData")
                                    {
                                        if (reader.Name == "EventData")
                                        {
                                            isEventDataNode = true;
                                        }
                                        else
                                        {
                                            isUserDataNode = true;
                                        }
                                        dataDict = new Dictionary<string, object>();
                                    }
                                    else if (reader.Name == "Event")
                                    {
                                        //
                                    }
                                    break;
                                case XmlNodeType.EndElement:
                                    if (reader.Name == rootName)
                                    {
                                        Console.WriteLine("End Element");
                                    }
                                    else if (reader.Name == "System")
                                    {
                                        isSystemNode = false;
                                    }
                                    else if (reader.Name == "EventData")
                                    {
                                        isEventDataNode = false;
                                        data = reader.Name;
                                    }
                                    else if (reader.Name == "UserData")
                                    {
                                        isUserDataNode = false;
                                        data = reader.Name;
                                    }
                                    else if (reader.Name == "Event")
                                    {
                                        var dictEvent = new Dictionary<string, object>();
                                        dictEvent["System"] = systemDict;
                                        dictEvent[data] = dataDict;
                                        eventsList.Add(dictEvent);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                finally
                {
                    var json = JsonConvert.SerializeObject(eventsList, GlobalSetuper.indentSetting);
                    File.WriteAllText(jsonFilePath, json);
                    Console.WriteLine($"Done! >> {jsonFilePath}");
                }

                return 0;
            }
        }
    }
}