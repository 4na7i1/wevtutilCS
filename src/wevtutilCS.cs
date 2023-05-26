#define DEBUG

using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
// using System.IO;
// using System.Linq;
using System.CommandLine;
// using System.Managmenet;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.Security.Principal;
using System.Text;
using System.Security;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using utils;
using converter;

namespace wevtutilCS
{
    class Program
    {
        public static bool isDebug = false;
        public static Stopwatch sw = new Stopwatch();
        public static List<Task> tasks = new List<Task>();
        static void Main(string[] args)
        {
            var pathOption = new Option<string>(
                "--path",
                "Specify path to evtx,xml file"
            );
            var jsonOption = new Option<bool>(
                "-j",
                "Export JSON"
            );
            var xmlOption = new Option<bool>(
                "-x",
                "Export XML"
            );
            var debugOption = new Option<bool>(
                "-d",
                getDefaultValue: () => false,
                "Debug Mode"
            );
            var indentOption = new Option<bool>(
                "-i",
                getDefaultValue: () => false,
                "indent"
            );
            var formatOption = new Option<bool>(
                "-f",
                getDefaultValue: () => false,
                "Json format"
            );
            
            var rootCommand = new RootCommand
            {
                pathOption,
                jsonOption,
                xmlOption,
                debugOption,
                indentOption,
                formatOption
            };

            rootCommand.SetHandler((pathOption, jsonOption, xmlOption, debugOption, indentOption, formatOption) =>
            {

                if (!File.Exists(pathOption))
                {
                    Console.WriteLine($"The file does not exist.: {pathOption}");
                    return;
                }
                if (debugOption)
                {
                    Console.WriteLine("DEBUG MODE ON.");
                    isDebug = true;
                    sw.Start();
                }
                if (indentOption)
                {
                    GlobalSetuper.indentSetting = Newtonsoft.Json.Formatting.Indented;
                }
                if (formatOption)
                {
                    GlobalSetuper.isFormat = true;
                }

                if (jsonOption)
                {
                    // var jsonPath = Path.ChangeExtension(pathOption, "json");

                    if (string.Compare(Path.GetExtension(pathOption), ".xml", true) == 0)
                    {
                        Console.WriteLine($"XML: {pathOption} => json ");
                        
                        // int rtn = ConvertXmlToJson(pathOption);
                        int rtn = XmlToJsonConverter.ConvertXmlToJson(pathOption);
                        if (rtn == 0)
                        {
                            Console.WriteLine("Successful.");
                        }
                        else
                        {
                            Console.WriteLine("[Error]>> ConvertXmlToJson()");
                        }
                    }
                    else if (string.Compare(Path.GetExtension(pathOption), ".evtx", true) == 0)
                    {
                        Console.WriteLine($"EVTX: {pathOption} => json ");
                        EvtxToJsonConverter.ConvertEvtxToJson(pathOption);
                    }
                    else
                    {
                        Console.WriteLine($"Text format is not suitable >> {pathOption} ");
                        return;
                    }
                }

                if (xmlOption)
                {
                    if (string.Compare(Path.GetExtension(pathOption), ".evtx", true) == 0)
                    {
                        Console.WriteLine($"Evtx: {pathOption} => XML");
                        // ConvertEvtxToXml(pathOption);
                        // EvtxToXMLConverter.ConvertEvtxToXml(pathOption);
                        // tasks.Add(EvtxToXMLConverter.ConvertEvtxToXml(pathOption));
                        EvtxToXMLConverter.ConvertEvtxToXml_LogReader(pathOption);
                        // testEvtxBinary(pathOption);
                    }
                    else
                    {
                        Console.WriteLine($"{pathOption} is not evtx-file");
                        return;
                    }
                }
                if (!jsonOption && !xmlOption)
                {//none -j,-x
                    // var content = File.ReadAllText(pathOption);
                    // Console.WriteLine(content);
                    Console.WriteLine("Enter -j or -x as an option (help option => --help)");
                }
            }, pathOption, jsonOption, xmlOption, debugOption, indentOption, formatOption);

            /* Main */
            var platform = OsDetecter.GetOperatingSystem;

            rootCommand.Invoke(args);

            if (isDebug && sw.IsRunning)
            {
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                ConsoleWriter.WriteLineWithColor($"[D]>>(Program) time:{ts}", ConsoleColor.Yellow);
            }

            // tests.evtxFilesList();

            /* exit */
            // Wait for all task Complete
            Console.WriteLine("Wait for all task Complete.");
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Complete.");
        }

        /*
        static int ConvertXmlToJson(string xmlFilePath)
        {

            // string jsonFilePath = @Path.GetDirectoryName(xmlFilePath)+@"\"+Path.GetFileNameWithoutExtension(xmlFilePath)+"-"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".json";
            string jsonFilePath = Path.ChangeExtension(xmlFilePath, "json");

            // XmlReaderSettings settings = new XmlReaderSettings();
            // settings.IgnoreComments = true;
            // settings.CheckCharacters = true;
            // settings.IgnoreProcessingInstructions = true;
            // settings.IgnoreWhitespace = true;
            // settings.ValidationType = ValidationType.None;

            using (XmlReader reader = XmlReader.Create(xmlFilePath, GlobalSetuper.XmlReaderSettings)) // Create the reader.
            {
                bool isEventDataNode = false;
                bool isUserDataNode = false;
                bool isSystemNode = false;
                var eventsList = new List<Dictionary<string, object>>();
                var dataList = new List<Dictionary<string, object>>();
                // var testList = new List<object>();
                var dict = new Dictionary<string, string>();
                // var htEvent = new Dictionary<string, object>();
                var systemDict = new Dictionary<string, object>();
                var DataDict = new Dictionary<string, object>();
                string data = "";

                try
                {
                    while (reader.Read())
                    {
                        string localName = "";
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                // Console.WriteLine($"[D]>> Start Element: {reader.Name}");
                                localName = reader.Name;
                                if (isSystemNode == true)
                                {
                                    dict = new Dictionary<string, string>();
                                    if (reader.HasAttributes)
                                    {
                                        while (reader.MoveToNextAttribute())
                                        {
                                            var name = reader.Name;
                                            // var value = reader.Value;
                                            string value = Regex.Replace(reader.Value, @"[\x00-\x08\x0B\x0C]", "");
                                            dict[name] = value;
                                        }
                                        systemDict[localName] = dict;
                                    }
                                    else
                                    {
                                        var name = reader.Name;
                                        // var value = reader.ReadString();
                                        string value = Regex.Replace(reader.ReadString(), @"[\x00-\x08\x0B\x0C]", "");
                                        systemDict[name] = value;
                                    }
                                }
                                else if (isEventDataNode == true || isUserDataNode == true)
                                {
                                    // var dict = new Dictionary<string, string>();
                                    if (reader.HasAttributes)
                                    {//not use?
                                        // htEvent = new Dictionary<string, object>();
                                        while (reader.MoveToNextAttribute())
                                        {
                                            var name = reader.Value;
                                            // var value = reader.ReadString();
                                            string value = Regex.Replace(reader.ReadString(), @"[\x00-\x08\x0B\x0C]", "");
                                            // dict[name] = value;
                                            DataDict[name] = value;
                                            // dataList.Add(new Dictionary<string, object> { { localName, dict } }); => data{},data{},data{},,,
                                        }
                                        // htEvent[localName] = dict; // need fix
                                        // testList.Add(dict);
                                    }
                                    else
                                    {
                                        var name = reader.Name;
                                        // var value = reader.ReadString();
                                        string value = Regex.Replace(reader.ReadString(), @"[\x00-\x08\x0B\x0C]", "");
                                        DataDict[name] = value;
                                        // if(reader.NodeType == XmlNodeType.Element){
                                        //     reader.Read();
                                        //     var name = reader.Name;
                                        //     if(reader.NodeType == XmlNodeType.Text){
                                        //         var value = reader.Value;
                                        //         DataDict[name] = value;
                                        //     }
                                        //     reader.Read();
                                        // }
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
                                    dict = new Dictionary<string, string>();
                                    dataList = new List<Dictionary<string, object>>();
                                    // testList = new List<object>();
                                    // htEvent = new Dictionary<string, object>();
                                    DataDict = new Dictionary<string, object>();
                                }
                                else if (reader.Name == "Event")
                                {
                                    // reader.
                                }
                                continue;
                            case XmlNodeType.Text:
                                // Console.WriteLine($"[D]>> Element Text: {reader.Value}");
                                continue;
                            case XmlNodeType.EndElement:
                                // Console.WriteLine($"[D]>> End Element: {reader.Name}");                                
                                if (reader.Name == "root")
                                {
                                    Console.WriteLine("DONE!");
                                }
                                else if (reader.Name == "System")
                                {
                                    isSystemNode = false;
                                }
                                else if (reader.Name == "EventData")
                                {
                                    isEventDataNode = false;
                                    data = reader.Name; // Maybe Bug Need Fix

                                    // htEventData[reader.Name] = dataList;
                                }
                                else if (reader.Name == "UserData")
                                {
                                    isUserDataNode = false;
                                    data = reader.Name; // 
                                }
                                else if (reader.Name == "Event")
                                {
                                    // htEvent["System"] = systemDict;
                                    // htEvent["EventData"] = htEventData;
                                    var dictEvent = new Dictionary<string, object>();
                                    dictEvent["System"] = systemDict;
                                    // dict[data] = dataList;
                                    dictEvent[data] = DataDict;
                                    eventsList.Add(dictEvent);
                                }
                                continue;
                            default:
                                break;
                        }
                    }
                }
                finally
                {
                    // string json = JsonConvert.SerializeObject(eventsList);
                    var json = JsonConvert.SerializeObject(eventsList, GlobalSetuper.indentSetting);
                    File.WriteAllText(@jsonFilePath, json);

                    // sw.Stop();
                    // TimeSpan ts = sw.Elapsed;
                    // Console.WriteLine($"[D]>>(ConvertXmlToJson) time:{ts}");
                    Console.WriteLine($"Done! >> {jsonFilePath}");
                }

                return 0;

            }
        }
        */
    }

    class GlobalSetuper
    {
        public static Newtonsoft.Json.Formatting indentSetting = Newtonsoft.Json.Formatting.None;
        public static bool isFormat = false;
        public static XmlReaderSettings XmlReaderSettings { get; } = new XmlReaderSettings
        {
            IgnoreComments = true,
            CheckCharacters = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            ValidationType = ValidationType.None
        };
    }
}
