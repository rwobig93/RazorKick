using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;
using Domain.Models.Example;

namespace Infrastructure.Repositories.MsSql.Example;

public class BookGenreRepositoryMsSql : IBookGenreRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    
    public BookGenreRepositoryMsSql(ISqlDataService database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<DatabaseActionResult<List<BookGenreDb>>> GetAllAsync()
    {
        DatabaseActionResult<List<BookGenreDb>> actionReturn = new ();
        
        try
        {
            var allGenres = (await _database.LoadData<BookGenreDb, dynamic>(BookGenresMsSql.GetAll, new { })).ToList();
            actionReturn.Succeed(allGenres);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenresMsSql.GetAll.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid?>> CreateAsync(BookGenreCreate genreCreate)
    {
        DatabaseActionResult<Guid?> actionReturn = new ();
        
        try
        {
            var createdId = await _database.SaveDataReturnId(BookGenresMsSql.Insert, genreCreate);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenresMsSql.Insert.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookGenreDb?>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<BookGenreDb?> actionReturn = new ();
        
        try
        {
            var foundGenre = (await _database.LoadData<BookGenreDb, dynamic>(
                BookGenresMsSql.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundGenre);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenresMsSql.GetById.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookGenreDb?>> GetByNameAsync(string genreName)
    {
        DatabaseActionResult<BookGenreDb?> actionReturn = new ();
        
        try
        {
            var foundGenre = (await _database.LoadData<BookGenreDb, dynamic>(
                BookGenresMsSql.GetByName, new {Name = genreName})).FirstOrDefault();
            actionReturn.Succeed(foundGenre);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenresMsSql.GetByName.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookGenreDb?>> GetByValueAsync(string genreValue)
    {
        DatabaseActionResult<BookGenreDb?> actionReturn = new ();

        try
        {
            var foundGenre = (await _database.LoadData<BookGenreDb, dynamic>(
                BookGenresMsSql.GetByValue, new {Value = genreValue})).FirstOrDefault();
            actionReturn.Succeed(foundGenre);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenresMsSql.GetByValue.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(BookGenreUpdate genreUpdate)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenresMsSql.Update, genreUpdate);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenresMsSql.Update.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenresMsSql.Delete, new {Id = id});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenresMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> AddToBookAsync(Guid bookId, Guid genreId)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenreJunctionsMsSql.Insert, new {BookId = bookId, GenreId = genreId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenreJunctionsMsSql.Insert.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult> RemoveFromBookAsync(Guid bookId, Guid genreId)
    {
        DatabaseActionResult actionReturn = new ();

        try
        {
            await _database.SaveData(BookGenreJunctionsMsSql.Delete, new {BookId = bookId, GenreId = genreId});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookGenreJunctionsMsSql.Delete.Path, ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookGenreDb>>> GetGenresForBookAsync(Guid bookId)
    {
        DatabaseActionResult<List<BookGenreDb>> actionReturn = new ();

        try
        {
            var bookGenreIds = await _database.LoadData<Guid, dynamic>(
                BookGenreJunctionsMsSql.GetGenresForBook, new { BookId = bookId });

            var allGenres = (await GetAllAsync()).Result ?? new List<BookGenreDb>();
            var matchingGenres = allGenres.Where(x => bookGenreIds.Any(p => p == x.Id)).ToList();

            actionReturn.Succeed(matchingGenres);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetGenresForBookAsync", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookDb>>> GetBooksWithGenreAsync(Guid genreId)
    {
        DatabaseActionResult<List<BookDb>> actionReturn = new ();

        try
        {
            var bookIds = await _database.LoadData<Guid, dynamic>(
                BookGenreJunctionsMsSql.GetBooksForGenre, new { GenreId = genreId });

            var allBooks = await _database.LoadData<BookDb, dynamic>(
                BooksMsSql.GetAll, new { });

            var matchingBooks = allBooks.Where(x => bookIds.Any(p => p == x.Id)).ToList();

            actionReturn.Succeed(matchingBooks);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetBooksWithGenreAsync", ex.Message);
        }

        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookGenreJunctionFull>>> GetAllBookGenreMappingsAsync()
    {
        DatabaseActionResult<List<BookGenreJunctionFull>> actionReturn = new ();

        try
        {
            var mappings = await _database.LoadData<BookGenreJunctionDb, dynamic>(
                BookGenreJunctionsMsSql.GetAll, new { });

            var allBooks = await _database.LoadData<BookDb, dynamic>(
                BooksMsSql.GetAll, new { });

            var allGenres = await _database.LoadData<BookGenreDb, dynamic>(
                BookGenresMsSql.GetAll, new { });

            var mappingList = mappings.ToList().Select(mapping => new BookGenreJunctionFull()
            {
                Book = allBooks.FirstOrDefault(x => x.Id == mapping.BookId)!,
                Genre = allGenres.FirstOrDefault(x => x.Id == mapping.GenreId)!
            }).ToList();

            actionReturn.Succeed(mappingList);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetAllBookGenreMappingsAsync", ex.Message);
        }

        return actionReturn;
    }
}