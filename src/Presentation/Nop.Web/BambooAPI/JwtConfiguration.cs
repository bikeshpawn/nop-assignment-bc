using Nop.Core.Configuration;

namespace Nop.Web.BambooAPI;

public class JwtConfiguration : IConfig
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Key { get; set; }
}
