//using Ardalis.GuardClauses;
//using HotelRoomMS.Application.Books;
//using HotelRoomMS.Application.Books.Features.CreateBooks;
//using HotelRoomMS.Application.Books.Features.CreateBooks.Requests;
//using HotelRoomMS.Application.Books.Features.GetBooksById;
//using HotelRoomMS.Application.Books.Features.GettingBooks;
//using HotelRoomMS.Application.Books.Features.UpdateBooks;
//using HotelRoomMS.Application.Books.Features.UpdateBooks.Requests;
//using Common.Abstractions.CQRS;
//using Microsoft.AspNetCore.Mvc;

//namespace HotelRoomMS.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class BookController : Controller
//    {
//        private readonly ISender sender;

//        public BookController(ISender sender)
//        {
//            this.sender = sender;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Create(CreateBookRequest request, CancellationToken cancellationToken)
//        {
//            Guard.Against.Null(request, nameof(request));

//            var command = new CreateBook(request);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }



//        [HttpPost("grid")]
//        public async Task<IActionResult> GetGridView(GettingBookGridRequest request, CancellationToken cancellationToken)
//        {
//            Guard.Against.Null(request, nameof(request));

//            var command = BookMapper.GetRequestMap(request);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }



//        [HttpGet()]
//        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
//        {

//            var command = new GettingBook();

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }



//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
//        {
//            Guard.Against.NegativeOrZero(id, nameof(id));

//            var command = new GetBookById(id);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }




//        [HttpPut]
//        public async Task<IActionResult> Update(UpdateBookRequest request, CancellationToken cancellationToken)
//        {
//            Guard.Against.Null(request, nameof(request));

//            var command = new UpdateBook(request);

//            var result = await sender.Send(command, cancellationToken);

//            return Ok(result);
//        }
//    }
//}
