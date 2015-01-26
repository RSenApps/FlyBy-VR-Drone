var arDrone = require('ar-drone');
var client  = arDrone.createClient();

require('ar-drone-png-stream')(client, { port: 8000});