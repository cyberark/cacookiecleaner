import json

def clear_cookies_and_tokens(input_file, output_file):
    try:
        with open(input_file, 'r') as file:
            har_data = json.load(file)

        for entry in har_data.get('log',{}).get('entries'):
            if 'request' in entry:
                if 'headers' in entry.get('request', {}):
                    entry['request']['headers'] = [header for header in entry['request']['headers'] if 'cookie' not in header['name'].lower()]
            if 'response' in entry:
                if 'headers' in entry.get('response',{}):
                    entry['response']['headers'] = [header for header in entry['response']['headers'] if 'set-cookie' not in header['name'].lower()]

        with open(output_file, 'w') as output:
            json.dump(har_data, output, indent=2)

        print('cookies cleaned successfully')
    
    except Exception as e:
        print(f"Error: {str(e)}")

if __name__ == '__main__':
    input_file: str = input('insert full directory of the hat file -> ')
    output_file:str = input('insert full path to the output file -> ')
    
    
    if input_file.endswith('.har'):
        clear_cookies_and_tokens(input_file, output_file)
    
