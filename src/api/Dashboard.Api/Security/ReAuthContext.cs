using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard.Api.Security
{
    public sealed class ReAuthContext
    {
        public const string ReAuthContextKey = "OIDC-ReAuthContext";
        public readonly string LoginHint;
        public readonly string Prompt;
        public readonly bool LiteVersion;

        public ReAuthContext(string loginHint, string prompt, bool liteVersion = false)
        {
            LoginHint = loginHint;
            Prompt = prompt;
            LiteVersion = liteVersion;
        }

        public enum ReAuthType
        {
            None,
            ChangePassword,
            ChangeUsername,
            RequestChangeUsername,
            ResetPassword,
            RequestSetup2SV,
            RequestReAuthenticate

        }
    }
}
