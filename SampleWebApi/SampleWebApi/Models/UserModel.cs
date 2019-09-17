using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleWebApi.Models
{
    public class UserModel
    {
        public string FirstName { get; set; }

        public string SurName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string MobileNo { get; set; }

        public string EmailId { get; set; }
    }
}