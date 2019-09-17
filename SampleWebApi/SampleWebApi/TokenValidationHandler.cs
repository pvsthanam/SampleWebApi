using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
namespace SampleWebApi
{
    internal class TokenValidationHandler : DelegatingHandler
    {

        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;
            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isCorsRequest = request.Headers.Contains(Origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            var origin = request.Headers.GetValues(Origin).First();

            string token = null;
            HttpStatusCode statusCode;

            if (isCorsRequest)
            {
                if (!TryRetrieveToken(request, out token))
                {
                    if (isPreflightRequest)
                    {
                        return Task.Factory.StartNew(() =>
                       {
                           HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                           response.Headers.Add(AccessControlAllowOrigin, origin);

                           string accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                           if (accessControlRequestMethod != null)
                           {
                               response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                           }

                           string requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
                           if (!string.IsNullOrEmpty(requestedHeaders))
                           {
                               response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                           }

                           return response;
                       }, cancellationToken);
                    }
                }
                try
                {

                    const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
                    var now = DateTime.UtcNow;
                    var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));

                    SecurityToken securityToken;
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    TokenValidationParameters validationParameters = new TokenValidationParameters()
                    {
                        ValidAudience = "http://localhost:62361",
                        ValidIssuer = "http://localhost:62361",
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        LifetimeValidator = this.LifetimeValidator,
                        IssuerSigningKey = securityKey
                    };
                    //extract and assign the user of the jwt
                    Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
                    HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out securityToken);

                    //return base.SendAsync(request, cancellationToken);
                    return base.SendAsync(request, cancellationToken).ContinueWith(t =>
                    {
                        t.Result.Headers.Add(AccessControlAllowOrigin, origin);
                        return t.Result;
                    }, cancellationToken);
                }
                catch (SecurityTokenValidationException e)
                {
                    statusCode = HttpStatusCode.Unauthorized;
                }
                catch (Exception ex)
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }
            }
            //return base.SendAsync(request, cancellationToken);
            return base.SendAsync(request, cancellationToken).ContinueWith(t =>
            {
                t.Result.Headers.Add(AccessControlAllowOrigin, origin);
                return t.Result;
            }, cancellationToken);

        }

        //protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        //{
        //    HttpStatusCode statusCode;
        //    string token;
        //    //determine whether a jwt exists or not
        //    if (!TryRetrieveToken(request, out token))
        //    {
        //        statusCode = HttpStatusCode.Unauthorized;
        //        //allow requests with no token - whether a action method needs an authentication can be set with the claimsauthorization attribute
        //        return base.SendAsync(request, cancellationToken);
        //    }

        //    try
        //    {
        //        const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
        //        var now = DateTime.UtcNow;
        //        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));


        //        SecurityToken securityToken;
        //        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        //        TokenValidationParameters validationParameters = new TokenValidationParameters()
        //        {
        //            ValidAudience = "http://localhost:62361",
        //            ValidIssuer = "http://localhost:62361",
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            LifetimeValidator = this.LifetimeValidator,
        //            IssuerSigningKey = securityKey
        //        };
        //        //extract and assign the user of the jwt
        //        Thread.CurrentPrincipal = handler.ValidateToken(token, validationParameters, out securityToken);
        //        HttpContext.Current.User = handler.ValidateToken(token, validationParameters, out securityToken);

        //        return base.SendAsync(request, cancellationToken);
        //    }
        //    catch (SecurityTokenValidationException e)
        //    {
        //        statusCode = HttpStatusCode.Unauthorized;
        //    }
        //    catch (Exception ex)
        //    {
        //        statusCode = HttpStatusCode.InternalServerError;
        //    }
        //    return Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage(statusCode) { });
        //}

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }

    }
}