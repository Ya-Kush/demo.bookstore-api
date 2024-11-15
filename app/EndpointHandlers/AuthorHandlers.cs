using App.Common;
using App.DomainModels;
using App.EndpointModels;
using App.EndpointServices;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.EndpointHandlers;

public static class AuthorHandlers
{
    public static T MapAuthors<T>(this T routeBuilder) where T : IEndpointRouteBuilder => routeBuilder.MapSub(x => x.Map(
        "/authors", GetAuthors, PostAuthor).MapSub(x => x.Map(
            "/{id:guid}", GetAuthor, PutAuthor, PatchAuthor, DeleteAuthor).MapSub(x => x.Map(
                "/books", GetAuthorBooks) ) ) );

    #region Handlers
    public static IResult GetAuthors(AuthorHandlerService service)
        => Ok(service.UntrackedAuthors.ToGetAuthors(service));

    public static IResult GetAuthor(Guid id, AuthorHandlerService service)
        => service.FindUntracked(id) switch
        {
            Ok<Author> ok => Ok(ok.Value.ToGetAuthor(service)),
            NotFound<Author> => NotFound(new { id }),
            _ => StatusCode(500)
        };

    public static IResult GetAuthorBooks(Guid id, AuthorHandlerService service, BookHandlerService bookService)
        => service.FindAuthorBooks(id) switch
        {
            Ok<IEnumerable<Book>> books => Ok(books.Value.ToGetBooks(bookService)),
            NotFound<IEnumerable<Book>> => NotFound(new { id }),
            _ => StatusCode(500)
        };


    public static IResult PostAuthor([FromBody]PostAuthor postAuthor, AuthorHandlerService service)
        => service.SaveNewAuthor(postAuthor) switch
        {
            Created<Author> cr => Created(
                uri: service.GetLink(GetAuthor, new { cr.Value.Id }),
                value: cr.Value.ToGetAuthor(service)),
            Error<Author, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PutAuthor(Guid id, [FromBody]PutAuthor putAuthor, AuthorHandlerService service)
        => service.SaveAuthor(id, putAuthor) switch
        {
            Created<Author> cr => Created(
                uri: service.GetLink(GetAuthor, new { cr.Value.Id }),
                value: cr.Value.ToGetAuthor(service)),
            Updated<Author> ok => Ok(ok.Value.ToGetAuthor(service)),
            Error<Author, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PatchAuthor(Guid id, [FromBody]PatchAuthor patchAuthor, AuthorHandlerService service)
        => service.UpdateAuthor(id, patchAuthor) switch
        {
            Ok<Author> ok => Ok(ok),
            Error<Author, string> err => BadRequest(err),
            _ => StatusCode(500)
        };


    public static IResult DeleteAuthor(Guid id, AuthorHandlerService service)
        => service.DeleteAuthor(id) is Ok<Author>
        ? Ok(new { id })
        : NotFound(new { id });
    #endregion Handlers
}

public static class AuthorHandlerExtenstions
{
    public static IEnumerable<GetAuthor> ToGetAuthors(this IEnumerable<Author> authors, AuthorHandlerService service)
        => authors.Select(x => x.ToGetAuthor(service));

    public static GetAuthor ToGetAuthor(this Author author, AuthorHandlerService service)
        => author.ToGetAuthor(new
        {
            self = service.Context.GetLink(AuthorHandlers.GetAuthor, new { author.Id }),
            books = service.Context.GetLink(AuthorHandlers.GetAuthorBooks, new { author.Id }),
        });
}
