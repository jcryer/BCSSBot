using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        public UserController()
        {
        }

        [HttpPut]
        public async Task<IActionResult> PutUser(long userHash, User user)
        {
            Console.WriteLine($"User:\n" +
                              $"userHash: {user.UserHash}\n" +
                              $"discordId: {user.DiscordId}");
            
            return Ok();
        }
    }
}