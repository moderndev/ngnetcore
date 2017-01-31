using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Security
{
    public class Roles
    {
        public const string NonVerifiedUser = "NonVerifiedUser";
        public const string User = "User";
        public const string Admin = "Admin";
        public const string UserFromToken = "UserFromToken";
    }
}
