using SampleWebApi.CommonService;
using SampleWebApi.Interface;
using SampleWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApi.Service
{
    public class LoginService
    {
        private readonly DapperService dapperService;
        public LoginService(DapperService dapperService)
        {
            this.dapperService = dapperService;
        }      

        public LoginModel Login(LoginModel login)
        {
            //if (login.UserName !=null && login.Password != null)
            //{
            //    return this.dapperService.FirstOrDefault<LoginModel>("select * from tbl_user where username=@userName and password=@password", new { userName= login.UserName,password=login.Password });
            //}
            return this.dapperService.FirstOrDefault<LoginModel>("select * from tbl_user where username=@userName and password=@password", new { userName = login.UserName, password = login.Password });

        }
    }
}