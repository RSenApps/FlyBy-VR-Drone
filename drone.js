var sys = require('sys');
var net = require('net');
var sockets = [];
 var arDrone = require('ar-drone');
var client  = arDrone.createClient();
var flying = false;
fs = require('fs');
var svr = net.createServer(function(sock) {
    sys.puts('Connected: ' + sock.remoteAddress + ':' + sock.remotePort); 
    sock.write('Hello ' + sock.remoteAddress + ':' + sock.remotePort + '\n');
    sockets.push(sock);
	var message = "";
	var pngStream = client.getPngStream();
	
    sock.on('data', function(data) {  // client writes message
		pngStream.on('data', function(png) {
			fs.writeFile("test.jpg", png, function(err){
                    if (err) console.log(err);
                    //console.log(fileName + ' Saved');

                });
		});
		console.log(data.toString());
		for (var i = 0; i < data.toString().length; i++)
		{
			if (data.toString().charAt(i) == '|')
			{
				processMessage(message);
				message = "";
				//client.on('navdata', console.log);
			}
			else {
						message += data.toString().charAt(i);

			}
		}
		/*
        var len = sockets.length;
        for (var i = 0; i < len; i ++) { // broad cast
            if (sockets[i] != sock) {
                if (sockets[i]) {
                    sockets[i].write(sock.remoteAddress + ':' + sock.remotePort + ':' + data);
                }
            }
        }
		*/
    });
 
    sock.on('end', function() { // client disconnects
        var idx = sockets.indexOf(sock);
        if (idx != -1) {
            delete sockets[idx];
        }
    });
});
function processMessage (message)
{
var speed = getSpeed(message);
	if (message == "t") {
            console.log("Takeoff");
			flying = true;
			client.takeoff();
        }
		else if (message == "l") {
            console.log("Landing");
			flying = false;
			client.land();
        }
				

		else if (flying && speed != -979) {
			switch(message.charAt(0))
			{
				case 'u':
					client.up(speed);
					break;
				case 'd':
					client.down(speed);
					break;
				case 'l':	
					client.left(speed);
					break;
				case 'r':
					client.right(speed);
					break;
				case 'f':	
					client.front(speed);
					break;
				case 'b':
					client.back(speed);
					break;
				case 'c':
					if (speed < 0)
					{
					//console.log("clockwise");
						client.counterClockwise(-speed);
						}
						else {
							client.clockwise(speed);
						}
					break;
			}
		}
		
}
function getSpeed (message)
{
	try {
		return parseFloat(message.substring(1, message.length));
	}
	catch (e)
	{
		return -979;
	}
}
var svraddr = '127.0.0.1';
var svrport = 5000;
 
svr.listen(svrport, svraddr);
sys.puts('Server Created at ' + svraddr + ':' + svrport + '\n');