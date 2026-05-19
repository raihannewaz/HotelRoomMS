using Ardalis.GuardClauses;
using AutoMapper;
using Common.Abstractions.CQRS;
using Common.Accounts.Features.COAGroups.CreateCOAGroups;
using Common.Accounts.Features.COAGroups.GetCOAGroupsById;
using Common.Accounts.Features.COAGroups.GetCOAGroupsGrid;
using Common.Accounts.Features.COAGroups.UpdateCOAGroups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Common.Accounts.Controllers;

[ApiController]
[Route("api/coagroup")]
[Authorize]
public class GroupController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public GroupController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCOAGroupRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new CreateCOAGroup(request);
        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }



    [HttpPut]
    public async Task<IActionResult> Update(UpdateCOAGroupRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = new UpdateCOAGroup(request);

        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }


    [HttpPost("get/coagroup/grid")]

    public async Task<IActionResult> GetAll(GetCOAGroupGridResquest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        var command = _mapper.Map<GetCOAGroupGrid>(request);

        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
        
    }

    [HttpGet("{Id}")]
    public async Task<IActionResult> GetAll(long Id, CancellationToken cancellationToken)
    {
        Guard.Against.Null(Id, nameof(Id));

        var command = new GetCOAGroupById(Id);

        var result = await _sender.Send(command, cancellationToken);

        return Ok(result);
    }
}
