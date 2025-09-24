const fs = require('fs');

(async () => {
  if (!fs.existsSync('./certificates')) {
    
    fs.mkdir("./certificates", function(err) {
    });

    const cert = await (await fetch('http://certs.cha.rbxd.ds/local-cert/server.crt')).text();
    const key = await (await fetch('http://certs.cha.rbxd.ds/local-cert/server.key')).text();
    fs.writeFileSync('certificates/cert.pem', cert);
    fs.writeFileSync('certificates/key.pem', key);
  }
})();
