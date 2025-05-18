from flask import Flask, jsonify, request
import requests
from geopy.geocoders import Nominatim
from geopy.exc import GeocoderTimedOut, GeocoderUnavailable

app = Flask(__name__)
geolocator = Nominatim(user_agent="weather_app_copilot")

def get_coordinates(city_name):
    """Geocode a city name to latitude and longitude."""
    try:
        location = geolocator.geocode(city_name, timeout=10)
        if location:
            return location.latitude, location.longitude
    except GeocoderTimedOut:
        print(f"Geocoding timed out for {city_name}")
    except GeocoderUnavailable:
        print(f"Geocoding service unavailable for {city_name}")
    except Exception as e:
        print(f"An error occurred during geocoding for {city_name}: {e}")
    return None, None

@app.route('/weather', methods=['GET'])
def get_weather():
    city = request.args.get('city')
    if not city:
        return jsonify({"error": "City parameter is required"}), 400

    latitude, longitude = get_coordinates(city)

    if latitude is None or longitude is None:
        return jsonify({"error": f"Could not find coordinates for city: {city}"}), 404

    try:
        weather_url = f"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true"
        response = requests.get(weather_url, timeout=10)
        response.raise_for_status()  # Raises an HTTPError for bad responses (4XX or 5XX)
        weather_data = response.json()
        return jsonify(weather_data)
    except requests.exceptions.RequestException as e:
        return jsonify({"error": f"Failed to fetch weather data: {e}"}), 500
    except ValueError: # Includes JSONDecodeError
        return jsonify({"error": "Invalid response from weather API"}), 500

if __name__ == '__main__':
    # Reminder: Run this app using the 'dataaijhb' conda environment.
    # Example:
    # conda activate dataaijhb
    # python weather_app.py
    app.run(debug=True, port=5000)
