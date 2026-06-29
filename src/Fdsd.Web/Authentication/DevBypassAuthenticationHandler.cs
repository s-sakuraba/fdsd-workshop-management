using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fdsd.Web.Authentication;

public class DevBypassAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "DevBypass";
    private readonly IConfiguration _configuration;

    public DevBypassAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IConfiguration configuration)
        : base(options, logger, encoder)
    {
        _configuration = configuration;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // appsettings の Auth:BypassAccount を疑似ログイン名として使用
        var account = _configuration["Auth:BypassAccount"];
        if (string.IsNullOrWhiteSpace(account))
        {
            // 未設定時は実行環境のOSユーザー名を利用
            account = Environment.UserName;
        }

        if (string.IsNullOrWhiteSpace(account))
        {
            return Task.FromResult(AuthenticateResult.Fail("Bypass account is not configured."));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account),
            new Claim(ClaimTypes.Name, account)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
