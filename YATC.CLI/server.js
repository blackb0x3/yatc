const http = require('http');
const fs = require('fs');
const path = require('path');

http.createServer((req, res) => {
    if (req.url === '/') {
        fs.readFile(path.join(__dirname, 'twitch-auth.html'), (err, data) => {
            if (err) {
                res.writeHead(500);
                res.end('Error loading HTML file');
            } else {
                res.writeHead(200, { 'Content-Type': 'text/html' });
                res.end(data);
            }
        });
    } else if (req.url === '/twitch-auth.js') {
        fs.readFile(path.join(__dirname, 'twitch-auth.js'), (err, data) => {
            if (err) {
                res.writeHead(500);
                res.end('Error loading JavaScript file');
            } else {
                res.writeHead(200, { 'Content-Type': 'application/javascript' });
                res.end(data);
            }
        });
    } else {
        res.writeHead(404);
        res.end('Not found');
    }
}).listen(5678, () => {
    console.log('Server listening on port 5678');
});