var http = require('http');

module.exports = function (app) {
  var logger = app.get('logger');
  var requestOptions = {
    host: "localhost",
    port: "7878",
    path: "/v2/bans/list?token=" + app.get('config').token,
    method: "GET",
    headers: {
      'Content-Type': 'application/json'
    }
  };

  app.get('/bans', function (req, res) {

    var req = http.request(requestOptions, function(response) {
      var data = "";
      response.setEncoding('utf8');
      response.on('data', function (chunk) {
        data += chunk;
      });

      response.on('end', function () {
        var banlist = JSON.parse(data);
        res.render("bans.html", { bans: banlist });
      });
    });

    req.on('error', function (err) {
      logger.error(err);
      res.render("error.html", { error: err.stack });
    });

    req.end();
  });

  app.all('/', function (req, res) {
    logger.error("404: " + req.url);
    res.send(404);
  });

  app.use(function (err, req, res, next) {
    if (err) {
      logger.error(err + ": " + req.url);
      res.render("error.html", { error: err.stack });
    }
    else {
      logger.error("404: " + req.url);
      res.send(404);
    }
  });
};