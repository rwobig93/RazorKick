using Application.Constants.Communication;
using Application.Helpers.Web;
using Application.Mappers.Example;
using Application.Models.Web;
using Application.Repositories.Example;
using Application.Requests.Example;
using Application.Responses.Example;

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
        
        app.MapGet("/api/example/genre/books", GetGenresForBook).ApiVersionOne();
        app.MapPost("/api/example/genre/book/add", AddGenreToBook).ApiVersionOne();
        app.MapPost("/api/example/genre/book/remove", RemoveGenreFromBook).ApiVersionOne();
    }

    // TODO: Add authorization/genres to these endpoints
    private static async Task<IResult<List<BookGenreResponse>>> GetAllGenres(IBookGenreRepository repository)
    {
        try
        {
            var allGenres = await repository.GetAllAsync();
            if (!allGenres.Success)
                return await Result<List<BookGenreResponse>>.FailAsync(allGenres.ErrorMessage);

            return await Result<List<BookGenreResponse>>.SuccessAsync(allGenres.Result!.ToResponses());
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
            var foundGenre = await repository.GetByIdAsync(genreId);
            if (!foundGenre.Success)
                return await Result<BookGenreResponse>.FailAsync(ErrorMessageConstants.InvalidValueError);

            return await Result<BookGenreResponse>.SuccessAsync(foundGenre.Result!.ToResponse());
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
            var createdId = await repository.CreateAsync(genreRequest.ToCreate());
            if (!createdId.Success)
                return await Result<Guid>.FailAsync(ErrorMessageConstants.GenericError);

            return await Result<Guid>.SuccessAsync((Guid)createdId.Result!);
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
            var update = await repository.UpdateAsync(updateRequest.ToUpdate());
            if (!update.Success)
                return await Result.FailAsync(update.ErrorMessage);
            
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
            var delete = await repository.DeleteAsync(genreId);
            if (!delete.Success)
                return await Result.FailAsync(delete.ErrorMessage);
            
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
            var genres = await repository.GetGenresForBookAsync(bookId);
            if (!genres.Success)
                return await Result<List<BookGenreResponse>>.FailAsync(genres.ErrorMessage);
            
            return await Result<List<BookGenreResponse>>.SuccessAsync(genres.Result!.ToResponses());
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
            var add = await repository.AddToBookAsync(bookId, genreId);
            if (!add.Success)
                return await Result.FailAsync(add.ErrorMessage);
            
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
            var remove = await repository.RemoveFromBookAsync(bookId, genreId);
            if (!remove.Success)
                return await Result.FailAsync(remove.ErrorMessage);
            
            return await Result.SuccessAsync("Genre successfully removed!");
        }
        catch (Exception ex)
        {
            return await Result.FailAsync(ex.Message);
        }
    }
}