express = require("express");
axios = require("axios");
url = require('url');

router = express.Router();

router.get('/', async function(req, res) {

  console.log(req.query.code);
  console.log(req.query.state);
  res.redirect("https://www.thesubath.com/bcss/");

  axios.post('https://discord.com/api/oauth2/token', 
  `client_id=${"749611213406732370"}&client_secret=${"zTlQslYb63TTnfMRLlfBOSplsI3nlYby"}&grant_type=authorization_code&code=${req.query.code}&redirect_uri=http://bcss-su.herokuapp.com&scope=identify`  
  ).then(function (response) {
    axios.get('https://discord.com/api/users/@me', { 'headers': {'Authorization': 'Bearer ' + response.data.access_token} })
    .then(function(response2) {
      console.log(response2.data.id);
      axios.post('51.15.222.156/api/User/', {
        'userHash': req.query.state,
        'discordId': response2.data.id
      });
    });
  });
/*
  axios.post('http://localhost:3000/test2', {
    todo: 'Buy the milk'
  });
  res.send("Yay! home page :) - " + req.query.state);
  */
});

module.exports = router;