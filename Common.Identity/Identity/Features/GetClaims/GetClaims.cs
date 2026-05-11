using Common.Abstractions.CQRS;

namespace Common.Identity.Identity.Features.GetClaims;

public class GetClaims : IRequest<GetClaimsResponse>
{
}

public class GetClaimsHandler : IRequestHandler<GetClaims, GetClaimsResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetClaimsHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<GetClaimsResponse> Handle(GetClaims request, CancellationToken cancellationToken)
    {
        var claims = _httpContextAccessor.HttpContext?.User.Claims.Select(x => new ClaimDto
        {
            Type = x.Type, Value = x.Value
        });

        return Task.FromResult(new GetClaimsResponse(claims));
    }
}
