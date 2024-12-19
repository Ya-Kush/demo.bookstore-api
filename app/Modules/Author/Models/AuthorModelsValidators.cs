using App.Common;

namespace App.Endpoints.Models.Validators;

public static class AuthorModelValidators
{
    public static ValidationResult<PostAuthor, string> SimpleValidate(this PostAuthor book)
        => new(book, x =>
        {
            var res = new List<string>();
            if (x.FirstName.IsNullOrWhiteSpace()) res.Add("FirstName is wrong");
            if (x.MiddleName.IsNullOrWhiteSpace()) res.Add("MiddleName is wrong");
            if (x.LastName.IsNullOrWhiteSpace()) res.Add("LastName is wrong");
            return res;
        });

    public static ValidationResult<PutAuthor, string> SimpleValidate(this PutAuthor book)
        => new(book, x =>
        {
            var res = new List<string>();
            if (x.FirstName.IsNullOrWhiteSpace()) res.Add("FirstName is wrong");
            if (x.MiddleName.IsNullOrWhiteSpace()) res.Add("MiddleName is wrong");
            if (x.LastName.IsNullOrWhiteSpace()) res.Add("LastName is wrong");
            return res;
        });

    public static ValidationResult<PatchAuthor, string> SimpleValidate(this PatchAuthor book)
        => new(book, x =>
        {
            var res = new List<string>();
            if (x.FirstName is not null && x.FirstName.IsNullOrWhiteSpace()) res.Add("FirstName is wrong");
            if (x.MiddleName is not null && x.MiddleName.IsNullOrWhiteSpace()) res.Add("MiddleName is wrong");
            if (x.LastName is not null && x.LastName.IsNullOrWhiteSpace()) res.Add("LastName is wrong");
            return res;
        });
}
