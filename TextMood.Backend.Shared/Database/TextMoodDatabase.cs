using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TextMood.Backend.Common
{
    public class TextMoodDatabase
    {
        readonly static string _connectionString = Environment.GetEnvironmentVariable("TextMoodDatabaseConnectionString") ?? string.Empty;
        readonly TextMoodDatabaseContext _databaseContext;

        public TextMoodDatabase(TextMoodDatabaseContext databaseContext) => _databaseContext = databaseContext;

        public Task<TextMoodModel> GetTextModel(string id) => _databaseContext.TextMoodModels.SingleAsync(x => x.Id.Equals(id));

        public async Task<IReadOnlyList<TextMoodModel>> GetAllTextModels()
        {
            var textMoodModels = await _databaseContext.TextMoodModels.ToListAsync().ConfigureAwait(false);
            return textMoodModels.ToList();
        }

        public async Task<TextMoodModel> InsertTextModel(TextMoodModel text)
        {
            if (string.IsNullOrWhiteSpace(text.Id))
                text.Id = Guid.NewGuid().ToString();

            text.CreatedAt = DateTimeOffset.UtcNow;
            text.UpdatedAt = DateTimeOffset.UtcNow;

            await _databaseContext.AddAsync(text).ConfigureAwait(false);
            await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return text;
        }

        public async Task<TextMoodModel> PatchTextModel(TextMoodModel text)
        {
            var textFromDatabase = await GetTextModel(text.Id).ConfigureAwait(false);

            textFromDatabase.Id = text.Id;
            textFromDatabase.SentimentScore = text.SentimentScore;
            textFromDatabase.IsDeleted = text.IsDeleted;

            textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

            _databaseContext.Update(textFromDatabase);
            await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return textFromDatabase;
        }

        public async Task<TextMoodModel> DeleteTextModel(string id)
        {
            var textFromDatabase = await GetTextModel(id).ConfigureAwait(false);

            textFromDatabase.IsDeleted = true;
            textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

            _databaseContext.Update(textFromDatabase);
            await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return textFromDatabase;
        }

        public async Task<TextMoodModel> RemoveTextModel(string id)
        {
            var textFromDatabase = await GetTextModel(id).ConfigureAwait(false);

            _databaseContext.Remove(textFromDatabase);
            await _databaseContext.SaveChangesAsync().ConfigureAwait(false);

            return textFromDatabase;
        }
    }
}