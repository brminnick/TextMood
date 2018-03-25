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
        public static Task<IList<TextModel>> GetAllTextModels()
		{
            Func<DataContext, IList<TextModel>> getAllTextModelsFunction = dataContext => dataContext.GetTable<TextModel>().ToList();
            return PerformDatabaseFunction(getAllTextModelsFunction);
        }

        public static Task<TextModel> GetTextModel(string id)
        {
            Func<DataContext, TextModel> getTextModelFunction = dataContext => dataContext.GetTable<TextModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();
            return PerformDatabaseFunction(getTextModelFunction);
        }

        public static Task<TextModel> InsertTextModel(TextModel text)
        {
            Func<DataContext, TextModel> insertTextModelFunction = dataContext =>
            {
                if (string.IsNullOrWhiteSpace(text.Id))
                    text.Id = Guid.NewGuid().ToString();

                text.CreatedAt = DateTimeOffset.UtcNow;
                text.UpdatedAt = DateTimeOffset.UtcNow;

                dataContext.GetTable<TextModel>().InsertOnSubmit(text);

                return text;
            };

            return PerformDatabaseFunction(insertTextModelFunction);
        }

        public static Task<TextModel> PatchTextModel(TextModel text)
        {
            var textModelDelta = new Delta<TextModel>();

            textModelDelta.TrySetPropertyValue(nameof(TextModel.Text), text.Text);
            textModelDelta.TrySetPropertyValue(nameof(TextModel.IsDeleted), text.IsDeleted);

            return PatchTextModel(text.Id, textModelDelta);
        }

        public static Task<TextModel> PatchTextModel(string id, Delta<TextModel> text)
        {
            Func<DataContext, TextModel> patchTextModelFunction = dataContext =>
            {
                var textFromDatabase = dataContext.GetTable<TextModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                text.Patch(textFromDatabase);
                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                return textFromDatabase;
            };

            return PerformDatabaseFunction(patchTextModelFunction);
        }

        public static Task<TextModel> DeleteTextModel(string id)
        {
            Func<DataContext, TextModel> deleteTextModelFunction = dataContext =>
            {
                var textFromDatabase = dataContext.GetTable<TextModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                textFromDatabase.IsDeleted = true;
                textFromDatabase.UpdatedAt = DateTimeOffset.UtcNow;

                return textFromDatabase;
            };

            return PerformDatabaseFunction(deleteTextModelFunction);
        }

        public static Task<TextModel> RemoveTextModel(string id)
        {
            Func<DataContext, TextModel> removetextDatabaseFunction = dataContext =>
            {
                var textFromDatabase = dataContext.GetTable<TextModel>().Where(x => x.Id.Equals(id)).FirstOrDefault();

                dataContext.GetTable<TextModel>().DeleteOnSubmit(textFromDatabase);

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