import os
from flask import Flask, request, jsonify
import requests

app = Flask(__name__)

# It's recommended to use an environment variable for the API key
# For development, you can hardcode it, but ensure it's not committed to version control
OPENWEATHERMAP_API_KEY = os.environ.get("OPENWEATHERMAP_API_KEY", "YOUR_DEFAULT_API_KEY")
OPENWEATHERMAP_URL = "http://api.openweathermap.org/data/2.5/weather"

@app.route('/weather', methods=['GET'])
def get_weather():
    city = request.args.get('city')
    if not city:
        return jsonify({"error": "City parameter is required"}), 400

    params = {
        'q': city,
        'appid': OPENWEATHERMAP_API_KEY,
        'units': 'metric'  # Or 'imperial' for Fahrenheit
    }

    try:
        response = requests.get(OPENWEATHERMAP_URL, params=params)
        response.raise_for_status()  # Raises an HTTPError for bad responses (4XX or 5XX)
        weather_data = response.json()
        return jsonify(weather_data)
    except requests.exceptions.HTTPError as errh:
        return jsonify({"error": f"Http Error: {errh}"}), response.status_code
    except requests.exceptions.ConnectionError as errc:
        return jsonify({"error": f"Error Connecting: {errc}"}), 500
    except requests.exceptions.Timeout as errt:
        return jsonify({"error": f"Timeout Error: {errt}"}), 500
    except requests.exceptions.RequestException as err:
        return jsonify({"error": f"Oops: Something Else: {err}"}), 500

if __name__ == '__main__':
    # Make sure to set the OPENWEATHERMAP_API_KEY environment variable
    # For example, in your terminal: export OPENWEATHERMAP_API_KEY='your_api_key_here'
    # Or, for development, you can temporarily replace "YOUR_DEFAULT_API_KEY" above
    # with your actual OpenWeatherMap API key.
    if OPENWEATHERMAP_API_KEY == "YOUR_DEFAULT_API_KEY":
        print("Warning: OPENWEATHERMAP_API_KEY is not set. Please set it as an environment variable or in the script for development.")
    app.run(debug=True, port=5000)
