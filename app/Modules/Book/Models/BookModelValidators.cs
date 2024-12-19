using App.Common.Extensions;

namespace App.Endpoints.Models.Validators;

public static class BookModelValidators
{
    public static ValidationResult<PostBook, string> SimpleValidate(this PostBook book)
        => new(book, x =>
        {
            var res = new List<string>();
            if (x.Title.IsNullOrWhiteSpace()) res.Add("Title is wrong");
            if (x.Edition.IsNullOrWhiteSpace()) res.Add("Edition is wrong");
            if (x.Price <= 0) res.Add("Price is lower than zero");
            return res;
        });

    public static ValidationResult<PutBook, string> SimpleValidate(this PutBook book)
        => new(book, x =>
        {
            var res = new List<string>();
            if (x.Title.IsNullOrWhiteSpace()) res.Add("Title is wrong");
            if (x.Edition.IsNullOrWhiteSpace()) res.Add("Edition is wrong");
            if (x.Price <= 0) res.Add("Price is lower than zero");
            return res;
        });

    public static ValidationResult<PatchBook, string> SimpleValidate(this PatchBook book)
        => new(book, x =>
        {
            var res = new List<string>();
            if (x.Title is not null && x.Title.IsNullOrWhiteSpace()) res.Add("Title is wrong");
            if (x.Edition is not null && x.Edition.IsNullOrWhiteSpace()) res.Add("Edition is wrong");
            if (x.Price is not null && x.Price <= 0) res.Add("Price is lower than zero");
            return res;
        });
}
