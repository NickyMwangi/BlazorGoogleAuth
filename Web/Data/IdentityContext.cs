using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Data;

namespace Web.Data
{
    public class IdentityContext(DbContextOptions<IdentityContext> options) : IdentityDbContext<WebUser>(options)
    {
    }
}
