using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Infrastructure.Repositories.Example;

public class BookRepository : IBookRepository
{
    private readonly ISqlDataService _database;
    private readonly IBookReviewRepository _bookReviewRepository;
    private readonly IBookGenreRepository _bookGenreRepository;
    
    public BookRepository(ISqlDataService database,
        IBookReviewRepository bookReviewRepository, IBookGenreRepository bookGenreRepository)
    {
        _database = database;
        _bookReviewRepository = bookReviewRepository;
        _bookGenreRepository = bookGenreRepository;
    }
    
    public async Task<List<BookDb>> GetAll()
    {
        return (await _database.LoadData<BookDb, dynamic>(Books.GetAll, new { })).ToList();
    }

    public async Task<Guid?> Create(BookCreate createBook)
    {
        var createdId = await _database.SaveDataReturnId(Books.Insert, createBook);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<BookDb?> Get(Guid id)
    {
        var foundObject = await _database.LoadData<BookDb, dynamic>(
            Books.GetById,
            new {Id = id});
        
        return foundObject.FirstOrDefault()!;
    }

    public async Task<BookFull?> GetFull(Guid id)
    {
        var foundBook = await Get(id);

        if (foundBook is null)
            return null;

        var convertedFullBook = foundBook.ToFullObject();

        var foundReviews = await _bookReviewRepository.GetReviewsForBook(foundBook.Id);
        
        foreach (var review in foundReviews)
            convertedFullBook.Reviews.Add(review);

        var foundGenres = await _bookGenreRepository.GetGenresForBook(foundBook.Id);
        
        foreach (var genre in foundGenres)
            convertedFullBook.Genres.Add(genre);
        
        return convertedFullBook;
    }

    public async Task Update(BookUpdate bookUpdate)
    {
        await _database.SaveData(Books.Update, bookUpdate);
    }

    public async Task Delete(Guid id)
    {
        await _database.SaveData(Books.Delete, new {Id = id});
    }
}