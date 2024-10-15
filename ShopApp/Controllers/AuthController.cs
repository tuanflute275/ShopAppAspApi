using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApp.Data;
using ShopApp.Models.Entities;
using ShopApp.Models.ViewModels;
using ShopApp.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ShopApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, IConfiguration _configuration)
        {
            _context = context;
            this._configuration = _configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest(new ResponseObject(400, "Invalid request."));
            }
            try
            {
                var checkUser = await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == model.Email);

                if (checkUser == null)
                {
                    return BadRequest(new ResponseObject(400, "Account does not exist."));
                }

                if (model.Password.Length < 6)
                {
                    return BadRequest(new ResponseObject(400, "Password must be longer than 6 characters."));
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, checkUser.UserPassword))
                {
                    return BadRequest(new ResponseObject(400, "Incorrect password."));
                }

                //select user role get roleName
                var userWithRole = await (from ur in _context.UserRoles
                                          join u in _context.Users on ur.UserId equals u.Id
                                          join r in _context.Roles on ur.RoleId equals r.Id
                                          where ur.UserId == 2
                                          select new
                                          {
                                              UserId = u.Id,
                                              UserEmail = u.UserEmail,
                                              RoleName = r.RoleName
                                          }).FirstOrDefaultAsync();
                if (userWithRole != null)
                {
                    // Tạo token JWT
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_configuration["Jwt:key"]);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim(ClaimTypes.NameIdentifier, checkUser.Id.ToString()),
                        new Claim(ClaimTypes.Email, checkUser.UserEmail),
                        new Claim(ClaimTypes.Role, userWithRole.RoleName)

                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        Issuer = _configuration["Jwt:Issuer"],
                        Audience = _configuration["Jwt:Audience"],
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    return Ok(new ResponseObject(200, "Login successfully.", new { Token = tokenString }));
                }
                else
                {
                    return Ok(new ResponseObject(400, "You do not have access."));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest(new ResponseObject(400, "Invalid request."));
            }
            if (!AccountEmailExists(model.Email))
            {
                return BadRequest(new ResponseObject(400, "Email already taken"));
            }
            if (!AccountUserNameExists(model.UserName))
            {
                return BadRequest(new ResponseObject(400, "Username already taken"));
            }
            if(model.Password.Length <= 6)
            {
                return BadRequest(new ResponseObject(400, "Password must be longer than 6 characters !"));
            }
            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, 12);
                User user = new User
                {
                    UserName = model.UserName,
                    UserFullName = model.FullName,
                    UserEmail = model.Email,
                    UserPassword = passwordHash
                };
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                // lấy userId vừa tạo
                var userId = user.Id;
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == model.Role);
                if (role != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = userId,
                        RoleId = role.Id
                    };

                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                }
                // Gửi email xác nhận đăng ký
                EmailModel emailModel = new EmailModel()
                {
                    Subject = "Register Account",
                    To = user.UserEmail
                };

                using (MailMessage mm = new MailMessage(emailModel.From, emailModel.To))
                {
                    mm.Subject = emailModel.Subject;
                    mm.Body = BodyRegisterMail(user.UserFullName);
                    mm.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(emailModel.From, emailModel.Password);
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
                return Ok(new ResponseObject(200, "Register successfully,please check email!", model));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject(500, "Internal server error. Please try again later."));
            }
        }

        [HttpPost("forgotPassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (AccountEmailExists(forgotPasswordModel.UserEmail))
            {
                var acc = _context.Users.FirstOrDefault(x => x.UserEmail == forgotPasswordModel.UserEmail);
                var pass = CreateRandomPassword(8);
                acc.UserPassword = BCrypt.Net.BCrypt.HashPassword(pass, 12);
                _context.Update(acc);
                await _context.SaveChangesAsync();
                EmailModel model = new EmailModel()
                {
                    Subject = "Forgot Password",
                    To = forgotPasswordModel.UserEmail
                };
                using (MailMessage mm = new MailMessage(model.From, model.To))
                {
                    mm.Subject = model.Subject;
                    mm.Body = BodyResetPasswordMail(pass);
                    mm.IsBodyHtml = true;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential(model.From, model.Password);
                        smtp.UseDefaultCredentials = false;
                        smtp.EnableSsl = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
                return Ok(new ResponseObject(200, "Account registration successful, please check email!", pass));
            }
            else
            {
                return BadRequest(new ResponseObject(404, "Account does not exitst"));
            }
        }

        [HttpPost("changePassword/{userId}")]
        public async Task<ActionResult> ChangePassword(int userId, ChangePassModel model)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    if (BCrypt.Net.BCrypt.Verify(model.OldPassword,user.UserPassword))
                    {
                        if (BCrypt.Net.BCrypt.Verify(model.NewPassword,user.UserPassword))
                        {
                            return BadRequest(new ResponseObject(400, "New password cannot be the same as current password!"));
                        }
                        else
                        {
                            if (model.NewPassword == model.ConfirmPassword)
                            {
                                user.UserPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 12);
                                _context.Entry(user).State = EntityState.Modified;
                                await _context.SaveChangesAsync();
                                return Ok(new ResponseObject(200, "Password changed successfully!"));
                            }
                            else
                            {
                                return BadRequest(new ResponseObject(400, "Confirm password is incorrect"));
                            }
                        }
                    }
                    else
                    {
                        return BadRequest(new ResponseObject(400, "Current password is incorrect"));
                    }
                }
            }
            else
            {
                return BadRequest(new ResponseObject(400, "Account does not exitst"));
            }
            return Ok(new ResponseObject(200, "Password changed successfully!"));
        }

        public static string BodyResetPasswordMail(string pass)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Utils\\Mail", "ForgotPasswordMail.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{Password}}", pass);
            return body;
        }
        public static string BodyRegisterMail(string fullName)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "Utils\\Mail", "RegisterSuccessMail.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{{fullName}}", fullName);
            return body;
        }
        private bool AccountEmailExists(string email)
        {
            return _context.Users.Any(e => e.UserEmail == email);
        }
        private bool AccountUserNameExists(string username)
        {
            return _context.Users.Any(e => e.UserName == username);
        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
    }
}
