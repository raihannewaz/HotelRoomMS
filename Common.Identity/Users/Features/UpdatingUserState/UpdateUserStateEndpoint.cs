using Asp.Versioning.Conventions;
using BuildingBlocks.Abstractions.Web;
using Common.Identity.Users.Features.RegisteringUser;

namespace Common.Identity.Users.Features.UpdatingUserState;

public static class UpdateUserStateEndpoint
{
    internal static IEndpointRouteBuilder MapUpdateUserStateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut($"{UsersConfigs.UsersPrefixUri}/{{userId}}/state", UpdateUserState)
            .RequireAuthorization()
            .WithTags(UsersConfigs.Tag)
            .Produces<RegisterUserResponse>(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("UpdateUserState")
            .WithDisplayName("Update User State.")
            .WithApiVersionSet(UsersConfigs.VersionSet)
            .HasApiVersion(1.0);

        return endpoints;
    }

    private static Task<IResult> UpdateUserState(
        long userId,
        UpdateUserStateRequest request,
        IGatewayProcessor<IdentityModuleConfiguration> gatewayProcessor,
        CancellationToken cancellationToken)
    {
        return gatewayProcessor.ExecuteCommand(async commandProcessor =>
        {
            var command = new UpdateUserState(userId, request.UserState);

            await commandProcessor.SendAsync(command, cancellationToken);

            return Results.NoContent();
        });
    }
}
