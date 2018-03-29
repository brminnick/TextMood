using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.ProjectOxford.Text.Core;
using Microsoft.ProjectOxford.Text.Sentiment;

namespace TextMood.Functions
{
	static class TextAnalysisServices
	{          
		readonly static Lazy<SentimentClient> _sentimentClientHolder = new Lazy<SentimentClient>(() => new SentimentClient(CognitiveServicesConstants.TextSentimentAPIKey));

		static SentimentClient SentimentClient => _sentimentClientHolder.Value;

		public static async Task<float?> GetSentiment(string text)
		{
			var sentimentDocument = new SentimentDocument { Id = "1", Text = text };
			var sentimentRequest = new SentimentRequest { Documents = new List<IDocument> { { sentimentDocument } } };

			var sentimentResults = await SentimentClient.GetSentimentAsync(sentimentRequest).ConfigureAwait(false);
			var documentResult = sentimentResults.Documents.FirstOrDefault();

			return documentResult?.Score;
		}
	}
}
