using App.Data;

namespace App.EndpointHandlers;

public sealed class EndpointHandlerContext(BookstoreDbContext db, IHttpContextAccessor hca, LinkGenerator lg)
{
    public BookstoreDbContext DbContext { get; } = db;
    public HttpContext HttpContext { get; } = hca.HttpContext ?? throw new NullReferenceException();
    public LinkGenerator LinkGenerator { get; } = lg;
}
