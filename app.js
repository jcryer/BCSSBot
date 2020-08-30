const express = require('express');
const app = express();

app.use("/", require('./index'));

module.exports = app;