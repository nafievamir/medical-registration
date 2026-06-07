const express = require('express');
const app = express();
app.use(express.json());

let appointments = [];

app.post('/appointment/create', (req, res) => {
    const newApp = {
        id: appointments.length + 1,
        ...req.body,
        status: 'confirmed'
    };
    appointments.push(newApp);
    res.status(201).json(newApp);
});

app.listen(3000, () => console.log('Appointment service on 3000'));
