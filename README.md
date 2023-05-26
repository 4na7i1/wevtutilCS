# `wevtutilCS`

## Features

+ EVTX Conversion
  + EVTX => JSON
  + EVTX => XML
  + (XML => JSON)
+ Performance (MEMO)
  (EVTX) 628 MB => (XML) 619 MB  #time:13[s]
  (EVTX) 628 MB => (JSON) 627 MB  #time:23[s]

## Examples
+ evtx => json
    ```cmd
    ./wevtutilCS --path {evtxFilePath} -j
    ```
+ evtx => xml
    ```cmd
    ./wevtutilCS --path {evtxFilePath} -x
    ```
+ xml => json
    ```cmd
    ./wevtutilCS --path {xmlFilePath} -j
    ```
+ Options
  ```cmd
  --path : .evtx or .xml file path
  -j : convert to json
  -x : convert to xml
  -d : trace message
  -i : indent
  -f : format
  ```
 + -i (indent)
   indent option
 + -f (format)
  Select json key and value format in case of XML -> JSON

    + **with -f**
        ```json
        {
            "System": {
                "Provider": {
                    "Name": "Microsoft-Windows-Sysmon",
                    "Guid": "{5770385f-c22a-43e0-bf4c-06f5698ffbd9}"
                },
                "EventID": "10",
                "Version": "3",
                "Level": "4",
                "Task": "10",
                "Opcode": "0",
                "Keywords": "0x8000000000000000",
                "TimeCreated": {
                    "SystemTime": "2023-05-09T04:45:54.4197950Z"
                },
                "EventRecordID": "1185184",
                "Correlation": "",
                "Execution": {
                    "ProcessID": "8360",
                    "ThreadID": "4856"
                },
                "Channel": "Microsoft-Windows-Sysmon/Operational",
                "Computer": "VMTEST01",
                "Security": {
                    "UserID": "S-1-5-18"
                }
                },
            "EventData": {
                "RuleName": "-",
                "UtcTime": "2023-05-09 04:45:54.404",
                "SourceProcessGUID": "{82296a8a-d002-6459-9024-000000000a00}",
                "SourceProcessId": "9240",
                "SourceThreadId": "7684",
                "SourceImage": "C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\MicrosoftEdgeUpdate.exe",
                "TargetProcessGUID": "{82296a8a-7749-6452-8900-000000000a00}",
                "TargetProcessId": "5940",
                "TargetImage": "C:\\Windows\\system32\\SecurityHealthService.exe",
                "GrantedAccess": "0x1000",
                "CallTrace": "C:\\Windows\\SYSTEM32\\ntdll.dll+9d584|C:\\Windows\\System32\\wow64.dll+10955|C:\\Windows\\System32\\wow64.dll+901a|C:\\Windows\\System32\\wow64cpu.dll+17c3|C:\\Windows\\System32\\wow64cpu.dll+11b9|C:\\Windows\\System32\\wow64.dll+38c9|C:\\Windows\\System32\\wow64.dll+32bd|C:\\Windows\\SYSTEM32\\ntdll.dll+d39e7|C:\\Windows\\SYSTEM32\\ntdll.dll+74deb|C:\\Windows\\SYSTEM32\\ntdll.dll+74c73|C:\\Windows\\SYSTEM32\\ntdll.dll+74c1e|C:\\Windows\\SYSTEM32\\ntdll.dll+72d5c(wow64)|C:\\Windows\\System32\\KERNELBASE.dll+112638(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+17c4c(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+1799c(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+26e63(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+fcbef(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+94fb6(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+34aec(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+33d73(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+3389d(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+333d1(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+745f(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\MicrosoftEdgeUpdate.exe+ab31",
                "SourceUser": "NT AUTHORITY\\SYSTEM",
                "TargetUser": "NT AUTHORITY\\SYSTEM"
            }
        },
        ```

    + **without -f**
      ```json
      {
        "@xmlns": "http://schemas.microsoft.com/win/2004/08/events/event",
        "System": {
            "Provider": {
            "@Name": "Microsoft-Windows-Sysmon",
            "@Guid": "{5770385f-c22a-43e0-bf4c-06f5698ffbd9}"
            },
            "EventID": "10",
            "Version": "3",
            "Level": "4",
            "Task": "10",
            "Opcode": "0",
            "Keywords": "0x8000000000000000",
            "TimeCreated": {
            "@SystemTime": "2023-05-09T04:45:54.4197950Z"
            },
            "EventRecordID": "1185184",
            "Correlation": null,
            "Execution": {
            "@ProcessID": "8360",
            "@ThreadID": "4856"
            },
            "Channel": "Microsoft-Windows-Sysmon/Operational",
            "Computer": "VMTEST01",
            "Security": {
            "@UserID": "S-1-5-18"
            }
        },
        "EventData": {
            "Data": [
            {
                "@Name": "RuleName",
                "#text": "-"
            },
            {
                "@Name": "UtcTime",
                "#text": "2023-05-09 04:45:54.404"
            },
            {
                "@Name": "SourceProcessGUID",
                "#text": "{82296a8a-d002-6459-9024-000000000a00}"
            },
            {
                "@Name": "SourceProcessId",
                "#text": "9240"
            },
            {
                "@Name": "SourceThreadId",
                "#text": "7684"
            },
            {
                "@Name": "SourceImage",
                "#text": "C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\MicrosoftEdgeUpdate.exe"
            },
            {
                "@Name": "TargetProcessGUID",
                "#text": "{82296a8a-7749-6452-8900-000000000a00}"
            },
            {
                "@Name": "TargetProcessId",
                "#text": "5940"
            },
            {
                "@Name": "TargetImage",
                "#text": "C:\\Windows\\system32\\SecurityHealthService.exe"
            },
            {
                "@Name": "GrantedAccess",
                "#text": "0x1000"
            },
            {
                "@Name": "CallTrace",
                "#text": "C:\\Windows\\SYSTEM32\\ntdll.dll+9d584|C:\\Windows\\System32\\wow64.dll+10955|C:\\Windows\\System32\\wow64.dll+901a|C:\\Windows\\System32\\wow64cpu.dll+17c3|C:\\Windows\\System32\\wow64cpu.dll+11b9|C:\\Windows\\System32\\wow64.dll+38c9|C:\\Windows\\System32\\wow64.dll+32bd|C:\\Windows\\SYSTEM32\\ntdll.dll+d39e7|C:\\Windows\\SYSTEM32\\ntdll.dll+74deb|C:\\Windows\\SYSTEM32\\ntdll.dll+74c73|C:\\Windows\\SYSTEM32\\ntdll.dll+74c1e|C:\\Windows\\SYSTEM32\\ntdll.dll+72d5c(wow64)|C:\\Windows\\System32\\KERNELBASE.dll+112638(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+17c4c(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+1799c(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+26e63(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+fcbef(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+94fb6(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+34aec(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+33d73(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+3389d(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+333d1(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\1.3.171.39\\msedgeupdate.dll+745f(wow64)|C:\\Program Files (x86)\\Microsoft\\EdgeUpdate\\MicrosoftEdgeUpdate.exe+ab31"
            },
            {
                "@Name": "SourceUser",
                "#text": "NT AUTHORITY\\SYSTEM"
            },
            {
                "@Name": "TargetUser",
                "#text": "NT AUTHORITY\\SYSTEM"
            }
          ]
        }
      },
      ```

## TODO
+ EVTX Filtering (not use wevtuil.exe)
Need to process in binary format
+ Format Modification
+ Performance Fix