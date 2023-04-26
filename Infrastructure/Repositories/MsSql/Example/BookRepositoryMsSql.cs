using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;
using Domain.Models.Example;

namespace Infrastructure.Repositories.MsSql.Example;

public class BookRepositoryMsSql : IBookRepository
{
    private readonly ISqlDataService _database;
    private readonly IBookReviewRepository _bookReviewRepository;
    private readonly IBookGenreRepository _bookGenreRepository;
    private readonly ILogger _logger;
    
    public BookRepositoryMsSql(ISqlDataService database,
        IBookReviewRepository bookReviewRepository, IBookGenreRepository bookGenreRepository, ILogger logger)
    {
        _database = database;
        _bookReviewRepository = bookReviewRepository;
        _bookGenreRepository = bookGenreRepository;
        _logger = logger;
    }
    
    public async Task<DatabaseActionResult<List<BookDb>>> GetAllAsync()
    {
        DatabaseActionResult<List<BookDb>> actionReturn = new ();
        
        try
        {
            var allBooks = (await _database.LoadData<BookDb, dynamic>(BooksMsSql.GetAll, new { })).ToList();
            actionReturn.Succeed(allBooks);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BooksMsSql.GetAll.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid?>> CreateAsync(BookCreate createBook)
    {
        DatabaseActionResult<Guid?> actionReturn = new ();
        
        try
        {
            var createdId = await _database.SaveDataReturnId(BooksMsSql.Insert, createBook);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BooksMsSql.Insert.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookDb?>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<BookDb?> actionReturn = new ();
        
        try
        {
            var foundBook = (await _database.LoadData<BookDb, dynamic>(
                BooksMsSql.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundBook);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BooksMsSql.GetById.Path, ex.Message);
        }
        
        return actionReturn;
    }

    private static Func<BookFullDb, BookGenreDb, BookFullDb> BookFullJoinMapping()
    {
        return (bookFull, genre) =>
        {
            bookFull.Genres.Add(genre);
            return bookFull;
        };
    }

    public async Task<DatabaseActionResult<BookFullDb?>> GetFullByIdAsync(Guid id)
    {
        DatabaseActionResult<BookFullDb?> actionReturn = new ();
        
        try
        {
            var foundBook = (await _database.LoadDataJoin<BookFullDb, BookGenreDb, dynamic>(
                BooksMsSql.GetByIdFull, BookFullJoinMapping(), new {Id = id})).FirstOrDefault();
            
            var foundReviews = (await _bookReviewRepository.GetReviewsForBookAsync(foundBook!.Id)).Result ?? new List<BookReviewDb>();
            foreach (var review in foundReviews)
                foundBook.Reviews.Add(review);
            
            actionReturn.Succeed(foundBook);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, "GetFullByIdAsync", ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(BookUpdate bookUpdate)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(BooksMsSql.Update, bookUpdate);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BooksMsSql.Update.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(BooksMsSql.Delete, new {Id = id});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BooksMsSql.Delete.Path, ex.Message);
        }
        
        return actionReturn;
    }
}