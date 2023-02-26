using Application.Constants.Messages;
using Application.Helpers.Web;
using Application.Models.Example;
using Application.Models.Web;
using Application.Repositories.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Api.v1.Example;

public static class BookEndpoints
{
    public static void MapEndpointsBooks(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/example/books", GetAllBooks).ApiVersionOne();
        app.MapGet("/api/example/book", GetBookById).ApiVersionOne();
        app.MapGet("/api/example/book-full", GetBookFull).ApiVersionOne();
        app.MapPost("/api/example/book", CreateBook).ApiVersionOne();
        app.MapPut("/api/example/book", UpdateBook).ApiVersionOne();
        app.MapDelete("/api/example/book", DeleteBook).ApiVersionOne();
        
        app.MapPost("/api/example/book/review", AddReview).ApiVersionOne();
        app.MapDelete("/api/example/book/review", RemoveReview).ApiVersionOne();
    }

    // TODO: Add authorization/permissions to these endpoints
    private static async Task<IResult<List<BookResponse>>> GetAllBooks(IBookRepository repository)
    {
        try
        {
            var allBooks = await repository.GetAll();
            
            return await Result<List<BookResponse>>.SuccessAsync(allBooks.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<BookResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<BookResponse>> GetBookById(Guid bookId, IBookRepository repository)
    {
        try
        {
            var foundBook = await repository.Get(bookId);
            
            if (foundBook is null)
                return await Result<BookResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<BookResponse>.SuccessAsync(foundBook.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<BookResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<BookFullResponse>> GetBookFull(Guid bookId, IBookRepository repository)
    {
        try
        {
            var foundBook = await repository.GetFull(bookId);
            
            if (foundBook is null)
                return await Result<BookFullResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<BookFullResponse>.SuccessAsync(foundBook.ToFullResponse());
        }
        catch (Exception ex)
        {
            return await Result<BookFullResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<Guid>> CreateBook(BookCreateRequest bookRequest, IBookRepository repository)
    {
        try
        {
            var createdId = await repository.Create(bookRequest.ToCreate());
            if (createdId is null)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.GenericError);

            return await Result<Guid>.SuccessAsync((Guid)createdId);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdateBook(BookUpdateRequest updateRequest, IBookRepository repository)
    {
        try
        {
            await repository.Update(updateRequest.ToRequest());
            return await Result.SuccessAsync("Book successfully updated!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeleteBook(Guid bookId, IBookRepository repository)
    {
        try
        {
            await repository.Delete(bookId);
            return await Result.SuccessAsync("Book successfully deleted!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<Guid>> AddReview(BookReviewCreateRequest createReviewRequest,
        IBookReviewRepository repository)
    {
        try
        {
            var createdId = await repository.Create(createReviewRequest.ToCreate());
            if (createdId is null)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.GenericError);

            return await Result<Guid>.SuccessAsync((Guid)createdId);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemoveReview(Guid reviewId, IBookReviewRepository repository)
    {
        try
        {
            await repository.Delete(reviewId);
            return await Result.SuccessAsync("ExampleExtendedAttribute successfully deleted!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}