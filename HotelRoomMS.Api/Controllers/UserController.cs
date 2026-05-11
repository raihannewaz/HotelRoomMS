//using Ardalis.GuardClauses;
//using HotelRoomMS.Application.Identities.Users;
//using HotelRoomMS.Application.Identities.Users.Features.CreateUsers;
//using HotelRoomMS.Application.Identities.Users.Features.CreateUsers.Requests;
//using HotelRoomMS.Application.Identities.Users.Features.GettingUsers;
//using HotelRoomMS.Application.Identities.Users.Features.GetUsersById;
//using HotelRoomMS.Application.Identities.Users.Features.UpdateUsers;
//using HotelRoomMS.Application.Identities.Users.Features.UpdateUsers.Requests;
//using Common.Abstractions.CQRS;
//using Common.CustomIdentity.Dto;
//using Common.CustomIdentity.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace HotelRoomMS.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase
//    {
//        private readonly ISender sender;
//        private readonly IConfiguration _configuration;

//        public UserController(ISender sender, IConfiguration configuration)
//        {
//            this.sender = sender;
//            _configuration = configuration;
//        }



//        [HttpPost]
//        public async Task<IActionResult> Create(CreateUserRequest request, CancellationToken cancellationToken)
//        {
//            Guard.Against.Null(request, nameof(request));

//            var command = new CreateUser(request);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }



//        [HttpPost("grid")]
//        public async Task<IActionResult> GetGridView(GettingUserGridRequest request, CancellationToken cancellationToken)
//        {
//            Guard.Against.Null(request, nameof(request));

//            var command = UserMapper.GetRequestMap(request);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }



//        [HttpGet()]
//        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
//        {

//            var command = new GettingUser();

//            var result = await sender.Send(command, cancellationToken);
//            return Ok(result);
//        }



//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
//        {
//            Guard.Against.NegativeOrZero(id, nameof(id));

//            var command = new GetUsertById(id);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }




//        [HttpPut]
//        public async Task<IActionResult> Update(UpdateUserRequest request, CancellationToken cancellationToken)
//        {
//            Guard.Against.Null(request, nameof(request));

//            var command = new UpdateUser(request);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }


//        //[HttpPost("login")]
//        //public async Task<IActionResult> Login(UserDto request, CancellationToken cancellationToken)
//        //{
//        //    Guard.Against.Null(request, nameof(request));

//        //    var command = new CreateUser(request);

//        //    var result = await sender.Send(command, cancellationToken);

//        //    return Ok(result);
//        //}


//        private string CreateToken(User user)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.Name, user.UserName),
//            };

//            var jwtKey = _configuration.GetValue<string>("Jwt:Key");
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _configuration.GetValue<string>("Jwt:Issuer"),
//                audience: _configuration.GetValue<string>("Jwt:Audience"),
//                claims: claims,
//                expires: DateTime.Now.AddDays(1),
//                signingCredentials: creds);

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
