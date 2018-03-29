using System;
using System.Linq;
using System.Data.Linq;
using System.Diagnostics;
using System.Configuration;
using System.Web.Http.OData;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TextMood.Backend.Common
{
	public static class TextMoodDatabase
    {
        #region Constant Fields
        readonly static string _connectionString = ConfigurationManager.ConnectionStrings["TextMoodDatabaseConnectionString"].ConnectionString;
        #endregion

        #region Methods
        public static Task<IList<TextMoodModel>> GetAllTextModels()
		{
            Func<DataContext, IList<TextMoodModel>> getAllTextModelsFunction = dataContext => dataContext.GetTable<TextMoodModel>().ToList();
            return PerformDatabaseFunction(getAllTextModelsFunction);
        }

        public static Task<TextMoodModel> GetTextModel(string id)
        {
            Func<DataContext, TextMoodModel> getTextModelFunction = dataContext => dataContext.GetTable<TextMoodModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();
            return PerformDatabaseFunction(getTextModelFunction);
        }

        public static Task<TextMoodModel> InsertTextModel(TextMoodModel text)
        {
            Func<DataContext, TextMoodModel> insertTextModelFunction = dataContext =>
            {
                if (string.IsNullOrWhiteSpace(text.Id))
                    text.Id = Guid.NewGuid().ToString();

                text.CreatedAt = DateTimeOffset.UtcNow;
                text.UpdatedAt = DateTimeOffset.UtcNow;

                dataContext.GetTable<TextMoodModel>().InsertOnSubmit(text);

                return text;
            };

            return PerformDatabaseFunction(insertTextModelFunction);
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
            Func<DataContext, TextMoodModel> patchTextModelFunction = dataContext =>
            {
                var textFromDatabase = dataContext.GetTable<TextMoodModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                text.Patch(textFromDatabase);
                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                return textFromDatabase;
            };

            return PerformDatabaseFunction(patchTextModelFunction);
        }

        public static Task<TextMoodModel> DeleteTextModel(string id)
        {
            Func<DataContext, TextMoodModel> deleteTextModelFunction = dataContext =>
            {
                var textFromDatabase = dataContext.GetTable<TextMoodModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                textFromDatabase.IsDeleted = true;
                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                return textFromDatabase;
            };

            return PerformDatabaseFunction(deleteTextModelFunction);
        }

        public static Task<TextMoodModel> RemoveTextModel(string id)
        {
            Func<DataContext, TextMoodModel> removetextDatabaseFunction = dataContext =>
            {
                var textFromDatabase = dataContext.GetTable<TextMoodModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                dataContext.GetTable<TextMoodModel>().DeleteOnSubmit(textFromDatabase);

                return textFromDatabase;
            };

            return PerformDatabaseFunction(removetextDatabaseFunction);
        }

        static async Task<TResult> PerformDatabaseFunction<TResult>(Func<DataContext, TResult> databaseFunction) where TResult : class
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var dbContext = new DataContext(connection);

                var signUpTransaction = connection.BeginTransaction();
                dbContext.Transaction = signUpTransaction;

                try
                {
                    return databaseFunction?.Invoke(dbContext) ?? default(TResult);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("");
                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.ToString());
                    Debug.WriteLine("");

                    return default(TResult);
                }
                finally
                {
                    dbContext.SubmitChanges();
                    signUpTransaction.Commit();
                    connection.Close();
                }
            }
        }
        #endregion
    }
}