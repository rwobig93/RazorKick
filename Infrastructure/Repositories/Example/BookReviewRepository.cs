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
    
    public BookReviewRepository(ISqlDataService database)
    {
        _database = database;
    }
    
    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetAllAsync()
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookReviewDb, dynamic>(BookReviews.GetAll, new { })).ToList();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid?>> CreateAsync(BookReviewCreate createObject)
    {
        DatabaseActionResult<Guid?> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.SaveDataReturnId(BookReviews.Insert, createObject);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookReviewDb?>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<BookReviewDb?> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviews.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(BookReviewUpdate updateObject)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(BookReviews.Update, updateObject);
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
            await _database.SaveData(BookReviews.Delete, new {Id = id});
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsForBookAsync(Guid bookId)
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviews.GetByBookId, new {BookId = bookId})).ToList();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<List<BookReviewDb>>> GetReviewsFromAuthorAsync(string author)
    {
        DatabaseActionResult<List<BookReviewDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookReviewDb, dynamic>(
                BookReviews.GetAllFromAuthor, new {Author = author})).ToList();
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