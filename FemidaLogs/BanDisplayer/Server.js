var express = require('express');
var app = express();
var cons = require('consolidate');
var swig = require('swig');
var path = require('path');
var logger = require('./logger');
var conf = require('./configfile');

app.engine('.html', cons.swig);
app.set('view engine', 'html');

swig.setDefaults({
  root: './templates/',
  allowErrors: true,
  cache: false
});

app.set('views', './templates/');
app.use(express.static(path.join(__dirname, 'public')));

var config = new conf("/config.json");
config.getConfig();

app.set('logger', logger);
app.set('config', config.Config);

require('./routes/routes')(app);

var port = 8000;
if (process.argv.length > 2) {
  port = parseInt(process.argv[2]);
}
app.listen(port);

logger.info("Server has started on port " + port);