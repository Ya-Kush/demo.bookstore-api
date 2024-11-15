using App.Common;
using App.DomainModels;
using App.EndpointModels;
using App.EndpointServices;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.EndpointHandlers;

public static class BookHandlers
{
    public static RouteGroupBuilder MapBooks(this IEndpointRouteBuilder routeBuilder)
        => routeBuilder.MapGroup("/books").MapRouteHandlers(
            new("", GetBooks, PostBook),
            new("/{id:guid}", GetBook, PutBook, PatchBook, DeleteBook),
            new("/{id:guid}/authors", GetBookAuthors));

    #region Handlers
    public static IResult GetBooks(BookHandlerService service)
        => Ok(service.UntrackedBooks.ToGetBooks(service));

    public static IResult GetBook(Guid id, BookHandlerService service)
        => service.FindUntracked(id) switch
        {
            Ok<Book> ok => Ok(ok.Value.ToGetBook(service)),
            NotFound<Book> => NotFound(new { id }),
            _ => StatusCode(500)
        };

    public static IResult GetBookAuthors(Guid id, BookHandlerService service, AuthorHandlerService authorService)
        => service.FindBookAuthors(id) switch
        {
            Ok<IEnumerable<Author>> books => Ok(books.Value.ToGetAuthors(authorService)),
            NotFound<IEnumerable<Author>> => NotFound(new { id }),
            _ => StatusCode(500)
        };


    public static IResult PostBook([FromBody]PostBook postBook, BookHandlerService service)
        => service.SaveNewBook(postBook) switch
        {
            Created<Book> cr => Created(
                uri: service.GetLink(GetBook, new { cr.Value.Id }),
                value: cr.Value.ToGetBook(service)),
            Error<Book, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PutBook(Guid id, [FromBody]PutBook putBook, BookHandlerService service)
        => service.SaveBook(id, putBook) switch
        {
            Created<Book> cr => Created(
                uri: service.GetLink(GetBook, new { cr.Value.Id }),
                value: cr.Value.ToGetBook(service)),
            Updated<Book> ok => Ok(ok.Value.ToGetBook(service)),
            Error<Book, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PatchBook(Guid id, [FromBody]PatchBook patchBook, BookHandlerService service)
        => service.UpdateBook(id, patchBook) switch
        {
            Ok<Book> ok => Ok(ok),
            Error<Book, string> err => BadRequest(err),
            _ => StatusCode(500)
        };


    public static IResult DeleteBook(Guid id, BookHandlerService service)
        => service.DeleteBook(id) is Ok<Book>
        ? Ok(new { id })
        : NotFound(new { id });
    #endregion Handlers
}

public static class BookHandlerExtenstions
{
    public static IEnumerable<GetBook> ToGetBooks(this IEnumerable<Book> books, BookHandlerService service)
        => books.Select(x => x.ToGetBook(service));

    public static GetBook ToGetBook(this Book book, BookHandlerService service)
        => book.ToGetBook(new
        {
            self = service.Context.GetLink(BookHandlers.GetBook, new { book.Id }),
            books = service.Context.GetLink(BookHandlers.GetBookAuthors, new { book.Id }),
        });
}
