# CACookieCleaner (CCC)
This is tool meant to hide the value of the cookies from the recorded HAR file from chrome.
The tool generate a new HAR file and the cookie value is hidden with '*'.
Important Note: The original file is not deleted during this process, very recommended to delete the original file.

# Requirements
dot net version 4.8
Newtonsoft.Json version 13.0.3
Python 3.8 and above

# Usage instructions
CACookieCleaner.exe:
run the CACookieCleaner.exe via terminal/double click.
Insert the full direcotry of the .har file. Example: C:\My\dir\my_file.har
The tool will generate a new .har file in the same direcotry the CACookieCleaner.exe were triggered, named output(YYYY-MM-DD).har.

CACookieCleaner.py / CACookieCleaner.ps1
Run the script in the command line and follow the instructions

# License
Copyright (c) 2023 CyberArk Software Ltd. All rights reserved.
 
Please note: This tool is provided as is, and subject to the Apache License Version 2.0.
