using System;
using System.Linq;
using System.Threading.Tasks;
using BCSSBot.API.Models;
using BCSSBot.Database.DataAccess;
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
            _db = Settings.GetSettings().CreateContextBuilder().CreateContext();
            _callbackHolder = callbackHolder;
        }

        [HttpPut]
        public async Task<IActionResult> PutUser([FromBody] UserUpdate userUpdate)
        {
            Console.WriteLine($"User:\n" +
                              $"userHash: {userUpdate.UserHash}\n" +
                              $"discordId: {userUpdate.DiscordId}");
            
            if (ModelState.IsValid)
            {
                var user = _db.Users.FirstOrDefault(u => u.UserHash == userUpdate.UserHash);
                if (user != null)
                {
                    user.DiscordId = userUpdate.DiscordId;

                    Console.WriteLine(user.Memberships.Count());
                    _callbackHolder.Callback(user.DiscordId, user.Memberships.Select(x => x.Permission).ToArray());

                    _db.Users.Update(user);
                    await _db.SaveChangesAsync();
                    return Ok();
                }
                return BadRequest();
            }
            return BadRequest();
        }
    }
}