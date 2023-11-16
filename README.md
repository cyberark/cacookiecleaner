# CACookieCleaner (CCC)
This is tool designed to mask the values of the cookies and key value pairs in post data in a recorded HAR file to avoid sharing sensitive information with 3rd parties.
The tool will generate a new HAR file and mask specified cookie values '*'.
Important Note: The original file is not deleted during this process, we recommended to delete the original file to avoid sharing a file with sensitive data.

# Requirements
.NET version 4.8
Newtonsoft.Json version 13.0.3
Python 3.8 and above

# Customization instructions

* Verify the list of the names of cookies defined in "HIDE_COOKIE_VALUE" constant, change it according to your needs.

* In function "modifyEntryCookies":
The list excludedProperties contains names of cookies excluded from masking (The list contains "username" by default).

* NEW FEATURE - you can insert a regex for masking values in key/value pairs in POST DATA section of the HTTP POST Request. By default the tool will masks: "session", "token", "pass", "id".
   
# Usage Instructions 
* run the CACookieCleaner.exe via command line or by double clicking on it.
* When prompted, specify the full directory of the .har file. Example: C:\My\dir\my_file.har
* Enter the comma-separated list of keys for which you want to mask the values (optional). Clicking ENTER it will mask the default parameters. 

The tool will generate a new .har file in the same directory where CACookieCleaner.exe was executed. The output file will be named output(YYYY-MM-DD).har.
THE TOOL DOES NOT DELETE THE ORIGINAL .HAR FILE.

CACookieCleaner.py
Run the script in the command line and follow the instructions

# License
Copyright (c) 2023 CyberArk Software Ltd. All rights reserved.
 
Please note: This tool is provided as is, and subject to the Apache License Version 2.0.
