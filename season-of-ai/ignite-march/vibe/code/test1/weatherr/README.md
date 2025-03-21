# Weather Dashboard App

A simple Streamlit web application that displays current weather information for any city.

## Setup Instructions

1. First, install the required dependencies:
   ```
   pip install -r requirements.txt
   ```

2. Get your API key:
   - Sign up for a free API key at [OpenWeatherMap](https://openweathermap.org/api)
   - Replace `YOUR_OPENWEATHERMAP_API_KEY` in `weather_app.py` with your actual API key

3. Run the Streamlit app:
   ```
   streamlit run weather_app.py
   ```

4. Open your web browser and go to the URL shown in the terminal (usually http://localhost:8501)

## Features

- Real-time weather data for any city worldwide
- Displays temperature, humidity, wind speed, and more
- Visual weather icons
- Responsive design
- Sunrise and sunset times
- Raw data view option

## Technologies Used

- Python
- Streamlit
- OpenWeatherMap API

## Note

This app is for educational purposes. API usage may be rate-limited with the free OpenWeatherMap plan.