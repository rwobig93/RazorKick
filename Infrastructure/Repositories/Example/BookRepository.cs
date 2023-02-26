using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Database;
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
    
    public async Task<DatabaseActionResult<List<BookDb>>> GetAllAsync()
    {
        DatabaseActionResult<List<BookDb>> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookDb, dynamic>(Books.GetAll, new { })).ToList();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<Guid?>> CreateAsync(BookCreate createBook)
    {
        DatabaseActionResult<Guid?> actionReturn = new ();
        
        try
        {
            actionReturn.Result = await _database.SaveDataReturnId(Books.Insert, createBook);
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookDb?>> GetByIdAsync(Guid id)
    {
        DatabaseActionResult<BookDb?> actionReturn = new ();
        
        try
        {
            actionReturn.Result = (await _database.LoadData<BookDb, dynamic>(
                Books.GetById, new {Id = id})).FirstOrDefault();
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult<BookFull?>> GetFullByIdAsync(Guid id)
    {
        DatabaseActionResult<BookFull?> actionReturn = new ();
        
        try
        {
            var foundBook = (await GetByIdAsync(id)).Result;
            var convertedFullBook = foundBook?.ToFullObject();
            
            var foundReviews = (await _bookReviewRepository.GetReviewsForBookAsync(foundBook!.Id)).Result ?? new List<BookReviewDb>();
            foreach (var review in foundReviews)
                convertedFullBook!.Reviews.Add(review);

            var foundGenres = (await _bookGenreRepository.GetGenresForBookAsync(foundBook.Id)).Result ?? new List<BookGenreDb>();
            foreach (var genre in foundGenres)
                convertedFullBook!.Genres.Add(genre);
            
            actionReturn.Result = convertedFullBook;
            actionReturn.Success = true;
        }
        catch (Exception ex)
        {
            actionReturn.Success = false;
            actionReturn.ErrorMessage = ex.Message;
        }
        
        return actionReturn;
    }

    public async Task<DatabaseActionResult> UpdateAsync(BookUpdate bookUpdate)
    {
        DatabaseActionResult actionReturn = new ();
        
        try
        {
            await _database.SaveData(Books.Update, bookUpdate);
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
            await _database.SaveData(Books.Delete, new {Id = id});
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