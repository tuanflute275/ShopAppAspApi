using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApp.Data;
using ShopApp.DTO;
using ShopApp.Models.Entities;
using ShopApp.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new ResponseObject(400, "Invalid request."));
            }
            try
            {
                var checkUser = await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == dto.Email);

                if (checkUser == null)
                {
                    return BadRequest(new ResponseObject(400, "Account does not exist."));
                }

                if (dto.Password.Length < 6)
                {
                    return BadRequest(new ResponseObject(400, "Password must be longer than 6 characters."));
                }

                if (!BCrypt.Net.BCrypt.Verify(dto.Password, checkUser.UserPassword))
                {
                    return BadRequest(new ResponseObject(400, "Incorrect password."));
                }

                // Tạo token JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("tuanflute275aspwebapishopapp0000");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, checkUser.Id.ToString()),
                        new Claim(ClaimTypes.Email, checkUser.UserEmail),
                       
                    }),
                    Expires = DateTime.UtcNow.AddHours(1), 
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new ResponseObject(200, "Login successfully.", new { Token = tokenString }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new ResponseObject(400, "Invalid request."));
            }
            try
            {
                var checkUserName = _context.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserName);
                var checkEmail = _context.Users.FirstOrDefaultAsync(x => x.UserEmail == dto.Email);
                if (checkUserName != null) {
                   if(checkEmail != null)
                    {
                        if(dto.Password.Length >= 6)
                        {
                            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, 12);
                            User user = new User
                            {
                                UserName = dto.UserName,
                                UserFullName = dto.FullName,
                                UserEmail = dto.Email,
                                UserPassword = passwordHash
                            };
                            await _context.Users.AddAsync(user);
                            _context.SaveChanges();
                            return Ok(new ResponseObject(200, "Register successfully", null));
                        }
                        else
                        {
                            return BadRequest(new ResponseObject(400, "Password must be longer than 6 characters !"));
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseObject(400, "Email already taken"));
                    }
                }
                else
                {
                    return BadRequest(new ResponseObject(400, "Username already taken"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }
    }
}
