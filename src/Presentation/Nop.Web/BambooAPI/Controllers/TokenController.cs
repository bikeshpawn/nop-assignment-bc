using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Configuration;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.BambooAPI.Dtos;

namespace Nop.Web.BambooAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{

    #region Fields
    protected readonly AppSettings _appSettings;
    protected readonly ICustomerService _customerService;
    private readonly ICustomerRegistrationService _customerRegistrationService;
    private readonly CustomerSettings _customerSettings;
    #endregion

    #region Ctor
    public TokenController(AppSettings appSettings,
        ICustomerService customerService,
        ICustomerRegistrationService customerRegistrationService,
        CustomerSettings customerSettings)
    {
        _appSettings = appSettings;
        _customerService = customerService;
        _customerRegistrationService = customerRegistrationService;
        _customerSettings = customerSettings;
    }
    #endregion


    #region Endpoints


    [HttpPost("GenerateToken")]
    public async Task<IActionResult> GenerateTokenAsync([FromBody] AuthenticationRequestModel loginRequest)
    {
        if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.UserName) || string.IsNullOrWhiteSpace(loginRequest.Password))
            return BadRequest("Username and password must be provided.");

        var customer = await LoginAsync(loginRequest.UserName, loginRequest.Password);

        if (customer is null)
            return StatusCode((int)HttpStatusCode.Forbidden, "Wrong username or password");

        // Load JWT configuration
        var jwtConfigSection = _appSettings.Configuration["JwtConfig"];
        var jwtKey = jwtConfigSection.Value<string>("Key");
        var jwtIssuer = jwtConfigSection.Value<string>("Issuer");
        var jwtAudience = jwtConfigSection.Value<string>("Audience");
        var jwtExpiryMinutes = jwtConfigSection.Value<int>("ExpiresInMinutes");

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwtClaims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, loginRequest.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var jwtToken = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: jwtClaims,
            expires: DateTime.Now.AddMinutes(jwtExpiryMinutes),
            signingCredentials: signingCredentials
        );

        var generatedToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return Ok(new { Token = generatedToken });
    }
    #endregion

    #region Private Methods

    private async Task<Customer> LoginAsync(string username, string password)
    {
        var result = await _customerRegistrationService.ValidateCustomerAsync(username, password);

        if (result == CustomerLoginResults.Successful)
        {
            var customer = await (_customerSettings.UsernamesEnabled
                       ? _customerService.GetCustomerByUsernameAsync(username)
                       : _customerService.GetCustomerByEmailAsync(username));
            return customer;
        }

        return null;
    }

    #endregion


}
