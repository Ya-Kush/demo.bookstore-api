using App.Common;

namespace App.Services.Errors;

public sealed record class NotFoundError(string Message = "Not Found") : Error(Message);
