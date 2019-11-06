using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;

namespace HearSay.Services
{
    static class TextAnalysisService
    {
        public const string TextSentimentAPIKey = "0f24dab2ec9647d3b45bc99caf576507";
        public const string BaseUrl = "https://sentimentan.cognitiveservices.azure.com/";

        readonly static Lazy<TextAnalyticsClient> _textAnalyticsApiClientHolder = new Lazy<TextAnalyticsClient>(() => new TextAnalyticsClient(new ApiKeyServiceClientCredentials(TextSentimentAPIKey)) { Endpoint = "https://sentimentan.cognitiveservices.azure.com/" });
        static TextAnalyticsClient TextAnalyticsApiClient => _textAnalyticsApiClientHolder.Value;


        public static async Task<double?> GetSentiment(string text)
        {
            var sentimentResults = await TextAnalyticsApiClient.SentimentAsync(text, "en");
            Console.WriteLine("SENTIMENT ANALYSIS SCORE == " +sentimentResults.Score);
            var score = sentimentResults?.Score;
            return score;
        }


    }

    //Client authentication
    class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        readonly string _subscriptionKey;
        public ApiKeyServiceClientCredentials(string subscriptionKey) => _subscriptionKey = subscriptionKey;
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
            return Task.CompletedTask;
        }
    }
}
