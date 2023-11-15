# CACookieCleaner (CCC)
This is tool designed to hide the value of the cookies from the recorded HAR file to avoid sharing sensitive information with 3rd parties.
The tool will generate a new HAR file and mask specified cookie values '*'.
Important Note: The original file is not deleted during this process, we recommended to delete the original file to avoid sharing a file with sensitive data.

# Requirements
dot net version 4.8
Newtonsoft.Json version 13.0.3
Python 3.8 and above

# Usage instructions
BEFORE RUNNING:
* Verify the list of the names of coolies defined in "HIDE_COOKIE_VALUE" constant, change it according to your needs.

* In function "modifyEntryCookies":
You have a list excludedProperties you can exclude cookies by adding the cookies names (in the example i inserted "username")

* NEW FEATURE - you can insert a regex for masking, by default it will mask: "session", "token", "pass", "id".
  you can modify as much as you, for example: username, action
  Use comma (,) as separator, it will mask the cookie that containa the key, for example if you insert username and a key contain that word, the tool will mask that key.
  
# Using CACookieCleaner
* run the CACookieCleaner.exe via command line or by double clicking on it.
* When prompted, specify the full directory of the .har file. Example: C:\My\dir\my_file.har
* Enter the comma-separated list of properties you want to mask (optional). Clicking ENTER it will mask the default parameters. 

The tool will generate a new .har file in the same directory where CACookieCleaner.exe was executed. The output file will be named output(YYYY-MM-DD).har.
THE TOOL DOES NOT DELETE THE ORIGINAL .HAR FILE.

CACookieCleaner.py
Run the script in the command line and follow the instructions

# License
Copyright (c) 2023 CyberArk Software Ltd. All rights reserved.
 
Please note: This tool is provided as is, and subject to the Apache License Version 2.0.
