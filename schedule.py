from flask import Flask, request, jsonify
app = Flask(__name__)

slots = []

@app.route('/schedule/add', methods=['POST'])
def add_slot():
    data = request.json
    slots.append(data)
    return jsonify({"status": "added", "slot": data}), 201

@app.route('/schedule', methods=['GET'])
def get_slots():
    return jsonify(slots)

if __name__ == '__main__':
    app.run(port=5002)
