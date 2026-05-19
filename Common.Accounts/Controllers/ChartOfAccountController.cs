using AuthSystem.Identity.DTOs;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Accounts.Controllers;

[ApiController]
[Route("api/chartofaccount")]
[Authorize]
public class ChartOfAccountController() : ControllerBase
{
    
}
