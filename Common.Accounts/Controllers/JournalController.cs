using System.IdentityModel.Tokens.Jwt;
using AuthSystem.Identity.DTOs;
using AuthSystem.Identity.Models;
using AuthSystem.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Common.Accounts.Controllers;

[ApiController]
[Route("api/journal")]
[Authorize]
public class JournalController() : ControllerBase
{
}
