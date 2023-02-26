using Application.Constants.Messages;
using Application.Helpers.Web;
using Application.Models.Example;
using Application.Models.Web;
using Application.Repositories.Example;
using Domain.DatabaseEntities.Example;
using Shared.Requests.Example;
using Shared.Responses.Example;

namespace Application.Api.v1.Example;

public static class BookGenreEndpoints
{
    public static void MapEndpointsBookGenres(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/example/genres", GetAllGenres).ApiVersionOne();
        app.MapGet("/api/example/genre", GetGenre).ApiVersionOne();
        app.MapPost("/api/example/genre", CreateGenre).ApiVersionOne();
        app.MapPut("/api/example/genre", UpdateGenre).ApiVersionOne();
        app.MapDelete("/api/example/genre", DeleteGenre).ApiVersionOne();
        app.MapPost("/api/example/genre/book-add", AddGenreToBook).ApiVersionOne();
        app.MapPost("/api/example/genre/book-remove", RemoveGenreFromBook).ApiVersionOne();
    }

    // TODO: Add authorization/genres to these endpoints
    private static async Task<IResult<List<BookGenreResponse>>> GetAllGenres(IBookGenreRepository repository)
    {
        try
        {
            var allGenres = await repository.GetAll();
            
            return await Result<List<BookGenreResponse>>.SuccessAsync(allGenres.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<BookGenreResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<BookGenreResponse>> GetGenre(Guid genreId, IBookGenreRepository repository)
    {
        try
        {
            var foundGenre = await repository.GetById(genreId);
            
            if (foundGenre is null)
                return await Result<BookGenreResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<BookGenreResponse>.SuccessAsync(foundGenre.ToResponse());
        }
        catch (Exception ex)
        {
            return await Result<BookGenreResponse>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<Guid>> CreateGenre(BookGenreCreateRequest genreRequest, IBookGenreRepository repository)
    {
        try
        {
            var createdId = await repository.Create(genreRequest.ToCreate());
            if (createdId is null)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.GenericError);

            return await Result<Guid>.SuccessAsync((Guid)createdId);
        }
        catch (Exception ex)
        {
            return await Result<Guid>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> UpdateGenre(BookGenreUpdateRequest updateRequest, IBookGenreRepository repository)
    {
        try
        {
            await repository.Update(updateRequest.ToUpdate());
            return await Result.SuccessAsync("Genre successfully updated!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> DeleteGenre(Guid genreId, IBookGenreRepository repository)
    {
        try
        {
            await repository.Delete(genreId);
            return await Result.SuccessAsync("Genre successfully deleted!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult<List<BookGenreResponse>>> GetGenresForBook(Guid bookId, IBookGenreRepository repository)
    {
        try
        {
            var genres = await repository.GetGenresForBook(bookId);
            return await Result<List<BookGenreResponse>>.SuccessAsync(genres.ToResponses());
        }
        catch (Exception ex)
        {
            return await Result<List<BookGenreResponse>>.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> AddGenreToBook(Guid bookId, Guid genreId, IBookGenreRepository repository)
    {
        try
        {
            await repository.AddToBook(bookId, genreId);
            return await Result.SuccessAsync("Genre successfully added!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }

    private static async Task<IResult> RemoveGenreFromBook(Guid bookId, Guid genreId, IBookGenreRepository repository)
    {
        try
        {
            await repository.RemoveFromBook(bookId, genreId);
            return await Result.SuccessAsync("Genre successfully removed!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}