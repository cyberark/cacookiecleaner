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
* in class HarFile you can modify the value "HIDE_COOKIE_VALUE", this is how the values are maked.

* In function "modifyEntryCookies":
You have a list excludedProperties you can exclude cookies by adding the cookies names (in the example i inserted "username")

* NEW FEATURE - you can insert a regex for masking, by default it will mask: "session", "token", "pass", "id".
  you can modify as much as you, for example: username, action
  please use comma (,) as sepearator, it will mask the cookie that contain the key, for example if you insert username and some key contain that word, it will mask it.
  
# Using CACookieCleaner
* run the CACookieCleaner.exe via terminal/double click. 
* Insert the full direcotry of the .har file. Example: C:\My\dir\my_file.har
* Enter the properties you want to mask (not mandatory) by clicking ENTER it will collect the default. but if you want to use this feature seperate the value with comma.

The tool will generate a new .har file in the same direcotry the CACookieCleaner.exe were triggered, named output(YYYY-MM-DD).har.
THE TOOL DON'T DELETE THE ORIGINAL .HAR FILE.

CACookieCleaner.py
Run the script in the command line and follow the instructions

# License
Copyright (c) 2023 CyberArk Software Ltd. All rights reserved.
 
Please note: This tool is provided as is, and subject to the Apache License Version 2.0.
