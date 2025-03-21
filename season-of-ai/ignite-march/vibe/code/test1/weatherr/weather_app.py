import streamlit as st
import requests
import json
from datetime import datetime

# Set page configuration
st.set_page_config(page_title="Weather App", page_icon="🌤️")

# Add a title and description
st.title("📊 Weather Dashboard")
st.write("Enter a city name to get the current weather information.")

# API configuration
API_KEY = "YOUR_OPENWEATHERMAP_API_KEY"  # Replace with your actual API key
BASE_URL = "https://api.openweathermap.org/data/2.5/weather"

# Function to get weather data
def get_weather_data(city):
    params = {
        "q": city,
        "appid": API_KEY,
        "units": "metric"  # Use metric units for temperature in Celsius
    }
    
    response = requests.get(BASE_URL, params=params)
    
    if response.status_code == 200:
        return response.json()
    else:
        return None

# Input for city name
city_name = st.text_input("Enter City Name", "")

# Button to trigger the API call
if st.button("Get Weather") or city_name:
    if city_name:
        # Show a spinner while fetching data
        with st.spinner(f"Fetching weather data for {city_name}..."):
            weather_data = get_weather_data(city_name)
        
        if weather_data:
            # Display the weather information in a nice format
            col1, col2 = st.columns(2)
            
            # Extract data
            temp = weather_data["main"]["temp"]
            feels_like = weather_data["main"]["feels_like"]
            humidity = weather_data["main"]["humidity"]
            pressure = weather_data["main"]["pressure"]
            wind_speed = weather_data["wind"]["speed"]
            description = weather_data["weather"][0]["description"].capitalize()
            icon_code = weather_data["weather"][0]["icon"]
            icon_url = f"http://openweathermap.org/img/wn/{icon_code}@2x.png"
            
            # Format sunrise and sunset times
            sunrise_time = datetime.fromtimestamp(weather_data["sys"]["sunrise"])
            sunset_time = datetime.fromtimestamp(weather_data["sys"]["sunset"])
            
            # Display the information
            with col1:
                st.metric("Temperature", f"{temp} °C")
                st.metric("Feels Like", f"{feels_like} °C")
                st.metric("Humidity", f"{humidity}%")
            
            with col2:
                st.image(icon_url)
                st.write(f"**{description}**")
                st.metric("Wind Speed", f"{wind_speed} m/s")
                
            # Display additional information
            st.subheader("Additional Information")
            col3, col4 = st.columns(2)
            
            with col3:
                st.write(f"**Pressure:** {pressure} hPa")
                st.write(f"**Sunrise:** {sunrise_time.strftime('%H:%M')} local time")
            
            with col4:
                st.write(f"**Country:** {weather_data['sys']['country']}")
                st.write(f"**Sunset:** {sunset_time.strftime('%H:%M')} local time")
                
            # Display raw JSON data in an expandable section
            with st.expander("View Raw Data"):
                st.json(weather_data)
                
        else:
            st.error(f"Could not retrieve weather data for {city_name}. Please check the city name and try again.")
    else:
        st.warning("Please enter a city name.")

# Add footer
st.markdown("---")
st.caption("Data provided by OpenWeatherMap API")