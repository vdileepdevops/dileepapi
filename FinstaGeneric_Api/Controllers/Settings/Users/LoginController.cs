using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinstaInfrastructure.Settings.Users;
using Microsoft.AspNetCore.Authorization;
using FinstaApi.Authentication;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using FinstaApi.Common;

namespace FinstaApi.Controllers.Settings.Users
{
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]
    
    public class LoginController : ControllerBase
    {
        private IUserService _userService;
        string Con = string.Empty;
        static IConfiguration _iconfiguration;
        private IHostingEnvironment _hostingEnvironment;
        public LoginController(IUserService userService, IConfiguration iconfiguration, IHostingEnvironment hostingEnvironment)
        {
            _userService = userService;
            _iconfiguration = iconfiguration;
            _hostingEnvironment = hostingEnvironment;
            Con = _iconfiguration.GetSection("ConnectionStrings").GetSection("Connection").Value;
        }


        [AllowAnonymous]
        [Route("/api/login")]
        //[HttpPost("authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody]UserAccessDTO userParam)
        {
            var user = _userService.Authenticate(userParam.pUserName, userParam.pPassword, Con);

            if (user == null)
                return Unauthorized();

            return Ok(user);
        }
        [AllowAnonymous]
        [Route("/api/VerifyOtp")]
        //[HttpPost("authenticate")]
        [HttpPost]
        public IActionResult VerifyOtp([FromBody]UserAccessDTO userParam)
        {
            var user = _userService.VerifyOTP(userParam, Con);

            if (user == null)
                return Unauthorized();

            return Ok(user);
        }

        [HttpGet]
        [Route("/api/getAllemployees")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }
    }
}