using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BCSSBot.API.DataAccess;
using BCSSBot.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BCSSBot.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CoreContainer _coreContainer;
        private PostgresSqlContext db;
        public UserController(CoreContainer coreContainer)
        {
            _coreContainer = coreContainer;
            db = coreContainer.Program.GlobalContextBuilder.CreateContext();
        }

        [HttpPut]
        public IActionResult PutUser([FromBody] UserUpdate userUpdate)
        {
            Console.WriteLine($"User:\n" +
                              $"userHash: {userUpdate.userHash}\n" +
                              $"discordId: {userUpdate.discordId}");

            if (ModelState.IsValid)
            {
                var user = db.Users.First(u => u.UserHash == userUpdate.userHash);
                user.DiscordId = userUpdate.discordId;
                db.Users.Update(user);
                db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }
}