using App.Common;
using App.Data;
using App.Data.Models;
using App.Endpoints.Models;
using App.Endpoints.Services;
using App.Services.Errors;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace App.Endpoints;

public static class AuthorEndpoints
{
    public static T MapAuthors<T>(this T routeBuilder) where T : IEndpointRouteBuilder
    {
        return routeBuilder.MapSub(x => x.Map(
        // "/some-part", () => "request"), x => x.Map(
        "/authors", GetAuthors, PostAuthor).MapSub(x => x.Map(
            "/{authorId:guid}", GetAuthor, PatchAuthor, DeleteAuthor).MapSub(x => x.Map(
                "/books", GetAuthorBooks).MapSub(x => x.Map(
                    "/{bookId:guid}", PutAuthorBook, DeleteAuthorBook)))));
    }

    #region Handlers
    public static IResult GetAuthors(AuthorRepo authorRepo, EndpointContext context)
    {
        return Ok(new { data = authorRepo.Authors.Untrack().ToGetAuthors(context) });
    }

    public static IResult PostAuthor([FromBody] PostAuthor postAuthor, AuthorRepo authorRepo, EndpointContext context)
    {
        var res = authorRepo.SaveNewAuthor(postAuthor);
        return res.Match<IResult>(
            author => Created(
                uri: context.GetLink(GetAuthor, new { author.Id }),
                value: author.ToGetAuthor(context)),
            err => BadRequest(err.Message));
    }


    public static IResult GetAuthor(Guid authorId, AuthorRepo authorRepo, EndpointContext context)
    {
        var res = authorRepo.FindAuthor(authorId);
        return res.Match<IResult>(
            author => Ok(author.ToGetAuthor(context)),
            err => NotFound());
    }

    public static IResult PatchAuthor(Guid authorId, [FromBody] PatchAuthor patchAuthor, AuthorRepo authorRepo)
    {
        var res = authorRepo.UpdateAuthor(authorId, patchAuthor);
        return res.Match<IResult>(
            author => Ok(),
            err => err switch
            {
                NotFoundError => NotFound(),
                _ => BadRequest(err.Message)
            });
    }

    public static IResult DeleteAuthor(Guid authorId, AuthorRepo authorRepo)
    {
        return authorRepo.DeleteAuthor(authorId).IsOk
            ? Ok(new { authorId })
            : NotFound(new { authorId });
    }


    public static IResult GetAuthorBooks(Guid authorId, AuthorRepo authorRepo, EndpointContext context)
    {
        var res = authorRepo.FindAuthorBooks(authorId);
        return res.Match<IResult>(
            books => Ok(books.ToGetBooks(context)),
            err => NotFound());
    }


    public static IResult PutAuthorBook(Guid authorId, Guid bookId, AuthorRepo authorRepo)
    {
        var res = authorRepo.AddBookToAuthor(authorId, bookId);
        return res.Match<IResult>(
            Ok,
            err => NotFound());
    }

    public static IResult DeleteAuthorBook(Guid authorId, Guid bookId, AuthorRepo authorRepo)
    {
        var res = authorRepo.RemoveBookFromAuthor(authorId, bookId);
        return res.Match<IResult>(
            Ok,
            err => NotFound());
    }
    #endregion Handlers
}

public static class AuthorEnpointsExtenstions
{
    public static IEnumerable<GetAuthor> ToGetAuthors(this IEnumerable<Author> authors, EndpointContext context)
    {
        return authors.Select(x => x.ToGetAuthor(context));
    }

    public static GetAuthor ToGetAuthor(this Author author, EndpointContext context)
    {
        return author.ToGetAuthor(new
        {
            self = context.GetLink(AuthorEndpoints.GetAuthor, new { authorId = author.Id }),
            books = context.GetLink(AuthorEndpoints.GetAuthorBooks, new { authorId = author.Id }),
        });
    }
}
