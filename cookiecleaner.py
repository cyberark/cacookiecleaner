import os
import json
from datetime import datetime
from typing import List, Any, Dict
import re


HIDE_COOKIE_VALUE = '*********'

class HarFile:
    def __init__(self, input_file: str, user_patterns: str = '') -> None:
        self.patterns: List[Any] = self.generate_postdata_regex(patterns=user_patterns)
        self.input_file: str = input_file
        self.har_json: Any = json.loads(self.get_file_content())  
        self.entries: List[Dict[str,Any]] = self.har_json.get('log',{}).get('entries',[])
        self.output_file: str = os.path.join(os.getcwd(), f"output_{datetime.now()}.har")
        self.header_filters: List[str] = ["cookie", "set-cookie"]
        

    def procces_har_file(self) -> None:
        '''
        procces the orginal har file
        '''
        for entry in self.entries:
            if entry.get('request', {}):
                self.modify_entry_header(entry.get('request', {}))
                self.modify_entry_cookies(entry.get('request', {}))
                
                if entry.get('request', {}).get('method', '') == 'POST':                 
                    if entry.get('request', {}).get('postData',{}):
                        self.modify_post_data(entry.get('request', {}).get('postData',{}))
                    
            if entry.get('response', {}):
                self.modify_entry_header(entry.get('response', {}))
                self.modify_entry_cookies(entry.get('response', {}))
    
    def get_file_content(self) -> str:
        try:
            with open(self.input_file, "r", encoding="utf-8") as har_file:
                return har_file.read()
        except json.JSONDecodeError:
            raise

    def modify_entry_header(self, entry: Dict[str, Any]) -> None:
        '''
        masking cookies
        '''
        entry_headers: List[Dict[str, Any]] = entry.get('headers', {})
        
        for header in entry_headers:
            if header.get('name','').lower() in self.header_filters:
                edited_cookies: str =  self.edit_cookies_values(header['value'].split(';'))
                header['value'] = edited_cookies
                
    def edit_cookies_values(self,cookies: List[str]) -> str:
        return ';'.join([f'{cookie.split("=")[0]}={HIDE_COOKIE_VALUE}' for cookie in cookies])
    
    def modify_entry_cookies(self, entry: Dict[str, Any]) -> None:
        '''
        masking cookies
        '''
        entry_cookies: List[Dict[str,Any]] = entry.get("cookies", {})
        excluded_properties: List[str] = ["username"]
   
        if entry_cookies:   
            for cookie in entry_cookies:
                cookie_name:str = cookie.get('name',{})
                if cookie_name and cookie_name not in excluded_properties:
                    cookie['value'] = HIDE_COOKIE_VALUE
                    
    def generate_postdata_regex(self, patterns: str) -> List[Any]:
        '''
        generate a regex for masking values in postdata
        '''
        default_patterns: List[str] = ['id','token','session','pass']

        if patterns:
            default_patterns.extend(patterns.split(','))
            default_patterns = list(set(default_patterns))
            
            for index in range(len(default_patterns)):
                if default_patterns[index] == "." or default_patterns[index] == "":
                    continue

                if default_patterns[index] == "*":
                    return '.*'


        for index in range(len(default_patterns)):
            default_patterns[index] = default_patterns[index].strip()
            default_patterns[index] = f'.*{default_patterns[index]}.*'

        return '|'.join(default_patterns)
            

    
    def modify_post_data(self, entry: Dict[str, Any]) -> None:
        '''
        modify the postdata section
        '''
        postdata_text_json: json = json.loads(entry.get('text'))
        
        if postdata_text_json:
            for key in postdata_text_json.keys():
                if isinstance(postdata_text_json[key], str):
                    if self.is_key_match_regex(key):
                        postdata_text_json[key] = HIDE_COOKIE_VALUE
                
                elif postdata_text_json[key] is None:
                    continue
                else: 
                    self.handle_none_str_value(postdata_text_json[key])
        
        entry['text'] = json.dumps(postdata_text_json)
        
    def is_key_match_regex(self, key: str) -> bool:
        return bool(re.match(self.patterns, key.lower()))

    
    def handle_none_str_value(self, postdata: Any) -> None:
        if isinstance(postdata, list):
            for data in postdata:
                if isinstance(data, dict):
                    self.handle_none_str_value(data)
                        
        elif isinstance(postdata, dict):
            for key in postdata.keys():
                if self.is_key_match_regex(key):
                        postdata[key] = HIDE_COOKIE_VALUE
                    
    
    def generate_output_file(self):
        '''
        generate a new har file
        '''
        with open(self.output_file, "a") as new_har_file:
            json.dump(self.har_json, new_har_file)
    
def input_validation(file_path: str) -> bool:
    '''
    this function meant to validate the file extention
    '''
    if os.path.exists(file_path):
        extention: str =  os.path.splitext(file_path)[1]
        return extention.lower().endswith('.har')    
    
    return False

if __name__ == "__main__":
    try:
        file_path: str = input('Enter the full path to the input HAR file: ')
        
        patterns: str = input('insert cookies names exclude from post data ')
        
        if not input_validation(file_path):
            exit()

        my_har: HarFile = HarFile(file_path, user_patterns=patterns)
        
        my_har.procces_har_file()
        my_har.generate_output_file()
        
        print("New Har created successfully. Press Enter any key to exit")
    
    except json.JSONDecodeError as ex:
        print("Cannot load file: " + str(ex))

    except Exception as ex:
        print("Failed, Reason: " + str(ex))
