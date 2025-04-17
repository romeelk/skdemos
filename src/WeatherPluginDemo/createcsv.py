import csv

cities_data = [
    {"city": "London", "country": "United Kingdom", "lat": 51.5074, "lon": -0.1278},
    {"city": "New York", "country": "United States", "lat": 40.7128, "lon": -74.0060},
    {"city": "Paris", "country": "France", "lat": 48.8566, "lon": 2.3522},
    {"city": "Tokyo", "country": "Japan", "lat": 35.6895, "lon": 139.6917},
    {"city": "Sydney", "country": "Australia", "lat": -33.8688, "lon": 151.2093},
    {"city": "Cape Town", "country": "South Africa", "lat": -33.9249, "lon": 18.4241},
    {"city": "Moscow", "country": "Russia", "lat": 55.7558, "lon": 37.6173},
    {"city": "Mumbai", "country": "India", "lat": 19.0760, "lon": 72.8777},
    {"city": "Beijing", "country": "China", "lat": 39.9042, "lon": 116.4074},
    {"city": "Rio de Janeiro", "country": "Brazil", "lat": -22.9068, "lon": -43.1729}
]

with open('locations.csv', 'w') as f:
    writer = csv.DictWriter(f,fieldnames=["city", "country", "lat", "lon"])
    writer.writeheader()
    writer.writerows(cities_data)