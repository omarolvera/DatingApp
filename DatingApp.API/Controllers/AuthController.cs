using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerContext
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO userRegisterDTO){

            // if(!ModelState.IsValid)
            // return new BadRequestObjectResult(ModelState);

           userRegisterDTO.Username = userRegisterDTO.Username.ToLower();

            if(await _repo.UserExists(userRegisterDTO.Username))
            return new BadRequestObjectResult("Username already exists");

            var userToCreate = new User{
                Username = userRegisterDTO.Username
            };

            var createUser = await _repo.Register(userToCreate, userRegisterDTO.Password);
            return new StatusCodeResult(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLogin) {

            var userFromRepo = await _repo.Login(userForLogin.Username, userForLogin.Password);
            if(userFromRepo == null)
            return new UnauthorizedResult();

             return new StatusCodeResult(201);
        }
    }
}