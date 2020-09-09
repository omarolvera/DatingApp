using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerContext
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO userRegisterDTO)
        {

            // if(!ModelState.IsValid)
            // return new BadRequestObjectResult(ModelState);

            userRegisterDTO.Username = userRegisterDTO.Username.ToLower();

            if (await _repo.UserExists(userRegisterDTO.Username))
                return new BadRequestObjectResult("Username already exists");

            var userToCreate = new User
            {
                Username = userRegisterDTO.Username
            };

            var createUser = await _repo.Register(userToCreate, userRegisterDTO.Password);
            return new StatusCodeResult(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLogin)
        {

            var userFromRepo = await _repo.Login(userForLogin.Username.ToLower(), userForLogin.Password);
            if (userFromRepo == null)
                return new UnauthorizedResult();

            var claims = new[]{
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username)

                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor{
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                 return new OkObjectResult(new {
                       token = tokenHandler.WriteToken(token) 
                 });
    }
  }
}