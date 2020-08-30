const http = require('http');
const axios = require('axios')
const app = require('./app.js');
var port = process.env.PORT || 3000;

var server = http.createServer(app);
server.listen(port);