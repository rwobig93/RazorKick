using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;

namespace Infrastructure.Repositories.Example;

public class BookReviewRepository : IBookReviewRepository
{
    private readonly ISqlDataService _database;
    private readonly ILogger _logger;
    
    public BookReviewRepository(ISqlDataService database, ILogger logger)
    {
        _database = database;
        _logger = logger;
    }
    
    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetAllAsync()
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            var allReviews = (await _database.LoadData<BookReviewDb, dynamic>(BookReviews.GetAll, new { })).ToList();
            actionReturn.Succeed(allReviews);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviews.GetAll.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid?>> CreateAsync(BookReviewCreate createObject)
    {
        DatabaseActionResult<Guid?> actionReturn = new ();
        
        try
        {
            var createdId = await _database.SaveDataReturnId(BookReviews.Insert, createObject);
            actionReturn.Succeed(createdId);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviews.Insert.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookReviewDb?>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<BookReviewDb?> actionReturn = new ();
        
        try
        {
            var foundReview = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviews.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Succeed(foundReview);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviews.GetById.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(BookReviewUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(BookReviews.Update, updateObject);
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviews.Update.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> DeleteAsync(Guid id)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(BookReviews.Delete, new {Id = id});
            actionReturn.Succeed();
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviews.Delete.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsForBookAsync(Guid bookId)
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            var foundReviews = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviews.GetByBookId, new {BookId = bookId})).ToList();
            actionReturn.Succeed(foundReviews);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviews.GetByBookId.Path, ex.Message);
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsFromAuthorAsync(string author)
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            var foundReviews = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviews.GetAllFromAuthor, new {Author = author})).ToList();
            actionReturn.Succeed(foundReviews);
        }
        catch (Exception ex)
        {
            actionReturn.FailLog(_logger, BookReviews.GetAllFromAuthor.Path, ex.Message);
        }
        
        return actionReturn;
    }
}