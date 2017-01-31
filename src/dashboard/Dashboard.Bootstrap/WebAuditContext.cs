using CommonLib;
using CommonLib.Dashboard;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard
{
    public interface IAuditContext
    {
        string ClientIp { get; }
        IDictionary<string, string> AdditionalData { get; }
        Guid PersonId { get; }
    }

    public class WebAuditContext : IAuditContext
    {
        private readonly string[] _headersToStore = { "X-Forwarded-For", "X-Forwarded-Host", "X-Forwarded-Proto", "X-Original-For", "X-Original-Host", "X-Original-Proto", "User-Agent" };

        public string ClientIp { get; }

        public IDictionary<string, string> AdditionalData { get; } = new Dictionary<string, string>();

        public Guid PersonId { get; }

        public WebAuditContext(ICurrentIdentity identity, IHttpContextAccessor httpContextAccessor)
        {
            Args.NotNull(httpContextAccessor, nameof(httpContextAccessor));
            Args.NotNull(identity, nameof(identity));

            ClientIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString();
            PersonId = identity.PersonId;

            foreach (var header in _headersToStore)
            {
                AddHeader(header, httpContextAccessor);
            }
        }

        private void AddHeader(string name, IHttpContextAccessor httpContextAccessor)
        {
            var headerValue = httpContextAccessor.HttpContext?.Request.Headers[name];
            if (!String.IsNullOrEmpty(headerValue)) { AdditionalData.Add(name, headerValue); }
        }
    }
}
