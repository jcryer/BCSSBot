using System;
using System.Linq;
using System.Threading.Tasks;
using BCSSBot.API.Models;
using BCSSBot.Database.DataAccess;
using BCSSBot.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace BCSSBot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PostgresSqlContext _db;
        private readonly CallbackHolder _callbackHolder;
        public UserController(CallbackHolder callbackHolder)
        {
            _db = Settings.GetSettings().BuildContext();
            _callbackHolder = callbackHolder;
        }

        // One Api control
        // Puts the discord id into the database
        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] UserUpdate userUpdate)
        {
            Console.WriteLine($"PUT REQUEST {{User:\n" +
                             $"userHash: {userUpdate.UserHash}\n" +
                             $"discordId: {userUpdate.DiscordId}}}");

            // Dont know why this would ever fire, I am afraid of it so I'm gonna leave it in
            if (!ModelState.IsValid) 
            {
                Console.WriteLine("API put request. Invalid model? Not sure why this would fire! Someones having problems.");
                return BadRequest();
            }
            
            var user = _db.Users.FirstOrDefault(u => u.UserHash == userUpdate.UserHash);

            // If the user cant be found then return a bad request
            if (user == null)
            {
                Console.WriteLine("API Put request with BAD USER (SOMETHING IS VERY WRONG)");
                return BadRequest();
            }
                
            var permissions = _db.Users.Where(x => x.UserHash == userUpdate.UserHash)?.SelectMany(x => x.Memberships)?.Select(x => x.Permission)?.ToArray();
                    
            user.DiscordId = userUpdate.DiscordId;
            user.Email = "";

            _callbackHolder.Callback(userUpdate.DiscordId, permissions ?? new Permission[0]);

            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}