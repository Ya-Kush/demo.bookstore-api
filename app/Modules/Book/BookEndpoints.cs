using App.Common;
using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public static class BookEndpoints
{
    public static RouteGroupBuilder MapBooks(this IEndpointRouteBuilder routeBuilder)
        => routeBuilder.MapGroup("/books").MapRouteHandlers(
            new("", GetBooks, PostBook),
            new("/{id:guid}", GetBook, PutBook, PatchBook, DeleteBook),
            new("/{id:guid}/authors", GetBookAuthors));

    #region Handlers
    public static IResult GetBooks(BookRepo repo, EndpointContext context)
        => Ok(new { data = repo.UntrackedBooks.ToGetBooks(context) });

    public static IResult GetBook(Guid id, BookRepo repo)
        => repo.FindUntracked(id) switch
        {
            Ok<Book> ok => Ok(ok.Value.ToGetBook(repo)),
            NotFound<Book> => NotFound(new { id }),
            _ => StatusCode(500)
        };

    public static IResult GetBookAuthors(Guid id, BookRepo repo, AuthorRepo authorRepo, EndpointContext context)
        => repo.FindBookAuthors(id) switch
        {
            Ok<IEnumerable<Author>> books => Ok(books.Value.ToGetAuthors(context)),
            NotFound<IEnumerable<Author>> => NotFound(new { id }),
            _ => StatusCode(500)
        };


    public static IResult PostBook([FromBody]PostBook postBook, BookRepo repo, EndpointContext context)
        => repo.SaveNewBook(postBook) switch
        {
            Created<Book> cr => Created(
                uri: context.GetLink(GetBook, new { cr.Value.Id }),
                value: cr.Value.ToGetBook(repo)),
            Error<Book, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PutBook(Guid id, [FromBody]PutBook putBook, BookRepo repo, EndpointContext context)
        => repo.SaveBook(id, putBook) switch
        {
            Created<Book> cr => Created(
                uri: context.GetLink(GetBook, new { cr.Value.Id }),
                value: cr.Value.ToGetBook(repo)),
            Updated<Book> ok => Ok(ok.Value.ToGetBook(repo)),
            Error<Book, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PatchBook(Guid id, [FromBody]PatchBook patchBook, BookRepo repo)
        => repo.UpdateBook(id, patchBook) switch
        {
            Ok<Book> ok => Ok(ok),
            Error<Book, string> err => BadRequest(err),
            _ => StatusCode(500)
        };


    public static IResult DeleteBook(Guid id, BookRepo repo)
        => repo.DeleteBook(id) is Ok<Book>
        ? Ok(new { id })
        : NotFound(new { id });
    #endregion Handlers
}

public static class BookEndpointsExtenstions
{
    public static IEnumerable<GetBook> ToGetBooks(this IEnumerable<Book> books, EndpointContext context)
        => books.Select(x => x.ToGetBook(context));

    public static GetBook ToGetBook(this Book book, EndpointContext context)
        => book.ToGetBook(new
        {
            self = context.GetLink(BookEndpoints.GetBook, new { book.Id }),
            books = context.GetLink(BookEndpoints.GetBookAuthors, new { book.Id }),
        });
}
