const http = require('http');
const request = require('request');
const app = require('./app.js');
var port = process.env.PORT || 3000;

var server = http.createServer(app);
server.listen(port);