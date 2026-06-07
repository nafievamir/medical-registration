from flask import Flask, request, jsonify
import sqlite3
import requests
import threading
import time

app = Flask(__name__)

# Создаём базу и таблицы
conn = sqlite3.connect('medical.db')
conn.execute('''
    CREATE TABLE IF NOT EXISTS doctors (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        name TEXT NOT NULL,
        specialty TEXT NOT NULL
    )
''')
conn.execute('''
    CREATE TABLE IF NOT EXISTS slots (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        doctor_id INTEGER NOT NULL,
        time TEXT NOT NULL,
        is_available BOOLEAN DEFAULT 1
    )
''')
conn.execute('''
    CREATE TABLE IF NOT EXISTS appointments (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        patient_id INTEGER NOT NULL,
        doctor_id INTEGER NOT NULL,
        datetime TEXT NOT NULL,
        status TEXT NOT NULL
    )
''')
conn.close()

# Компенсация (Saga): освобождаем слот при ошибке
def compensate_slot(doctor_id, slot_time):
    try:
        conn = sqlite3.connect('medical.db')
        conn.execute("UPDATE slots SET is_available = 1 WHERE doctor_id = ? AND time = ?", (doctor_id, slot_time))
        conn.commit()
        conn.close()
        print(f"[COMPENSATE] Слот {doctor_id} {slot_time} освобожден")
    except Exception as e:
        print(f"[COMPENSATE] Ошибка отката: {e}")

@app.route('/appointment/saga', methods=['POST'])
def saga_create_appointment():
    data = request.json
    patient_id = data['patient_id']
    doctor_id = data['doctor_id']
    slot_time = data['slot_time']
    datetime = data['datetime']
    
    print(f"[SAGA START] Запись пациента {patient_id} к врачу {doctor_id} на {slot_time}")
    
    try:
        # ШАГ 1 - Резервируем слот
        conn = sqlite3.connect('medical.db')
        cur = conn.cursor()
        cur.execute("SELECT id FROM slots WHERE doctor_id = ? AND time = ? AND is_available = 1", (doctor_id, slot_time))
        slot = cur.fetchone()
        if not slot:
            return jsonify({"error": "Slot not available"}), 409
        
        cur.execute("UPDATE slots SET is_available = 0 WHERE id = ?", (slot[0],))
        conn.commit()
        print(f"[SAGA STEP 1] Слот зарезервирован")
        
        # ШАГ 2 - Создаём запись
        cur.execute("INSERT INTO appointments (patient_id, doctor_id, datetime, status) VALUES (?, ?, ?, ?)",
                    (patient_id, doctor_id, datetime, 'confirmed'))
        appointment_id = cur.lastrowid
        conn.commit()
        print(f"[SAGA STEP 2] Запись создана ID={appointment_id}")
        
        conn.close()
        
        # ШАГ 3 - Отправляем вебхуки об успехе
        threading.Thread(target=lambda: requests.post('http://localhost:5002/webhook/appointment_created', json={'appointment_id': appointment_id})).start()
        threading.Thread(target=lambda: requests.post('http://localhost:3000/webhook/appointment_created', json={'appointment_id': appointment_id})).start()
        
        print(f"[SAGA SUCCESS] Успешно завершена")
        return jsonify({"id": appointment_id, "status": "confirmed"}), 201
        
    except Exception as e:
        print(f"[SAGA ERROR] {e}")
        # КОМПЕНСАЦИЯ
        compensate_slot(doctor_id, slot_time)
        return jsonify({"error": "Transaction failed, rollback done"}), 500

@app.route('/doctors', methods=['POST'])
def create_doctor():
    data = request.json
    conn = sqlite3.connect('medical.db')
    cur = conn.cursor()
    cur.execute("INSERT INTO doctors (name, specialty) VALUES (?, ?)", (data['name'], data['specialty']))
    conn.commit()
    doctor_id = cur.lastrowid
    conn.close()
    
    # Вебхуки
    threading.Thread(target=lambda: requests.post('http://localhost:5002/webhook/doctor_created', json={'id': doctor_id, 'name': data['name']})).start()
    threading.Thread(target=lambda: requests.post('http://localhost:3000/webhook/doctor_created', json={'id': doctor_id})).start()
    
    return jsonify({"id": doctor_id, "status": "created"}), 201

if __name__ == '__main__':
    app.run(port=5001)
