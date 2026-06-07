from flask import Flask, jsonify
app = Flask(__name__)

@app.route('/doctors')
def get_doctors():
    return jsonify([
        {"id": 1, "name": "Иванов И.И.", "specialty": "Терапевт"},
        {"id": 2, "name": "Петрова А.С.", "specialty": "Кардиолог"}
    ])

if __name__ == '__main__':
    app.run(port=5001)
