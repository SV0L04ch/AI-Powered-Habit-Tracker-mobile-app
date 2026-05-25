# mock_weather_api.py
from flask import Flask, request, jsonify

app = Flask(__name__)

@app.route('/weather', methods=['GET'])
@app.route('/data/2.5/weather', methods=['GET'])
def weather():
    city = request.args.get('q')
    if not city:
        return jsonify({"cod": 400, "message": "query parameter 'q' is required"}), 400
    if city.lower() == 'invalidcity':
        return jsonify({"cod": "404", "message": "city not found"}), 404
    if city.lower() == 'ratelimit':
        return jsonify({"cod": 429, "message": "rate limit exceeded"}), 429
    # Успешный ответ по умолчанию
    return jsonify({
        "cod": 200,
        "name": city,
        "main": {"temp": 22, "humidity": 65},
        "weather": [{"main": "Clear"}]
    }), 200


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8080)