using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.AzureTranslator.Models.TranslateDocuments;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.Models.AzureTranslator.Translate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTI.Microservices.Library.Services
{
    /// <summary>
    /// Service in charge of exposing access to Azure Translator
    /// </summary>
    public class AzureTranslatorService
    {
        private ILogger<AzureTranslatorService> Logger { get; }
        private AzureTranslatorConfiguration AzureTranslatorConfiguration { get; }
        private CustomHttpClient CustomHttpClient { get; }
        /// <summary>
        /// Creates a new instance of <see cref="AzureTranslatorService"/>
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="azureTranslatorConfiguration"></param>
        /// <param name="customHttpClient"></param>
        public AzureTranslatorService(ILogger<AzureTranslatorService> logger,
            AzureTranslatorConfiguration azureTranslatorConfiguration, CustomHttpClient customHttpClient)
        {
            this.Logger = logger;
            this.AzureTranslatorConfiguration = azureTranslatorConfiguration;
            this.CustomHttpClient = customHttpClient;
        }

        /// <summary>
        /// Name of the language for translations
        /// </summary>
        public enum AzureTranslatorLanguage
        {
            /// <summary>
            /// Spanish Language
            /// </summary>
            Spanish = 0,
            /// <summary>
            /// English Language
            /// </summary>
            English = 1
        }

        private static string GetLanguageString(AzureTranslatorLanguage outputLanguage)
        {
            string languageString = string.Empty;
            switch (outputLanguage)
            {
                case AzureTranslatorLanguage.Spanish:
                    languageString = "es";
                    break;
                case AzureTranslatorLanguage.English:
                    languageString = "en";
                    break;
            }

            return languageString;
        }

        /// <summary>
        /// Translate a given text
        /// </summary>
        /// <param name="textToTranslate"></param>
        /// <param name="inputLanguage"></param>
        /// <param name="outputLanguage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <remarks>Check supported languages here: https://docs.microsoft.com/en-us/azure/cognitive-services/translator/language-support</remarks>
        public async Task<TranslateResponseLanguageInforomation[]> TranslateSimpleTextAsync(string textToTranslate,
            string inputLanguageCode,
            string outputLanguageCode,
            CancellationToken cancellationToken = default)
        {
            try
            {
                this.CustomHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.AzureTranslatorConfiguration.Key);
                TranslateRequestTextItem[] model = new TranslateRequestTextItem[1]
                {
                    new TranslateRequestTextItem(){ Text = textToTranslate }
                };
                var postresult =
                    await this.CustomHttpClient.PostAsJsonAsync<TranslateRequestTextItem[]>($"{this.AzureTranslatorConfiguration.Endpoint}/translate?" +
                    $"api-version=3.0&from={inputLanguageCode}&to={outputLanguageCode}&textType=plain",
                    model);
                if (postresult.IsSuccessStatusCode)
                {
                    var resultString = await postresult.Content.ReadAsStringAsync();
                    var translatedData = await postresult.Content.ReadFromJsonAsync<TranslateResponseLanguageInforomation[]>();
                    return translatedData;
                }
                else
                {
                    var errorContentString = await postresult.Content.ReadAsStringAsync();
                    throw new Exception(errorContentString);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                throw;
            }
        }

        /// <summary>
        /// Translates a set of specified texts.
        /// Check https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-translate#request-body
        /// The array can have at most 100 elements.
        /// The entire text included in the request cannot exceed 10,000 characters including spaces.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="inputLanguage"></param>
        /// <param name="outputLanguage"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TranslateResponseLanguageInforomation[]> TranslateMultipleItemsAsync(TranslateRequestTextItem[] model,
            string inputLanguageCode,
            string outputLanguageCode,
            CancellationToken cancellationToken = default)
        {
            try
            {
                this.CustomHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.AzureTranslatorConfiguration.Key);
                var postresult =
                    await this.CustomHttpClient.PostAsJsonAsync<TranslateRequestTextItem[]>($"{this.AzureTranslatorConfiguration.Endpoint}/translate?" +
                    $"api-version=3.0&from={inputLanguageCode}&to={outputLanguageCode}&textType=plain",
                    model);
                if (postresult.IsSuccessStatusCode)
                {
                    var resultString = await postresult.Content.ReadAsStringAsync();
                    var translatedData = await postresult.Content.ReadFromJsonAsync<TranslateResponseLanguageInforomation[]>();
                    return translatedData;
                }
                else
                {
                    var errorContentString = await postresult.Content.ReadAsStringAsync();
                    throw new Exception(errorContentString);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                throw;
            }
        }


        public async Task TranslateDocumentsAsync(TranslateDocumentsRequest model,
            TimeSpan timetoWaitBetweenStatusRequests,
            CancellationToken cancellationToken=default)
        {
            try
            {
                string requestUrl = $"https://{this.AzureTranslatorConfiguration.ResourceName}.cognitiveservices.azure.com" +
                    $"/translator/text/batch/v1.0/batches";
                this.CustomHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", this.AzureTranslatorConfiguration.Key);
                var postResponse = await this.CustomHttpClient.PostAsJsonAsync<TranslateDocumentsRequest>(requestUrl, model,
                    cancellationToken: cancellationToken);
                if (postResponse.IsSuccessStatusCode)
                {
                    TranslateDocumentsStatusResponse translateDocumentsStatusResponse = null;
                    do
                    {
                        var operationLocation =postResponse.Headers.GetValues("Operation-Location").Single();
                        translateDocumentsStatusResponse = await GetTranslateDocumentsStatusAsync(operationLocation,
                            timetoWaitBetweenStatusRequests:timetoWaitBetweenStatusRequests,
                            cancellationToken: cancellationToken);
                    } while (translateDocumentsStatusResponse.status != "Canceled" &&
                    translateDocumentsStatusResponse.status != "Failed" &&
                    translateDocumentsStatusResponse.status != "Succeeded");
                }
                else
                {
                    var error = await postResponse.Content.ReadAsStringAsync(cancellationToken: cancellationToken);
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                throw;
            }
        }

        public async Task<TranslateDocumentsStatusResponse> GetTranslateDocumentsStatusAsync(string operationLocation, 
            TimeSpan timetoWaitBetweenStatusRequests, CancellationToken cancellationToken)
        {
            try
            {
                var result = await this.CustomHttpClient
                    .GetFromJsonAsync<TranslateDocumentsStatusResponse>(operationLocation, cancellationToken:cancellationToken);
                return result;
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(ex.Message, ex);
                throw;
            }
        }
    }
}
