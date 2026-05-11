using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Identity.Identity.Users.Features.GettingUserById;
using Common.Identity.Users;
using Common.Identity.Users.Features.ChangePassword;
using Common.Identity.Users.Features.ChangePassword.Request;
using Common.Identity.Users.Features.DeleteUser;
using Common.Identity.Users.Features.GettingUerByEmail;
using Common.Identity.Users.Features.GettingUsers;
using Common.Identity.Users.Features.RegisteringUser;
using Common.Identity.Users.Features.UpdateUser;
using Common.Identity.Users.Features.UpdateUser.Request;

namespace Common.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ISender sender;

        public UserController(ISender sender)
        {
            this.sender = sender;
        }



        [HttpPost("grid")]
        public async Task<IActionResult> GetGridView(GetUsersRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = UsersMapper.GetRequestMap(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpGet()]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {

            var command = new GettingUsers();

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            Guard.Against.NegativeOrZero(id, nameof(id));

            var command = new GetUserById(id);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
        {
            Guard.Against.NullOrEmpty(email, nameof(email));

            var command = new GetUserByEmail(email);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new RegisterUser(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut()]
        public async Task<IActionResult> Update(UpdateUserRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new UpdateUser(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePasswords(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new ChangePassword(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long Id, CancellationToken cancellationToken)
        {
            var command = new DeleteUser(Id);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
