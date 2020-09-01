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
        public UserController()
        {
            _db = Settings.getSettings().CreateContextBuilder().CreateContext();
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
                   
                    // TODO: CHANGE
                    //var worked = await _coreContainer.Program.Bot.ModifyUser((ulong)user.DiscordId, user.Memberships.Select(x => x.Permission).ToArray());
                    
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