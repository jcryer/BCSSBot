express = require("express");
axios = require("axios");
url = require('url');

router = express.Router();
var errorText = `Sorry, something's gone wrong!<br><br>
Please contact <a href='mailto:jjc82@bath.ac.uk'>Joseph Cryer</a> or <a href='mailto:ar2227@bath.ac.uk'>Alfie Richards</a>.<br>
<a href='https://www.thesubath.com/bcss/'>Link to BCSS Discord.</a>.
`;

router.get('/', async function(req, res) {
  
  axios.post('https://discord.com/api/oauth2/token', 
  `client_id=${"749611213406732370"}&client_secret=${"zTlQslYb63TTnfMRLlfBOSplsI3nlYby"}&grant_type=authorization_code&code=${req.query.code}&redirect_uri=http://bcss-su.herokuapp.com&scope=identify`  
  )
  .then(function (oAuthResponse) {
    axios.get('https://discord.com/api/users/@me', { 'headers': {'Authorization': 'Bearer ' + oAuthResponse.data.access_token} })
    .then(function(user) {
      console.log(user.data.id);
      axios.post('51.15.222.156/api/User/', {
        'userHash': req.query.state,
        'discordId': user.data.id
      })
      .then(function(response) {
        res.redirect("https://www.thesubath.com/bcss/");
      })
      .catch(function(error) {
        res.send(errorText);
      });
    })
    .catch (function(error) {
      res.send(errorText);
    });
  })
  .catch (function(error) {
    res.send(errorText);
  });
});

module.exports = router;