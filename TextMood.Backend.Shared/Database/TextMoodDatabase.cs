using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TextMood.Backend.Common
{
    public class TextMoodDatabase
    {
        readonly static string _connectionString = Environment.GetEnvironmentVariable("TextMoodDatabaseConnectionString") ?? string.Empty;

        public async Task<List<TextMoodModel>> GetAllTextModels()
        {
            using var context = new DatabaseContext();
            return await context.TextMoodModels.ToListAsync().ConfigureAwait(false);
        }

        public async Task<TextMoodModel> GetTextModel(string id)
        {
            using var context = new DatabaseContext();
            return await context.TextMoodModels.SingleAsync(x => x.Id.Equals(id)).ConfigureAwait(false);
        }

        public Task<TextMoodModel> InsertTextModel(TextMoodModel text)
        {
            return PerformDatabaseWrite(insertTextModelFunction);

            async Task<TextMoodModel> insertTextModelFunction(DatabaseContext context)
            {
                if (string.IsNullOrWhiteSpace(text.Id))
                    text.Id = Guid.NewGuid().ToString();

                text.CreatedAt = DateTimeOffset.UtcNow;
                text.UpdatedAt = DateTimeOffset.UtcNow;

                await context.AddAsync(text).ConfigureAwait(false);

                return text;
            }
        }

        public Task<TextMoodModel> PatchTextModel(TextMoodModel text)
        {
            return PerformDatabaseWrite(patchTextModelFunction);

            async Task<TextMoodModel> patchTextModelFunction(DatabaseContext context)
            {
                var textFromDatabase = await GetTextModel(text.Id).ConfigureAwait(false);

                textFromDatabase.Id = text.Id;
                textFromDatabase.SentimentScore = text.SentimentScore;
                textFromDatabase.IsDeleted = text.IsDeleted;

                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                context.Update(textFromDatabase);

                return textFromDatabase;
            }
        }

        public Task<TextMoodModel> DeleteTextModel(string id)
        {
            return PerformDatabaseWrite(deleteTextModelFunction);

            async Task<TextMoodModel> deleteTextModelFunction(DatabaseContext context)
            {
                var textFromDatabase = await GetTextModel(id).ConfigureAwait(false);

                textFromDatabase.IsDeleted = true;
                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                context.Update(textFromDatabase);

                return textFromDatabase;
            }
        }

        public Task<TextMoodModel> RemoveTextModel(string id)
        {
            return PerformDatabaseWrite(removetextDatabaseFunction);

            async Task<TextMoodModel> removetextDatabaseFunction(DatabaseContext context)
            {
                var textFromDatabase = await GetTextModel(id).ConfigureAwait(false);

                context.Remove(textFromDatabase);

                return textFromDatabase;
            }
        }

        static async Task<TResult> PerformDatabaseWrite<TResult>(Func<DatabaseContext, Task<TResult>> databaseFunction) where TResult : class?
        {
            using var connection = new DatabaseContext();

            var result = await databaseFunction(connection).ConfigureAwait(false);
            await connection.SaveChangesAsync().ConfigureAwait(false);

            return result;
        }

        class DatabaseContext : DbContext
        {
            public DatabaseContext() => Database.EnsureCreated();

            public DbSet<TextMoodModel>? TextMoodModels { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}