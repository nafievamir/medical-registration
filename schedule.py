from flask import Flask, request, jsonify
import sqlite3

app = Flask(__name__)

conn = sqlite3.connect('medical.db')
conn.execute('''
    CREATE TABLE IF NOT EXISTS slots (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        doctor_id INTEGER NOT NULL,
        time TEXT NOT NULL,
        is_available BOOLEAN DEFAULT 1
    )
''')
conn.close()

@app.route('/webhook/doctor_created', methods=['POST'])
def doctor_created_webhook():
    data = request.json
    print(f"[SCHEDULE] Получен новый врач: {data}")
    return jsonify({'status': 'ok'})

@app.route('/schedule/add', methods=['POST'])
def add_slot():
    data = request.json
    conn = sqlite3.connect('medical.db')
    conn.execute("INSERT INTO slots (doctor_id, time, is_available) VALUES (?, ?, 1)", (data['doctor_id'], data['time']))
    conn.commit()
    conn.close()
    return jsonify({'status': 'added'}), 201

if __name__ == '__main__':
    app.run(port=5002)
