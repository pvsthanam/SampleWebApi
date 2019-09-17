using SampleWebApi.CommonService;
using SampleWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApi.Service
{
    public class UserService
    {
        private readonly DapperService dapperService;
        public UserService(DapperService dapperService)
        {
            this.dapperService = dapperService;
        }

        public IEnumerable<UserModel> GetUsers()
        {
            return this.dapperService.Query<UserModel>("select * from tbl_user");            
        }
    }
}