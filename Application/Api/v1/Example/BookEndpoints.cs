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
        app.MapGet("/api/example/book/full", GetBookFull).ApiVersionOne();
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
            var allBooks = await repository.GetAllAsync();
            if (!allBooks.Success)
                return await Result<List<BookResponse>>.FailAsync(allBooks.ErrorMessage);
            
            return await Result<List<BookResponse>>.SuccessAsync(allBooks.Result!.ToResponses());
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
            var foundBook = await repository.GetByIdAsync(bookId);
            if (!foundBook.Success)
                return await Result<BookResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<BookResponse>.SuccessAsync(foundBook.Result!.ToResponse());
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
            var foundBook = await repository.GetFullByIdAsync(bookId);
            if (!foundBook.Success)
                return await Result<BookFullResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<BookFullResponse>.SuccessAsync(foundBook.Result!.ToFullResponse());
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
            var createdId = await repository.CreateAsync(bookRequest.ToCreate());
            if (!createdId.Success)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.GenericError);

            return await Result<Guid>.SuccessAsync((Guid)createdId.Result!);
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
            var update = await repository.UpdateAsync(updateRequest.ToRequest());
            if (!update.Success)
                return await Result.FailAsync(update.ErrorMessage);
            
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
            var delete = await repository.DeleteAsync(bookId);
            if (!delete.Success)
                return await Result.FailAsync(delete.ErrorMessage);
            
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
            var createdId = await repository.CreateAsync(createReviewRequest.ToCreate());
            if (!createdId.Success)
                return await Result<Guid>.FailAsync(createdId.ErrorMessage);

            return await Result<Guid>.SuccessAsync((Guid)createdId.Result!);
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
            var remove = await repository.DeleteAsync(reviewId);
            if (!remove.Success)
                return await Result.FailAsync(remove.ErrorMessage);
            
            return await Result.SuccessAsync("Book review successfully deleted!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}