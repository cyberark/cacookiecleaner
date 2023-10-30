import os
import json
from datetime import datetime
from typing import List, Any, Dict

HIDE_COOKIE_VALUE = '*********'

class HarFile:
    def __init__(self, input_file):
            self.input_file: str = input_file
            self.data: Dict[str, Any] = json.loads(self.get_file_content())
            self.entries: List[Dict[str,Any]] = self.data['log']['entries']
            self.output_file: str = os.path.join(os.getcwd(), f"output_{datetime.now().strftime('%Y-%m-%d_%H_%M_%S')}.har")
            self.header_filters: List[str] = ["Cookie", "Set-Cookie"]

    def process_file(self):
        for entry in self.entries:            
            if entry.get('request', {}):
                self.modify_entry_header(entry.get('request', {}))
                self.modify_entry_cookies(entry.get('request', {}))
            
            if entry.get('response', {}):
                self.modify_entry_header(entry.get('response', {}))
                self.modify_entry_cookies(entry.get('response', {}))

    def modify_entry_header(self, entry: Dict[str, Any]):
        headers: List[Dict[str, Any]] = entry.get('headers', {})
        
        if headers:            
            for header in headers:
                if header.get('name', {}) and self.is_filter_found(header.get('name', {})):
                    edited_cookies: str =  self.edit_cookies_values(header['value'].split(';'))
                    header['value'] = edited_cookies
                

    def edit_cookies_values(self,cookies: List[str]) -> str:
        return ';'.join([f'{cookie.split("=")[0]}={HIDE_COOKIE_VALUE}' for cookie in cookies])

    def modify_entry_cookies(self, entry: Dict[str, Any]) -> None:
        entry_cookies: List[Dict[str,Any]] = entry.get("cookies", {})
        excluded_properties: List[str] = ["username"]
   
        if entry_cookies:   
            for cookie in entry_cookies:
                cookie_name:str = cookie.get('name',{})
                if cookie_name and cookie_name not in excluded_properties:
                    cookie['value'] = HIDE_COOKIE_VALUE

    def is_filter_found(self, field_name: str) -> bool:
        for filter in self.header_filters:
            if field_name and (filter in field_name or field_name == filter or field_name.casefold() == filter.casefold()):
                return True
        return False


    def generate_output_file(self):
        with open(self.output_file, "w") as output_file:
            json.dump(self.data, output_file)

    def get_file_content(self):
        with open(self.input_file, "r", encoding="utf-8") as har_file:
            return har_file.read()

    @staticmethod
    def input_validation(file_path):
        if os.path.exists(file_path):
            extention: str =  os.path.splitext(file_path)[1]
            return extention.lower().endswith(".har")
        
        return False

if __name__ == "__main__":
    try:
        file_path: str = input("Enter the full path to the input HAR file: ")
        
        if not HarFile.input_validation(input_file):
            exit()

        har_file: HarFile = HarFile(input_file)
        har_file.process_file()
        har_file.generate_output_file()

        print("New Har created successfully. Press Enter any key to exit")
    
    except Exception as ex:
        print("Failed, Reason: " + str(ex))
