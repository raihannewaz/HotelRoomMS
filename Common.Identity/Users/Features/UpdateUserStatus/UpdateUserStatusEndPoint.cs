using Asp.Versioning.Conventions;
using BuildingBlocks.Abstractions.Web;
using Common.Identity.Users.Features.UpdateUser.Request;
using Common.Identity.Users.Features.UpdateUser.Response;

namespace Common.Identity.Users.Features.UpdateUserStatus;

public static class UpdateUserStatusEndPoint
{
    internal static IEndpointRouteBuilder MapUpdateUserStatusEndPoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"{UsersConfigs.UsersPrefixUri}/active-status", UpdateUserStatus)
            .RequireAuthorization()
            .WithTags(UsersConfigs.Tag)
            .Produces<UpdateUserStatusResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateUserStatus")
            .WithDisplayName("Update User Status")
            .WithApiVersionSet(UsersConfigs.VersionSet)
            .HasApiVersion(1.0)
            .HasApiVersion(2.0);

        return endpoints;
    }

    private static Task<IResult> UpdateUserStatus(
        UpdateUserStatusRequest request,
        IGatewayProcessor<IdentityModuleConfiguration> gatewayProcessor,
        CancellationToken cancellationToken)
    {
        return gatewayProcessor.ExecuteCommand(async commandProcessor =>
        {
            var command = new UpdateUserStatuses(request.Id);
            var result = await commandProcessor.SendAsync(command, cancellationToken);
            return Results.Ok(result);
        });
    }
}
