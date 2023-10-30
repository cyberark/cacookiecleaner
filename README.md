# CACookieCleaner (CCC)
This is tool meant to hide the value of the cookies from the recorded HAR file from chrome.
The tool generate a new HAR file and the cookie value is hidden with '*'.
Important Note: The original file is not deleted during this process, very recommended to delete the original file.

# Requirements
dot net version 4.8
Newtonsoft.Json version 13.0.3
Python 3.8 and above

# Usage instructions
BEFORE RUNNING:

The class HarFile contain 2 properties:
1. HIDE_COOKIE_VALUE
2. ALLOWED_DIRECTORY

HIDE_COOKIE_VALUE - How the cookie value will be displayed in the new generated HAR file
ALLOWED_DIRECTORY - In order to prevent Path Traversal Attack you need to edit this value to the dir that the tool will run only from there.

In function "modifyEntryCookies":
You have a list excludedProperties you can exclude cookies by adding the cookies names (in the example i inserted "username")

run the CACookieCleaner.exe via terminal/double click. 
Insert the full direcotry of the .har file. Example: C:\My\dir\my_file.har
The tool will generate a new .har file in the same direcotry the CACookieCleaner.exe were triggered, named output(YYYY-MM-DD).har.

CACookieCleaner.py
Run the script in the command line and follow the instructions

# License
Copyright (c) 2023 CyberArk Software Ltd. All rights reserved.
 
Please note: This tool is provided as is, and subject to the Apache License Version 2.0.
