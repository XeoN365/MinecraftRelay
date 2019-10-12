
var mc = require('minecraft-protocol');
var args = process.argv.slice(2);

if(args.length === 0 || args.length < 3)
{
    console.log(" Usage: node index.js [HOST] [PORT] [USERNAME]");
  process.exit(0);
}
if(mc !== null)
{


var client = mc.createClient({
  host: args[0],   // optional
  port: args[1],         // optional
  username: args[2]
});

var killed = false;

var readline = require('readline');
const cmd = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
  prompt: ''
});




client.on('login', function(e) {
    console.log("Logged in...");
    killed = false;
    if(!killed)
    {
        client.write('chat', { message: "/kill @s" });
        client.write('chat', { message: "/gamemode creative" });
        killed = true;
    }
});

client.on('connect', function(e) {
  console.log("Connected to "+args[0]);
});

process.on('exit', (code) => {
  console.log('Disconnected!');
});
client.on('chat', function(packet) {
  // Listen for chat messages and echo them back.
  var jsonMsg = JSON.parse(packet.message);
  if(jsonMsg.translate === 'chat.type.announcement' || jsonMsg.translate === 'chat.type.text') {
    var username = jsonMsg.with[0].text;
    var msg = jsonMsg.with[1];
    console.log("["+username+"]: "+msg);
  }
});

cmd.on('line', (line) => {
  client.write('chat', {message: (line)});
  
})

}

