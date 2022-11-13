# PTI.Microservices.Library.AzureTranslator

This is part of PTI.Microservices.Library set of packages

The purpose of this package is to facilitate the calls to Azure Translator APIs, while maintaining a consistent usage pattern among the different services in the group

**Examples:**

## Translate Single Item

    AzureTranslatorService azureTranslatorService = GetInstance();
    var result = await azureTranslatorService
        .TranslateSimpleTextAsync(
        textToTranslate: "Me gusta la pasta", inputLanguageCode:
        "es", outputLanguageCode: "en");

## Translate Multiple Items

    TranslateRequestTextItem[] items = new TranslateRequestTextItem[] {
                    new TranslateRequestTextItem(){ Text="They were playing" },
                    new TranslateRequestTextItem(){ Text="I want to eat" }
                };
                var result = await azureTranslatorService.TranslateMultipleItemsAsync(items, 
                    inputLanguageCode:"en", outputLanguageCode:"es");
