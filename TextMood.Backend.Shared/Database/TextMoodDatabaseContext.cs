using Microsoft.EntityFrameworkCore;
using TextMood.Backend.Common;

namespace TextMood
{
    public class TextMoodDatabaseContext : DbContext
    {
        public TextMoodDatabaseContext(DbContextOptions<TextMoodDatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<TextMoodModel> TextMoodModels => Set<TextMoodModel>();
    }
}

