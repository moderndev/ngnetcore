using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CommonLib.Dashboard.Model
{
    public class Person
    {
        public bool IsAccountVerified { get; private set; }
        public bool IsAdministrator { get; private set; }

        public Person(string uniqueid, string role) {
            IsAccountVerified = true;
            if (role == "admin") IsAdministrator = true;
        }
    }
}
