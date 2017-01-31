using CommonLib.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Security
{
    public class CurrentIdentity : ICurrentIdentity
    {
        private CurrentIdentity()
        {
            PersonId = new Guid("0FBC27F8-5324-A533-A024-170019F1A494");
            UserName = "anonymous@moderndev.com";
            Email = UserName;
            FirstName = "User";
            LastName = "Anonymous";
            IsAccountVerified = false;
            AuthenticatedAt = DateTime.MinValue;
            Roles = new List<string>();
            TwoStepVerificationTurnedOn = false;
            TwoStepVerificationUsed = false;
        }


        public Guid IdentityId { get; }
        public Guid PersonId { get; }
        public string UserName { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsAccountVerified { get; }
        public DateTime AuthenticatedAt { get; }
        public IEnumerable<string> Roles { get; }
        public DateTimeOffset PasswordExpiryDate { get; }
        public bool TwoStepVerificationTurnedOn { get; }
        public bool TwoStepVerificationUsed { get; }
        public DateTime? Date2SVOn { get; }
    }
}
