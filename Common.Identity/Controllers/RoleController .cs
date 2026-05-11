using Ardalis.GuardClauses;
using Common.Abstractions.CQRS;
using Common.Identity.Roles;
using Common.Identity.Roles.Features.AddRoleClaims;
using Common.Identity.Roles.Features.CreateRole;
using Common.Identity.Roles.Features.DeleteRole;
using Common.Identity.Roles.Features.GetAllPermissions;
using Common.Identity.Roles.Features.GetAllRolesForUser;
using Common.Identity.Roles.Features.GetRoleById;
using Common.Identity.Roles.Features.GettingRoles;
using Common.Identity.Roles.Features.UpdateRole;
using Common.Identity.Roles.Features.UpdateRoleClaims;
using Common.Identity.Users.Features.GettingUsers;

namespace Common.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ISender sender;

        public RoleController(ISender sender)
        {
            this.sender = sender;
        }



        [HttpGet()]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {

            var command = new GetRole();

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissions(CancellationToken cancellationToken)
        {
            var command = new GetPermissionList();

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpGet("user-permissions")]
        public async Task<IActionResult> GetPermissionforuser(CancellationToken cancellationToken)
        {
            var command = new GettingAllRolesForUser();

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(long id, CancellationToken cancellationToken)
        {
            Guard.Against.Null(id, nameof(id));

            var command = new GetRoleById(id);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpPost("grid")]
        public async Task<IActionResult> GetRolesGrid(GetRolesRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = RolesMapper.GetRequestMap(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPost("permissions")]
        public async Task<IActionResult> AddRoleClaims(AddRoleClaimsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new AddRoleClaim(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new CreateRoles(request.Name);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole(UpdateRoleRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new UpdateRoles(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut("permissions")]
        public async Task<IActionResult> UpdateRoleClaim(UpdateRoleClaimsRequest request, CancellationToken cancellationToken)
        {
            Guard.Against.Null(request, nameof(request));

            var command = new UpdateRoleClaim(request);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long Id, CancellationToken cancellationToken)
        {
            var command = new DeleteRoles(Id);

            var result = await sender.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}



