express = require("express");
axios = require("axios");
url = require('url');

router = express.Router();

router.get('/', async function(req, res) {

  console.log(req.query.code);
  console.log(req.query.state);
  res.send("200 OK");
  axios.post('https://discord.com/api/oauth2/token', 
  `client_id=${"749611213406732370"}&client_secret=${"zTlQslYb63TTnfMRLlfBOSplsI3nlYby"}&grant_type=authorization_code&code=${req.query.code}&redirect_uri=http://bcss-su.herokuapp.com&scope=identify`

  ).then(function (response) {
    console.log(response);
  })
  .catch(function (error) {
    console.log(error);
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