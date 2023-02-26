using Application.Database.MsSql.Example;
using Application.Models.Example;
using Application.Repositories.Example;
using Application.Services.Database;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Infrastructure.Repositories.Example;

public class BookGenreRepository : IBookGenreRepository
{
    private readonly ISqlDataService _database;
    
    public BookGenreRepository(ISqlDataService database)
    {
        _database = database;
    }
    
    public async Task<List<BookGenreDb>> GetAll()
    {
        return (await _database.LoadData<BookGenreDb, dynamic>(BookGenres.GetAll, new { })).ToList();
    }

    public async Task<Guid?> Create(BookGenreCreate genreCreate)
    {
        var createdId = await _database.SaveDataReturnId(BookGenres.Insert, genreCreate);
        if (createdId == Guid.Empty)
            return null;

        return createdId;
    }

    public async Task<BookGenreDb?> GetById(Guid id)
    {
        var foundGenre = await _database.LoadData<BookGenreDb, dynamic>(
            BookGenres.GetById,
            new {Id = id});
        
        return foundGenre.FirstOrDefault();
    }

    public async Task<BookGenreDb?> GetByName(string genreName)
    {
        var foundGenre = await _database.LoadData<BookGenreDb, dynamic>(
            BookGenres.GetByName,
            new {Name = genreName});
        
        return foundGenre.FirstOrDefault();
    }

    public async Task<BookGenreDb?> GetByValue(string genreValue)
    {
        var foundGenre = await _database.LoadData<BookGenreDb, dynamic>(
            BookGenres.GetByValue,
            new {Value = genreValue});
        
        return foundGenre.FirstOrDefault();
    }

    public async Task Update(BookGenreUpdate genreUpdate)
    {
        await _database.SaveData(BookGenres.Update, genreUpdate);
    }

    public async Task Delete(Guid id)
    {
        await _database.SaveData(BookGenres.Delete, new {Id = id});
    }

    public async Task AddToBook(Guid bookId, Guid genreId)
    {
        await _database.SaveData(BookGenreJunctions.Insert,
            new {BookId = bookId, GenreId = genreId});
    }

    public async Task RemoveFromBook(Guid bookId, Guid genreId)
    {
        await _database.SaveData(BookGenreJunctions.Delete,
            new {BookId = bookId, GenreId = genreId});
    }

    public async Task<List<BookGenreDb>> GetGenresForBook(Guid bookId)
    {
        var bookGenreIds = await _database.LoadData<Guid, dynamic>(
            BookGenreJunctions.GetGenresForBook, new { BookId = bookId });

        var allGenres = await GetAll();

        var matchingGenres = allGenres.Where(x => bookGenreIds.Any(p => p == x.Id));
        
        return matchingGenres.ToList();
    }

    public async Task<List<BookDb>> GetBooksWithGenre(Guid genreId)
    {
        var bookIds = await _database.LoadData<Guid, dynamic>(
            BookGenreJunctions.GetBooksForGenre, new { GenreId = genreId });

        var allBooks = await _database.LoadData<BookDb, dynamic>(
            Books.GetAll, new { });

        var matchingBooks = allBooks.Where(x => bookIds.Any(p => p == x.Id));
        
        return matchingBooks.ToList();
    }

    public async Task<List<BookGenreJunctionFull>> GetAllBookGenreMappings()
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

        return mappingList;
    }
}