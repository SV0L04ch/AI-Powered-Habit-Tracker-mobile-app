using Xunit;
using Moq;
using RichardSzalay.MockHttp;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using HabitApi.Services;
using System.Net;

namespace HabitApi.Tests.Services;

public class AiInsightsServiceTests
{
    private static AiInsightsService CreateService(HttpClient httpClient)
    {
        var cacheOptions = Options.Create(new MemoryCacheOptions());
        var cache = new MemoryCache(cacheOptions);
        return new AiInsightsService(httpClient, cache);
    }

    [Fact]
    public async Task GetAiInsightAsync_ValidRequest_ReturnsParsedContent()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openai.com/*")
                .Respond("application/json", "{ \"choices\": [{ \"text\": \"Отличная работа!\" }] }");

        var httpClient = mockHttp.ToHttpClient();
        var service = CreateService(httpClient);

        var result = await service.GetAiInsightAsync("Пользователь выполнил все привычки.");

        Assert.NotNull(result);
        Assert.Contains("Отличная работа", result);
    }

    [Fact]
    public async Task GetAiInsightAsync_ApiError_Throws()
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://api.openai.com/*")
                .Respond(HttpStatusCode.InternalServerError);

        var httpClient = mockHttp.ToHttpClient();
        var service = CreateService(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetAiInsightAsync("test"));
    }
}