using App.Common;
using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public static class AuthorEndpoints
{
    public static T MapAuthors<T>(this T routeBuilder) where T : IEndpointRouteBuilder => routeBuilder.MapSub(x => x.Map(
        "/authors", GetAuthors, PostAuthor).MapSub(x => x.Map(
            "/{id:guid}", GetAuthor, PutAuthor, PatchAuthor, DeleteAuthor).MapSub(x => x.Map(
                "/books", GetAuthorBooks) ) ) );

    #region Handlers
    public static IResult GetAuthors(AuthorRepo repo, EndpointContext context)
        => Ok(new { data = repo.UntrackedAuthors.ToGetAuthors(context) });

    public static IResult GetAuthor(Guid id, AuthorRepo repo)
        => repo.FindUntracked(id) switch
        {
            Ok<Author> ok => Ok(ok.Value.ToGetAuthor(repo)),
            NotFound<Author> => NotFound(new { id }),
            _ => StatusCode(500)
        };

    public static IResult GetAuthorBooks(Guid id, AuthorRepo repo, BookRepo bookRepo, EndpointContext context)
        => repo.FindAuthorBooks(id) switch
        {
            Ok<IEnumerable<Book>> books => Ok(books.Value.ToGetBooks(context)),
            NotFound<IEnumerable<Book>> => NotFound(new { id }),
            _ => StatusCode(500)
        };


    public static IResult PostAuthor([FromBody]PostAuthor postAuthor, AuthorRepo repo, EndpointContext context)
        => repo.SaveNewAuthor(postAuthor) switch
        {
            Created<Author> cr => Created(
                uri: context.GetLink(GetAuthor, new { cr.Value.Id }),
                value: cr.Value.ToGetAuthor(repo)),
            Error<Author, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PutAuthor(Guid id, [FromBody]PutAuthor putAuthor, AuthorRepo repo, EndpointContext context)
        => repo.SaveAuthor(id, putAuthor) switch
        {
            Created<Author> cr => Created(
                uri: context.GetLink(GetAuthor, new { cr.Value.Id }),
                value: cr.Value.ToGetAuthor(repo)),
            Updated<Author> ok => Ok(ok.Value.ToGetAuthor(repo)),
            Error<Author, string> err => BadRequest(err),
            _ => StatusCode(500)
        };

    public static IResult PatchAuthor(Guid id, [FromBody]PatchAuthor patchAuthor, AuthorRepo repo)
        => repo.UpdateAuthor(id, patchAuthor) switch
        {
            Ok<Author> ok => Ok(ok),
            Error<Author, string> err => BadRequest(err),
            _ => StatusCode(500)
        };


    public static IResult DeleteAuthor(Guid id, AuthorRepo repo)
        => repo.DeleteAuthor(id) is Ok<Author>
        ? Ok(new { id })
        : NotFound(new { id });
    #endregion Handlers
}

public static class AuthorEnpointsExtenstions
{
    public static IEnumerable<GetAuthor> ToGetAuthors(this IEnumerable<Author> authors, EndpointContext context)
        => authors.Select(x => x.ToGetAuthor(context));

    public static GetAuthor ToGetAuthor(this Author author, EndpointContext context)
        => author.ToGetAuthor(new
        {
            self = context.GetLink(AuthorEndpoints.GetAuthor, new { author.Id }),
            books = context.GetLink(AuthorEndpoints.GetAuthorBooks, new { author.Id }),
        });
}
