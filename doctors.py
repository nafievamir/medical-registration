from flask import Flask, request, jsonify
import sqlite3

app = Flask(__name__)

# Создаём БД и таблицу
conn = sqlite3.connect('medical.db')
conn.execute('DROP TABLE IF EXISTS doctors')
conn.execute('''
    CREATE TABLE doctors (
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
    try:
        # Принудительно декодируем в UTF-8
        raw_data = request.get_data()
        decoded = raw_data.decode('utf-8', errors='ignore')
        import json
        data = json.loads(decoded)
        
        name = data.get('name')
        specialty = data.get('specialty')
        
        conn = sqlite3.connect('medical.db')
        cur = conn.cursor()
        cur.execute('INSERT INTO doctors (name, specialty) VALUES (?, ?)', (name, specialty))
        conn.commit()
        doctor_id = cur.lastrowid
        conn.close()
        
        return jsonify({'id': doctor_id, 'status': 'created'}), 201
    except Exception as e:
        print(f"Ошибка: {e}")
        return jsonify({'error': str(e)}), 400

if __name__ == '__main__':
    app.run(port=5001, debug=True)
