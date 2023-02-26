using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;

namespace Infrastructure.Repositories.Example;

public class BookReviewRepository : IBookReviewRepository
{
    private readonly ISqlDataService _database;
    
    public BookReviewRepository(ISqlDataService database)
    {
        _database = database;
    }
    
    public async Task<List<BookReviewDb>> GetAll()
    {
        return (await _database.LoadData<BookReviewDb, dynamic>(BookReviews.GetAll, new { })).ToList();
    }

    public async Task<Guid?> Create(BookReviewCreate createObject)
    {
        var createdId = await _database.SaveDataReturnId(BookReviews.Insert, createObject);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<BookReviewDb?> Get(Guid id)
    {
        var foundObject = await _database.LoadData<BookReviewDb, dynamic>(
            BookReviews.GetById,
            new {Id = id});
        
        return foundObject.FirstOrDefault();
    }

    public async Task Update(BookReviewUpdate updateObject)
    {
        await _database.SaveData(BookReviews.Update, updateObject);
    }

    public async Task Delete(Guid id)
    {
        await _database.SaveData(BookReviews.Delete, new {Id = id});
    }

    public async Task<List<BookReviewDb>> GetReviewsForBook(Guid bookId)
    {
        var reviews = await _database.LoadData<BookReviewDb, dynamic>(
            BookReviews.GetByBookId, new { BookId = bookId });

        return reviews.ToList();
    }

    public async Task<List<BookReviewDb>> GetReviewsFromAuthor(string author)
    {
        var reviews = await _database.LoadData<BookReviewDb, dynamic>(
            BookReviews.GetAllFromAuthor, new { Author = author });

        return reviews.ToList();
    }
}