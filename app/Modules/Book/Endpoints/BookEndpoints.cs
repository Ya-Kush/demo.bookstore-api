using App.Data.Extensions;
using App.Endpoints.Models;
using App.Endpoints.Services;
using App.Services.Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public static class BookEndpoints
{
    public static RouteGroupBuilder MapBooks(this IEndpointRouteBuilder routeBuilder)
    {
        return routeBuilder.MapGroup("/books").WithTags("Book").MapRouteHandlers(
            new("", GetBooks, PostBook),
            new("/{bookId:guid}", GetBook, PatchBook, DeleteBook),
            new("/{bookId:guid}/authors", GetBookAuthors),
            new("/{bookId:guid}/authors/{authorId:guid}", PutBookAuthor, DeleteBookAuthor));
    }

    #region Handlers
    public static Ok<Set<GetBook>> GetBooks(BookRepo bookRepo, EndpointContext context)
    {
        return Ok(Set.New(bookRepo.Books.Untrack().ToGetBooks(context)));
    }

    public static IResult PostBook([FromBody] PostBook postBook, BookRepo bookRepo, EndpointContext context)
    {
        var res = bookRepo.SaveNewBook(postBook);
        return res.Match<IResult>(
            book => Created(
                uri: context.GetLink(GetBook, new { bookId = book.Id }),
                value: book.ToGetBook(context)),
            err => BadRequest(err.Message));
    }


    public static IResult GetBook(Guid bookId, BookRepo bookRepo, EndpointContext context)
    {
        var res = bookRepo.GetBook(bookId);

        return res.Match<IResult>(
            book => Ok(book.ToGetBook(context)),
            err => NotFound());
    }

    public static IResult PatchBook(Guid bookId, [FromBody] PatchBook patchBook, BookRepo bookRepo)
    {
        var res = bookRepo.UpdateBook(bookId, patchBook);
        return res.Match<IResult>(
            book => Ok(book),
            err => err switch
            {
                NotFoundError => NotFound(),
                _ => BadRequest(err.Message)
            });
    }

    public static IResult DeleteBook(Guid bookId, BookRepo bookRepo)
    {
        return bookRepo.DeleteBook(bookId).IsOk
            ? Ok(new { bookId })
            : NotFound(new { bookId });
    }


    public static IResult GetBookAuthors(Guid bookId, BookRepo bookRepo, EndpointContext context)
    {
        var res = bookRepo.GetBookAuthors(bookId);

        return res.Match<IResult>(
            authors => Ok(authors.ToGetAuthors(context)),
            err => NotFound());
    }


    public static IResult PutBookAuthor(Guid bookId, Guid authorId, BookRepo bookRepo, AuthorRepo authorRepo)
    {
        var res = bookRepo.AddAuthorToBook(bookId, authorId);
        return res.Match<IResult>(
            Ok,
            err => NotFound());
    }

    public static IResult DeleteBookAuthor(Guid bookId, Guid authorId, BookRepo bookRepo)
    {
        var res = bookRepo.RemoveAuthorFromBook(bookId, authorId);
        return res.Match<IResult>(
            Ok,
            err => NotFound());
    }
    #endregion Handlers
}
