using TestProject.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ApplicationDBContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
           
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Name
                
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));
            if (!await roleManager.RoleExistsAsync(UserRoles.User))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await roleManager.RoleExistsAsync(UserRoles.User))
            {
                await userManager.AddToRoleAsync(user, UserRoles.User);
            }

            //Send Email
            Email_Sent(model.Email,  model.Name);
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        int Email_Sent(string emailTo, string username)
        {
            int isSent = 0;
            try
            {
               
                string html = @"<html><body><div><center><table border='0' cellspacing='0' cellpadding='0' align='center' width='520' bgcolor='#ffffff' style='background:#ffffff;min-width:520px'>" +
                                "<tbody><tr><td height='15'></td></tr>" +
                                "<tr><td width='480' style='border:2px solid rgb(243, 170, 37);border-radius:16px;'>" +
                                "<table border='0' cellspacing='0' cellpadding='0' width='100%'><tbody>" +
                                "<tr><td height='40px'></td></tr>" +
                                "<tr><td><table border='0' cellspacing='0' cellpadding='0' align='center' width='100%'>" +
                                "<tbody><tr><td align='center' style='color:#202124;font-family:Google Sans,&quot;Roboto&quot;,Arial;font-size:22px;font-weight:normal;line-height:45px;margin:0;padding:0 80px 0 80px;text-align:center;word-break:normal;direction:ltr' dir='ltr'>Hi " + username + ",</td>" +
                                "</tr><tr><td height='15' style='line-height:4px;font-size:4px'></td></tr><tr>" +
                                "<td align='center' style='color:#3c4043;font-family:&quot;Roboto&quot;,OpenSans,&quot;Open Sans&quot;,Arial,sans-serif;font-size:16px;font-weight:normal;line-height:24px;margin:0;padding:0 70px 0 70px;text-align:center;word-break:normal;direction:ltr' dir='ltr'> Thanks for registration.</td>" +
                                "</tr><tr><td height='10px'></td></tr>" +
                                "<tr>" +
                                "<td valign='middle' style='color:#777;font-family:&quot;Roboto&quot;,OpenSans,&quot;Open Sans&quot;,Arial,sans-serif;font-size:10px;line-height:14px;margin:0;padding:0 70px 0 70px;text-align:center' align='center'>XYZ Block,Floor 12<br><br><img src=''/><span style='word-break:normal;direction:ltr' dir='ltr'>&copy; This email was sent to you because you are registring.</span></td>" +
                                "</tr></tbody></table></td></tr><tr><td height='18'></td></tr>" +
                                "</tbody></table></center></div></body></html>";

                Email email = new Email();
                isSent = email.SendMail("User Registered Successfully", html, emailTo);

            }
            catch (Exception ex)
            {

            }
            return isSent;
        }

        
    }
}
