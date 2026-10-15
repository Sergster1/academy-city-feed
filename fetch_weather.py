#!/usr/bin/env python3
"""Fetch current NYC weather (Open-Meteo, no API key) and write site/weather.json.
Stdlib only. On any failure the previous file is left untouched."""
import json, sys, urllib.request, datetime, pathlib

URL = ("https://api.open-meteo.com/v1/forecast?latitude=40.7128&longitude=-74.0060"
       "&current=temperature_2m,weather_code,wind_speed_10m,is_day"
       "&daily=temperature_2m_max,temperature_2m_min&temperature_unit=fahrenheit"
       "&wind_speed_unit=mph&timezone=America%2FNew_York&forecast_days=1")

WMO = {0:"Clear",1:"Mostly clear",2:"Partly cloudy",3:"Overcast",45:"Fog",48:"Fog",
       51:"Light drizzle",53:"Drizzle",55:"Heavy drizzle",56:"Freezing drizzle",57:"Freezing drizzle",
       61:"Light rain",63:"Rain",65:"Heavy rain",66:"Freezing rain",67:"Freezing rain",
       71:"Light snow",73:"Snow",75:"Heavy snow",77:"Snow grains",
       80:"Rain showers",81:"Rain showers",82:"Violent showers",85:"Snow showers",86:"Snow showers",
       95:"Thunderstorm",96:"Thunderstorm",99:"Thunderstorm"}

def main():
    with urllib.request.urlopen(URL, timeout=20) as r:
        d = json.load(r)
    cur, day = d["current"], d["daily"]
    code = int(cur["weather_code"])
    out = {
        "city": "New York",
        "temp_f": round(cur["temperature_2m"]),
        "hi_f": round(day["temperature_2m_max"][0]),
        "lo_f": round(day["temperature_2m_min"][0]),
        "wind_mph": round(cur["wind_speed_10m"]),
        "code": code,
        "desc": WMO.get(code, "Unknown"),
        "is_day": int(cur["is_day"]),
        "updated": datetime.datetime.now(datetime.timezone.utc).strftime("%Y-%m-%dT%H:%MZ"),
    }
    p = pathlib.Path(__file__).parent / "site"
    p.mkdir(exist_ok=True)
    (p / "weather.json").write_text(json.dumps(out, separators=(",", ":")))
    print(out)

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print("fetch failed:", e, file=sys.stderr)
        sys.exit(1)
