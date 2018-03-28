using System.Net.Http;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

using TextMood.Backend.Common;

namespace TextMood.Functions
{
	public static class GetTextModels
	{
		[FunctionName(nameof(GetTextModels))]
		public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage httpRequest, TraceWriter log)
		{
			log.Info("Retrieving Text Models from Database");
			try
			{
				var textModelList = TextMoodDatabase.GetAllTextModels().ConfigureAwait(false);
				return httpRequest.CreateResponse(System.Net.HttpStatusCode.OK, textModelList);
			}
			catch (System.Exception e)
			{
				return httpRequest.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, e);
			}
		}
	}
}
