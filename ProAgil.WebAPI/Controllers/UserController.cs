using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProAgil.Domain.Identity;
using ProAgil.WebAPI.Dtos;

namespace ProAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IMapper _mapper { get; }
        public UserManager<User> _userManager { get; }
        public IConfiguration _config { get; }
        public SignInManager<User> _signInManager { get; }
        public UserController(IConfiguration config,
                                UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                IMapper mapper)
        {
            this._signInManager = signInManager;
            this._config = config;
            this._userManager = userManager;
            this._mapper = mapper;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            return Ok(new UserDto());
        }

        [HttpPost("Register")]
        [AllowAnonymous] // <- Not necessary authentication to this method.
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                //Mapping to pass userDto data to the Entity User.
                var user = _mapper.Map<User>(userDto);
                //Creates a user.
                var result = await _userManager.CreateAsync(user, userDto.Password);
                //Returns to view only the necessary, that's the reason this following mapping occurs.
                var userToReturn = _mapper.Map<UserDto>(user);

                if (result.Succeeded)
                {
                    //Returns 201 calling the GetUser passing the userToReturn.
                    return Created("GetUser", userToReturn);
                }
                return BadRequest(result.Errors);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de DadosFalhou {ex.Message}");
            }
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                //Checks if the userName is registered.
                var user = await _userManager.FindByNameAsync(userLoginDto.UserName);
                //Checks if the password matches de userName and sets false to the block option in case of failure.
                var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);

                if (result.Succeeded)
                {
                    //If the login succeds we get the very user found
                    var appUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.NormalizedUserName == userLoginDto.UserName.ToUpper());

                    //Maps the appUser to UserLoginDto shape
                    var userToReturn = _mapper.Map<UserLoginDto>(appUser);

                    //This will be returned in the body
                    return Ok(new
                    {
                        token = GenerateJwtToken(appUser).Result,
                        user = userToReturn
                    });
                }
                return Unauthorized();
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de DadosFalhou {ex.Message}");
            }
        }

        //Build a token to user
        private async Task<string> GenerateJwtToken(User user)
        {
            //claims is about permissions
            var claims = new List<Claim>{
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            //Gets all the roles related to the user
            var roles = await _userManager.GetRolesAsync(user);

            //Creates a new claim following each role
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //the way the crypto is made
            var key = new SymmetricSecurityKey(Encoding.ASCII.
                  GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}