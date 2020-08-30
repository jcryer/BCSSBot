express = require("express");
axios = require("axios");
url = require('url');

router = express.Router();

router.get('/', async function(req, res) {
 // axios.post('http://localhost:3000/test2', {
 //   todo: 'Buy the milk'
 // });
  console.log(req.query.state);
  console.log(req.query.access_token);
  res.send("200 OK");
  //res.send("Yay! home page :) - " + req.query.state);
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