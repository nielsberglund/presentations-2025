import streamlit as st
import requests
from datetime import datetime

# Title of the Streamlit app
st.title("Weather Checker")

# Input field for the city name
city_name = st.text_input("Enter city name:", "Johannesburg")

# Button to trigger the weather fetch
if st.button("Get Weather"):
    if city_name:
        # URL of the Flask backend
        # Make sure your Flask app (weather_app.py) is running on port 5000
        flask_backend_url = f"http://127.0.0.1:5000/weather?city={city_name}"

        try:
            response = requests.get(flask_backend_url, timeout=10)
            response.raise_for_status()  # Raise an exception for HTTP errors
            weather_data = response.json()

            if "error" in weather_data:
                st.error(f"Error from backend: {weather_data['error']}")
            elif "current_weather" in weather_data:
                current_weather = weather_data["current_weather"]
                temperature = current_weather.get("temperature")
                time_str = current_weather.get("time")
                windspeed = current_weather.get("windspeed")
                weathercode = current_weather.get("weathercode")


                st.subheader(f"Weather in {city_name.capitalize()}")

                if temperature is not None:
                    st.metric(label="Temperature", value=f"{temperature}°C")

                if time_str is not None:
                    # Parse the ISO 8601 timestamp
                    dt_object = datetime.fromisoformat(time_str)
                    # Format it to a more readable string
                    readable_time = dt_object.strftime("%Y-%m-%d %I:%M %p")
                    st.write(f"**Time:** {readable_time}")
                else:
                    st.write("**Time:** Not available")

                if windspeed is not None:
                    st.write(f"**Wind Speed:** {windspeed} km/h")

                # You can add more details like weathercode interpretation if needed
                # For example: https://open-meteo.com/en/docs#weathervariables
                if weathercode is not None:
                    st.write(f"**Weather Code:** {weathercode} (Refer to Open-Meteo docs for interpretation)")


            else:
                st.error("Received unexpected data from the weather service.")

        except requests.exceptions.ConnectionError:
            st.error(f"Could not connect to the backend at {flask_backend_url}. Please ensure the Flask weather app is running.")
        except requests.exceptions.Timeout:
            st.error("The request to the weather service timed out.")
        except requests.exceptions.HTTPError as e:
            st.error(f"HTTP error occurred: {e}")
        except requests.exceptions.RequestException as e:
            st.error(f"An error occurred while fetching weather data: {e}")
        except ValueError: # Includes JSONDecodeError
            st.error("Could not parse the response from the weather service. Invalid JSON.")
    else:
        st.warning("Please enter a city name.")

st.markdown("""
---
**Note:**
- Make sure the Python Flask backend (`weather_app.py`) is running on `http://127.0.0.1:5000`.
- You can run the Flask backend with: `conda activate dataaijhb` followed by `python weather_app.py`.
- You can run this Streamlit frontend with: `conda activate dataaijhb` followed by `streamlit run frontend_app.py`.
""")
