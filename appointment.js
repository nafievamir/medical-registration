const express = require('express');
const app = express();
app.use(express.json());

app.post('/webhook/doctor_created', (req, res) => {
    console.log('[APPOINTMENT] Получен новый врач:', req.body);
    res.json({ status: 'ok' });
});

app.post('/webhook/appointment_created', (req, res) => {
    console.log('[APPOINTMENT] Получена новая запись:', req.body);
    res.json({ status: 'ok' });
});

app.post('/appointment/create', (req, res) => {
    console.log('[APPOINTMENT] Создание записи:', req.body);
    res.json({ id: 1, status: 'confirmed' });
});

app.listen(3000, () => console.log('Appointment service on 3000'));
