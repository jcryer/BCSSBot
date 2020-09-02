using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCSSBot.API.Models;
using BCSSBot.Database.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;

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
                var permissions = _db.Users.Where(x => x.UserHash == userUpdate.UserHash)?.SelectMany(x => x.Memberships)?.Select(x => x.Permission)?.ToArray();

                //var query = _db.Users.Join(_db.Memberships, id => id.UserHash,)
                /*
                var query = (from u in _db.Users
                             join m in _db.Memberships
                             on u.UserHash equals m.UserHash
                             join p in _db.Permissions
                             on m.Id equals p.Id
                             where u.UserHash == userUpdate.UserHash

                             select new
                             {
                                 discordId = u.DiscordId,
                                 permissions = p.JsonBlob
                             }).ToList();
                */               

                if (user != null)
                {
                    user.DiscordId = userUpdate.DiscordId;

                    _callbackHolder.Callback(userUpdate.DiscordId, permissions ?? new Permission[0]);

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