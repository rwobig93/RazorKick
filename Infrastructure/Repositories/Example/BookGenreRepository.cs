using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;
using Domain.Models.Example;

namespace Infrastructure.Repositories.Example;

public class BookGenreRepository : IBookGenreRepository
{
    private readonly ISqlDataService _database;
    
    public BookGenreRepository(ISqlDataService database)
    {
        _database = database;
    }
    
    public async Task<DatabaseActionResult<List<BookGenreDb>>> GetAllAsync()
    {
        DatabaseActionResult<List<BookGenreDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookGenreDb, dynamic>(BookGenres.GetAll, new { })).ToList();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid?>> CreateAsync(BookGenreCreate genreCreate)
    {
        DatabaseActionResult<Guid?> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.SaveDataReturnId(BookGenres.Insert, genreCreate);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookGenreDb?>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<BookGenreDb?> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookGenreDb, dynamic>(
                BookGenres.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookGenreDb?>> GetByNameAsync(string genreName)
    {
        DatabaseActionResult<BookGenreDb?> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookGenreDb, dynamic>(
                BookGenres.GetByName, new {Name = genreName})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookGenreDb?>> GetByValueAsync(string genreValue)
    {
        DatabaseActionResult<BookGenreDb?> actionReturn = new ();

        try
        {
            actionReturn.Result = (await _database.LoadData<BookGenreDb, dynamic>(
                BookGenres.GetByValue, new {Value = genreValue})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(BookGenreUpdate genreUpdate)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenres.Update, genreUpdate);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenres.Delete, new {Id = id});
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddToBookAsync(Guid bookId, Guid genreId)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenreJunctions.Insert, new {BookId = bookId, GenreId = genreId});
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveFromBookAsync(Guid bookId, Guid genreId)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenreJunctions.Delete, new {BookId = bookId, GenreId = genreId});
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookGenreDb>>> GetGenresForBookAsync(Guid bookId)
    {
        DatabaseActionResult<List<BookGenreDb>> actionReturn = new ();

        try
        {
            var bookGenreIds = await _database.LoadData<Guid, dynamic>(
                BookGenreJunctions.GetGenresForBook, new { BookId = bookId });

            var allGenres = (await GetAllAsync()).Result ?? new List<BookGenreDb>();
            var matchingGenres = allGenres.Where(x => bookGenreIds.Any(p => p == x.Id)).ToList();

            actionReturn.Result = matchingGenres;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookDb>>> GetBooksWithGenreAsync(Guid genreId)
    {
        DatabaseActionResult<List<BookDb>> actionReturn = new ();

        try
        {
            var bookIds = await _database.LoadData<Guid, dynamic>(
                BookGenreJunctions.GetBooksForGenre, new { GenreId = genreId });

            var allBooks = await _database.LoadData<BookDb, dynamic>(
                Books.GetAll, new { });

            var matchingBooks = allBooks.Where(x => bookIds.Any(p => p == x.Id)).ToList();

            actionReturn.Result = matchingBooks;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookGenreJunctionFull>>> GetAllBookGenreMappingsAsync()
    {
        DatabaseActionResult<List<BookGenreJunctionFull>> actionReturn = new ();

        try
        {
            var mappings = await _database.LoadData<BookGenreJunctionDb, dynamic>(
                BookGenreJunctions.GetAll, new { });

            var allBooks = await _database.LoadData<BookDb, dynamic>(
                Books.GetAll, new { });

            var allGenres = await _database.LoadData<BookGenreDb, dynamic>(
                BookGenres.GetAll, new { });

            var mappingList = mappings.ToList().Select(mapping => new BookGenreJunctionFull()
            {
                Book = allBooks.FirstOrDefault(x => x.Id == mapping.BookId)!,
                Genre = allGenres.FirstOrDefault(x => x.Id == mapping.GenreId)!
            }).ToList();

            actionReturn.Result = mappingList;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }

        return actionReturn;
    }
}