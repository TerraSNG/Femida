var fs = require('fs');

function ConfigFile(path) {
  this.path = __dirname + path;
  this.Config = {
    token: '1111',
  };
}

ConfigFile.prototype.getConfig = function getConfig() {
  if(fs.existsSync(this.path)) {
    this.Config = JSON.parse(fs.readFileSync(this.path, 'utf8'));
  } else {
    fs.writeFileSync(this.path, JSON.stringify(this.Config), 'utf8');
  }
}

module.exports = ConfigFile;