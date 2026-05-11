using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Identity.Identity;
using Common.Identity.Identity.Features.GetClaims;
using Common.Identity.Identity.Features.Login;
using Common.Identity.Identity.Features.RefreshingToken;
using Common.Identity.Identity.Features.RevokeRefreshToken;
using Common.Identity.Identity.Features.SendEmailVerificationCode;
using Common.Identity.Identity.Features.VerifyEmail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Common.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly ISender sender;

        public IdentityController(ISender sender)
        {
            this.sender = sender;
        }

        [HttpGet("claims")]
        public async Task<IActionResult> GetClaims(CancellationToken cancellationToken)
        {

            var command = new GetClaims();

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpGet("user-role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Role.User)]
        public IActionResult GetUserRole()
        {
            return Ok(new { Role = Constants.Role.User });
        }

        [HttpGet("admin-role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.Role.Admin)]
        public IActionResult GetAdminRole()
        {
            return Ok(new { Role = Constants.Role.Admin });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new Login(request.UserNameOrEmail, request.Password, request.Remember);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(HttpContext httpContext,CancellationToken cancellationToken)
        {
            await httpContext.SignOutAsync();
            return Ok();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new RefreshToken(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> RevokeToken(RevokeRefreshTokenRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new RevokeRefreshToken(request.RefreshToken);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpPost("send-email-verification-code")]
        public async Task<IActionResult> SendEmailVerificationCode(SendEmailVerificationCodeRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new SendEmailVerificationCode(request.Email);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new VerifyEmail(request.Email, request.Code);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}



