express = require("express");
request = require('request');
url = require('url');

router = express.Router();

router.get('/', async function(req, res) {

  console.log(req.query.code);
  console.log(req.query.state);
  res.send("200 OK");

  request.post('https://discord.com/api/oauth2/token', {
    form: {
      'client_id': 749611213406732370,
      'client_secret': "zTlQslYb63TTnfMRLlfBOSplsI3nlYby",
      'grant_type': 'authorization_code',
      'code': req.query.code,
      'redirect_uri': 'http://bcss-su.herokuapp.com',
      'scope': 'identify'
    }
  }, function (err, response, body) {
    console.log(response);
  });
/*
  axios.post('http://localhost:3000/test2', {
    todo: 'Buy the milk'
  });
  res.send("Yay! home page :) - " + req.query.state);
  */
});

router.get('/callback', async function(req, res) {
  axios.post('http://localhost:3000/test2', {
    todo: 'Buy the milk'
  });

  res.redirect("https://www.thesubath.com/bcss/");
  /*
  var data = await spotify.finaliseAuth(req.query.code);
  await database.updateAuthInfo(req.query.state, req.query.code, data);
  res.redirect('landing');*/
});

router.post("/test2", async function (req, res, next) {
  res.send("got it lol");
});

module.exports = router;