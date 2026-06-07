from flask import Flask, request, jsonify
import sqlite3

app = Flask(__name__)

# Инициализация БД
conn = sqlite3.connect('medical.db')
conn.execute('''
    CREATE TABLE IF NOT EXISTS doctors (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        specialty TEXT NOT NULL
    )
''')
conn.close()

@app.route('/doctors', methods=['GET'])
def get_doctors():
    conn = sqlite3.connect('medical.db')
    cur = conn.cursor()
    cur.execute('SELECT id, name, specialty FROM doctors')
    doctors = [{'id': row[0], 'name': row[1], 'specialty': row[2]} for row in cur.fetchall()]
    conn.close()
    return jsonify(doctors)

@app.route('/doctors', methods=['POST'])
def create_doctor():
    data = request.json
    conn = sqlite3.connect('medical.db')
    cur = conn.cursor()
    cur.execute('INSERT INTO doctors (name, specialty) VALUES (?, ?)', 
                (data['name'], data['specialty']))
    conn.commit()
    doctor_id = cur.lastrowid
    conn.close()
    return jsonify({'id': doctor_id, 'status': 'created'}), 201

if __name__ == '__main__':
    app.run(port=5001)
