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
        private readonly CallbackHolder _callbackHolder;
        public UserController(CallbackHolder callbackHolder)
        {
            _callbackHolder = callbackHolder;
        }

        // One Api control
        // Puts the discord id into the database
        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] UserUpdate userUpdate)
        {
            var db = Settings.GetSettings().BuildContext();

            Console.WriteLine($"PUT REQUEST {{User:\n" +
                             $"userHash: {userUpdate.UserHash}\n" +
                             $"discordId: {userUpdate.DiscordId}}}");

            // Dont know why this would ever fire, I am afraid of it so I'm gonna leave it in
            if (!ModelState.IsValid) 
            {
                Console.WriteLine("API put request. Invalid model? Not sure why this would fire! Someones having problems.");
                return BadRequest();
            }
            
            var user = db.Users.FirstOrDefault(u => u.UserHash == int.Parse(userUpdate.UserHash));

            // If the user cant be found then return a bad request
            if (user == null)
            {
                Console.WriteLine("API Put request with BAD USER (SOMETHING IS VERY WRONG)");
                return BadRequest();
            }
                
            var permissions = db.Users.Where(x => x.UserHash == int.Parse(userUpdate.UserHash))?.SelectMany(x => x.Memberships)?.Select(x => x.Permission)?.ToArray();
                    
            user.DiscordId = ulong.Parse(userUpdate.DiscordId);
            user.Email = "";

            _callbackHolder.Callback(ulong.Parse(userUpdate.DiscordId), permissions ?? new Permission[0]);

            db.Users.Update(user);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            return Ok();
        }
    }
}