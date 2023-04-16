using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;

namespace Infrastructure.Repositories.MsSql.Example;

public class BookReviewRepositoryMsSql : IBookReviewRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    
    public BookReviewRepositoryMsSql(ISqlDataService database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetAllAsync()
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            var allReviews = (await _database.LoadData<BookReviewDb, dynamic>(BookReviewsMsSql.GetAll, new { })).ToList();
            actionReturn.Succeed(allReviews);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviewsMsSql.GetAll.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid?>> CreateAsync(BookReviewCreate createObject)
    {
        DatabaseActionResult<Guid?> actionReturn = new ();
        
        try
        {
            var createdId = await _database.SaveDataReturnId(BookReviewsMsSql.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviewsMsSql.Insert.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookReviewDb?>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<BookReviewDb?> actionReturn = new ();
        
        try
        {
            var foundReview = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviewsMsSql.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundReview);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviewsMsSql.GetById.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(BookReviewUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(BookReviewsMsSql.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviewsMsSql.Update.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(BookReviewsMsSql.Delete, new {Id = id});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviewsMsSql.Delete.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsForBookAsync(Guid bookId)
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            var foundReviews = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviewsMsSql.GetByBookId, new {BookId = bookId})).ToList();
            actionReturn.Succeed(foundReviews);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviewsMsSql.GetByBookId.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsFromAuthorAsync(string author)
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            var foundReviews = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviewsMsSql.GetAllFromAuthor, new {Author = author})).ToList();
            actionReturn.Succeed(foundReviews);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviewsMsSql.GetAllFromAuthor.Path, ex.Message);
        }
        
        return actionReturn;
    }
}