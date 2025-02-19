using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Data;

public sealed class UserContext(DbContextOptions<UserContext> options) : IdentityUserContext<User, Guid>(options);