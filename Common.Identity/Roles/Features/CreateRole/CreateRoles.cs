using Ardalis.GuardClauses;
using Common.Abstractions.Core;
using Common.Abstractions.CQRS;
using Common.Abstractions.EFCoreConnection;
using Common.Abstractions.Messaging;
using Common.Core.Exceptions;
using Common.Core.IdsGenerator;
using Common.Identity.Shared.Data;
using Common.Identity.Shared.Models;
using Common.Identity.Users.Features.RegisteringUser;
using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Common.Identity.Roles.Features.CreateRole;

public record CreateRoles(string Name) : IRequest<CreateRoleResponse>
{
   // public long Id { get; init; } = SnowFlakIdGenerator.NewId();
}

internal class CreateRolesValidator : AbstractValidator<CreateRoles>
{
    public CreateRolesValidator()
    {


        RuleFor(v => v.Name)
            .NotEmpty()
            .WithMessage("Name is required.");
    }
}

internal class CreateRolesHandler : IRequestHandler<CreateRoles, CreateRoleResponse>
{
    private readonly ISecurityContextAccessor _securityContextAccessor;
    private readonly IdentityContext _context;
    private readonly IBus _bus;
    private readonly IDbConnectionCreator _connectionFactory;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateRolesHandler(
        RoleManager<ApplicationRole> roleManager,
        ISecurityContextAccessor securityContextAccessor,
        IdentityContext context,
        IBus bus,
        IDbConnectionCreator connectionFactory)
    {
        _securityContextAccessor = securityContextAccessor;
        _context = context;
        _bus = bus;
        _connectionFactory = connectionFactory;
        _roleManager = Guard.Against.Null(roleManager, nameof(roleManager));
    }

    public async Task<CreateRoleResponse> Handle(CreateRoles request, CancellationToken cancellationToken)
    {
        //var tenantId = request.TenantId;

        //if (_securityContextAccessor.IsAuthenticated)
        //{
        //    tenantId = await _context.Tenants.Where(t => t.ApiKey == _securityContextAccessor.ApiKey)
        //        .Select(t => t.Name).FirstOrDefaultAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        //}

        //if (!_securityContextAccessor.Permissions.Contains(IdentityModulePermissions.ROLECREATE, StringComparer.Ordinal))
        //    throw new ApiException("Access Permission Denied.", System.Net.HttpStatusCode.Forbidden);

        using (var con = _connectionFactory.GetOrCreateConnection())
        {
            var userId = Convert.ToInt64(_securityContextAccessor.UserId);

            var tenatId = _securityContextAccessor.TenantId;
            //if (string.IsNullOrEmpty(tenatId))
            //    tenatId = await con.QueryFirstOrDefaultAsync<string>($"SELECT tenant_id from [users].[asp_net_users] where id = @userId;", new { userId });
            //if (string.IsNullOrEmpty(tenatId))
            //    tenatId = "Main";

            if (await _roleManager.RoleExistsAsync(request.Name.Trim()))
                throw new ApiException(request.Name + " already exists.", System.Net.HttpStatusCode.Conflict);

            const string primary_key_sql = "SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END FROM [users].[asp_net_roles] where id = @id;";
            long PrimaryId = CurrentDateTimeCountIdGenerator.Id();
            long incrementalValue = 1;
            var isPKExists = await con.QueryFirstOrDefaultAsync<bool>(primary_key_sql, new { id = PrimaryId });
            while (isPKExists)
            {
                PrimaryId += incrementalValue;
                isPKExists = await con.QueryFirstOrDefaultAsync<bool>(primary_key_sql, new { id = PrimaryId });
                incrementalValue++;
            }

            ApplicationRole applicationRole = new ApplicationRole()
            {
                //Id = PrimaryId,
                Name = request.Name.Trim(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            var identityRole = await _roleManager.CreateAsync(applicationRole);

            if (!identityRole.Succeeded)
                throw new RegisterIdentityUserException(string.Join(',', identityRole.Errors.Select(e => e.Description)));

            //var newRole = _roleManager.FindByIdAsync(request.Id.ToString());

            //Claim claim = new Claim("role", "v");
            //// _roleManager.AddClaimAsync(newRole,)

            return new CreateRoleResponse(PrimaryId);
        }
    }
}
