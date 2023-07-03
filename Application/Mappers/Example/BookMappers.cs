using Application.Models.Example;
using Application.Requests.Example;
using Application.Responses.Example;
using Domain.DatabaseEntities.Example;
using Domain.Models.Example;

namespace Application.Mappers.Example;

public static class BookMappers
{
    public static BookFullDb ToFullObject(this BookDb book)
    {
        return new BookFullDb
        {
            Id = book.Id,
            Name = book.Name,
            Author = book.Author,
            Pages = book.Pages,
            Reviews = new List<BookReviewDb>(),
            Genres = new List<BookGenreDb>()
        };
    }
    
    public static BookResponse ToResponse(this BookDb book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Name = book.Name,
            Author = book.Author,
            Pages = book.Pages
        };
    }

    public static List<BookResponse> ToResponses(this IEnumerable<BookDb> bookList)
    {
        return bookList.Select(dbObject => dbObject.ToResponse()).ToList();
    }
    
    public static BookGenreResponse ToResponse(this BookGenreDb bookGenre)
    {
        return new BookGenreResponse
        {
            Id = bookGenre.Id,
            Name = bookGenre.Name,
            Value = bookGenre.Value
        };
    }

    public static List<BookGenreResponse> ToResponses(this IEnumerable<BookGenreDb> genreList)
    {
        return genreList.Select(dbObject => dbObject.ToResponse()).ToList();
    }
    
    public static BookReviewResponse ToResponse(this BookReviewDb bookReview)
    {
        return new BookReviewResponse
        {
            Id = bookReview.Id,
            BookId = bookReview.BookId,
            Author = bookReview.Author,
            Content = bookReview.Content
        };
    }
    
    public static List<BookReviewResponse> ToResponses(this IEnumerable<BookReviewDb> bookReviews)
    {
        return bookReviews.Select(x => x.ToResponse()).ToList();
    }
    
    public static BookReviewDb ToDb(this BookReviewResponse bookReviewResponse)
    {
        return new BookReviewDb
        {
            Id = bookReviewResponse.Id,
            BookId = bookReviewResponse.BookId,
            Author = bookReviewResponse.Author,
            Content = bookReviewResponse.Content
        };
    }
    
    public static BookFullResponse ToFullResponse(this BookFullDb bookFull)
    {
        return new BookFullResponse
        {
            Id = bookFull.Id,
            Name = bookFull.Name,
            Author = bookFull.Author,
            Pages = bookFull.Pages,
            Reviews = bookFull.Reviews.ToResponses(),
            Genres = bookFull.Genres.ToResponses()
        };
    }
    
    public static BookCreate ToCreate(this BookCreateRequest createRequest)
    {
        return new BookCreate
        {
            Name = createRequest.Name,
            Author = createRequest.Author,
            Pages = createRequest.Pages
        };
    }
    
    public static BookGenreCreateRequest ToCreateRequest(this BookGenreCreate createRequest)
    {
        return new BookGenreCreateRequest
        {
            Name = createRequest.Name,
            Value = createRequest.Value
        };
    }
    
    public static BookGenreCreate ToCreate(this BookGenreCreateRequest createRequest)
    {
        return new BookGenreCreate
        {
            Name = createRequest.Name,
            Value = createRequest.Value
        };
    }
    
    public static BookGenreUpdateRequest ToUpdateRequest(this BookGenreUpdate genreUpdate)
    {
        return new BookGenreUpdateRequest
        {
            Id = genreUpdate.Id,
            Name = genreUpdate.Name,
            Value = genreUpdate.Value
        };
    }
    
    public static BookGenreUpdate ToUpdate(this BookGenreUpdateRequest genreRequest)
    {
        return new BookGenreUpdate
        {
            Id = genreRequest.Id,
            Name = genreRequest.Name,
            Value = genreRequest.Value
        };
    }
    
    public static BookReviewCreateRequest ToCreateRequest(this BookReviewCreate reviewCreate)
    {
        return new BookReviewCreateRequest
        {
            BookId = reviewCreate.BookId,
            Author = reviewCreate.Author,
            Content = reviewCreate.Content
        };
    }
    
    public static BookReviewCreate ToCreate(this BookReviewCreateRequest reviewRequest)
    {
        return new BookReviewCreate
        {
            BookId = reviewRequest.BookId,
            Author = reviewRequest.Author,
            Content = reviewRequest.Content
        };
    }
    
    public static BookUpdate ToRequest(this BookUpdateRequest bookUpdate)
    {
        return new BookUpdate
        {
            Id = bookUpdate.Id,
            Name = bookUpdate.Name,
            Author = bookUpdate.Author,
            Pages = bookUpdate.Pages
        };
    }
}