using SampleWebApi.Models;
using SampleWebApi.Service;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.IO;
//using System.Net;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Web;
//using System.Web.Http;
//using WEBAPI_JWT_Authentication.Models;

namespace SampleWebApi.Controllers
{   
    [RoutePrefix("api/login")]    
    public class LoginController : ApiController
    {
        private readonly LoginService loginService;
        public LoginController(LoginService loginService)
        {
            this.loginService = loginService;
        }

        [HttpPost]
        [Route("submit")]       
        public IHttpActionResult Login(LoginModel login)
        {
           var result= this.loginService.Login(login);
            if (result != null)
            {
                string token = createToken(login.UserName);
                //return the token
                return Ok<string>(token);
                //return Ok();
            }
            return Ok();
        }

        private string createToken(string username)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddMinutes(2);

            //http://stackoverflow.com/questions/18223868/how-to-encrypt-jwt-security-token
            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("username", username),
                new Claim("sudhagar","sudhgar")
            });

            const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var now = DateTime.UtcNow;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


            //create the jwt
            var token =
                (JwtSecurityToken)
                    tokenHandler.CreateJwtSecurityToken(issuer: "http://localhost:62361", audience: "http://localhost:62361",
                        subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
