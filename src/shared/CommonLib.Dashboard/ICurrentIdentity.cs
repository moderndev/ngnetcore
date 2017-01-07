using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonLib.Dashboard
{
    public interface ICurrentIdentity
    {
        Guid IdentityId { get; }
        Guid PersonId { get; }
        string UserName { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        bool IsAccountVerified { get; }
        IEnumerable<string> Roles { get; }
        DateTimeOffset PasswordExpiryDate { get; }
        bool TwoStepVerificationTurnedOn { get; }
        bool TwoStepVerificationUsed { get; }
        DateTime? Date2SVOn { get; }
    }
}
