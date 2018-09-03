using System;
using System.Linq;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

using NPoco;

using Microsoft.AspNet.OData;

namespace TextMood.Backend.Common
{
    public static class TextMoodDatabase
    {
        #region Constant Fields
        readonly static string _connectionString = Environment.GetEnvironmentVariable("TextMoodDatabaseConnectionString");
        #endregion

        #region Methods
        public static Task<List<TextMoodModel>> GetAllTextModels()
        {
            return PerformDatabaseFunction(getAllTextModelsFunction);

            Task<List<TextMoodModel>> getAllTextModelsFunction(Database dataContext) => dataContext.FetchAsync<TextMoodModel>();
        }

        public static Task<TextMoodModel> GetTextModel(string id)
        {
            return PerformDatabaseFunction(getTextModelFunction);

            async Task<TextMoodModel> getTextModelFunction(Database dataContext)
            {
                var allTextModels = await GetAllTextModels().ConfigureAwait(false);
                return allTextModels.Where(x => x.Id.Equals(id)).FirstOrDefault();
            }
        }

        public static Task<TextMoodModel> InsertTextModel(TextMoodModel text)
        {
            return PerformDatabaseFunction(insertTextModelFunction);

            async Task<TextMoodModel> insertTextModelFunction(Database dataContext)
            {
                if (string.IsNullOrWhiteSpace(text.Id))
                    text.Id = Guid.NewGuid().ToString();

                text.CreatedAt = DateTimeOffset.UtcNow;
                text.UpdatedAt = DateTimeOffset.UtcNow;

                await dataContext.InsertAsync(text).ConfigureAwait(false);

                return text;
            }
        }

        public static Task<TextMoodModel> PatchTextModel(TextMoodModel text)
        {
            var textModelDelta = new Delta<TextMoodModel>();

            textModelDelta.TrySetPropertyValue(nameof(TextMoodModel.Text), text.Text);
            textModelDelta.TrySetPropertyValue(nameof(TextMoodModel.IsDeleted), text.IsDeleted);

            return PatchTextModel(text.Id, textModelDelta);
        }

        public static Task<TextMoodModel> PatchTextModel(string id, Delta<TextMoodModel> text)
        {
            return PerformDatabaseFunction(patchTextModelFunction);

            async Task<TextMoodModel> patchTextModelFunction(Database dataContext)
            {
                var textFromDatabase = dataContext.Fetch<TextMoodModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                text.Patch(textFromDatabase);
                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                await dataContext.UpdateAsync(textFromDatabase).ConfigureAwait(false);

                return textFromDatabase;
            }

        }

        public static Task<TextMoodModel> DeleteTextModel(string id)
        {
            return PerformDatabaseFunction(deleteTextModelFunction);

            async Task<TextMoodModel> deleteTextModelFunction(Database dataContext)
            {
                var textFromDatabase = dataContext.Fetch<TextMoodModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                textFromDatabase.IsDeleted = true;
                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                await dataContext.UpdateAsync(textFromDatabase).ConfigureAwait(false);

                return textFromDatabase;
            }
        }

        public static Task<TextMoodModel> RemoveTextModel(string id)
        {
            return PerformDatabaseFunction(removetextDatabaseFunction);

            async Task<TextMoodModel> removetextDatabaseFunction(Database dataContext)
            {
                var textFromDatabase = dataContext.Fetch<TextMoodModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                await dataContext.DeleteAsync(textFromDatabase).ConfigureAwait(false);

                return textFromDatabase;
            }
        }

        static TResult PerformDatabaseFunction<TResult>(Func<Database, TResult> databaseFunction) where TResult : class
        {
            using (var connection = new Database(_connectionString, DatabaseType.SqlServer2012, SqlClientFactory.Instance))
            {
                try
                {
                    return databaseFunction?.Invoke(connection) ?? default;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.ToString());
                    Debug.WriteLine("");

                    throw;
                }
            }
        }
    }
    #endregion
}