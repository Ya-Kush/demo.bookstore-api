using App.Common;
using App.EndpointModels;

namespace App.EndpointModelValidators;

public static class BookModelValidators
{
    public static ValidateResult<PostBook, string> SimpleValidate(this PostBook book) => new(book, x =>
    {
        var res = new List<string>();
        if (x.Title.IsNullOrWhiteSpace()) res.Add("Title is wrong");
        if (x.Edition.IsNullOrWhiteSpace()) res.Add("Edition is wrong");
        if (x.Price <= 0) res.Add("Price is lower than zero");
        return res;
    });

    public static ValidateResult<PutBook, string> SimpleValidate(this PutBook book) => new(book, x =>
    {
        var res = new List<string>();
        if (x.Title.IsNullOrWhiteSpace()) res.Add("Title is wrong");
        if (x.Edition.IsNullOrWhiteSpace()) res.Add("Edition is wrong");
        if (x.Price <= 0) res.Add("Price is lower than zero");
        return res;
    });

    public static ValidateResult<PatchBook, string> SimpleValidate(this PatchBook book) => new(book, x =>
    {
        var res = new List<string>();
        if (x.Title is not null && x.Title.IsNullOrWhiteSpace()) res.Add("Title is wrong");
        if (x.Edition is not null && x.Edition.IsNullOrWhiteSpace()) res.Add("Edition is wrong");
        if (x.Price is not null && x.Price <= 0) res.Add("Price is lower than zero");
        return res;
    });
}
