using Catchy;
using CatchyCoreTests.Helpers;
using System.Net;

namespace CatchyCoreTests.Assertions.Network;

/// <summary>
/// Integration tests for network assertions (HttpResponseMessage, Uri).
/// Covers status codes, headers, content, and URI properties.
/// </summary>
public class NetworkAssertionsTests
{
    // ===== HttpResponseMessage Assertions =====

    [Fact]
    public async Task HttpResponseMessage_StatusCode_Is_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).StatusCode().Is(HttpStatusCode.OK);
    }

    [Fact]
    public async Task HttpResponseMessage_StatusCode_IsSuccessStatusCode_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).IsSuccessfull();
    }

    [Fact]
    public async Task HttpResponseMessage_StatusCode_IsSuccessStatusCode_FailsOnError_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        await Stateless.Assert.That(response).IsSuccessfull();
        });
    }

    [Fact]
    public async Task HttpResponseMessage_StatusCode_IsClientErrorStatusCode_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        await Stateless.Assert.That(response).IsClientError();
    }

    [Fact]
    public async Task HttpResponseMessage_StatusCode_IsServerErrorStatusCode_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        await Stateless.Assert.That(response).IsServerError();
    }

    [Fact]
    public async Task HttpResponseMessage_StatusCode_Is_WithSpecificCode_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.Created);
        await Stateless.Assert.That(response).StatusCode().Is(HttpStatusCode.Created);
    }

    [Fact]
    public async Task HttpResponseMessage_Headers_Contains_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Custom-Header", "test-value");
        await Stateless.Assert.That(response).Headers().Contains("X-Custom-Header");
    }

    [Fact]
    public async Task HttpResponseMessage_Content_IsNotNull_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Content = new StringContent("test");
        await Stateless.Assert.That(response).Content().IsNotNull();
    }

    [Fact]
    public async Task HttpResponseMessage_ReasonPhrase_Is_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { ReasonPhrase = "OK" };
        await Stateless.Assert.That(response).ReasonPhrase().Is("OK");
    }

    [Fact]
    public async Task HttpResponseMessage_RequestMessage_IsNotNull_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { RequestMessage = request };
        await Stateless.Assert.That(response).RequestMessage().IsNotNull();
    }

    // ===== Uri Assertions =====

    [Fact]
    public async Task Uri_Is_WithSameUri_Passes()
    {
        var uri1 = new Uri("https://example.com/path");
        var uri2 = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri1).Is(uri2);
    }

    [Fact]
    public async Task Uri_Is_WithDifferentUri_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var uri1 = new Uri("https://example.com");
        var uri2 = new Uri("https://other.com");
        await Stateless.Assert.That(uri1).Is(uri2);
        });
    }

    [Fact]
    public async Task Uri_Scheme_Is_Passes()
    {
        var uri = new Uri("https://example.com");
        await Stateless.Assert.That(uri).Scheme().Is("https");
    }

    [Fact]
    public async Task Uri_Host_Is_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).Host().Is("example.com");
    }

    [Fact]
    public async Task Uri_Port_Is_Passes()
    {
        var uri = new Uri("https://example.com:8080/path");
        await Stateless.Assert.That(uri).Port().Is(8080);
    }

    [Fact]
    public async Task Uri_Path_Is_Passes()
    {
        var uri = new Uri("https://example.com/api/users");
        await Stateless.Assert.That(uri).Path().Is("/api/users");
    }

    [Fact]
    public async Task Uri_Query_IsEmpty_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).Query().IsEmpty();
    }

    [Fact]
    public async Task Uri_Query_Contains_Passes()
    {
        var uri = new Uri("https://example.com/path?key=value");
        await Stateless.Assert.That(uri).Query().Contains("key=value");
    }

    [Fact]
    public async Task Uri_IsAbsoluteUri_Passes()
    {
        var uri = new Uri("https://example.com");
        await Stateless.Assert.That(uri).IsAbsoluteUri();
    }

    [Fact]
    public async Task Uri_OriginalString_Is_Passes()
    {
        var uri = new Uri("https://example.com/path?query=value");
        await Stateless.Assert.That(uri).OriginalString().StartsWith("https://");
    }

    // ===== AmbientSoft Mode =====

    [Fact]
    public async Task Network_SoftMode_AccumulatesFailures()
    {
        var verify = Asserter.NewSoft();
        using var response = new HttpResponseMessage(HttpStatusCode.OK);

        await verify.That(response).StatusCode().Is(HttpStatusCode.OK);          // Pass
        await verify.That(response).StatusCode().Is(HttpStatusCode.NotFound);    // Fail
        await verify.That(response).IsServerErrorStatusCode();                    // Fail

        if (verify.ErrorCount != 2) throw new AssertionException($"Expected 2 errors, got {verify.ErrorCount}");
    }

    [Fact]
    public async Task HttpContent_HasMediaType_Passes()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).HasMediaType("text/plain");
    }

    [Fact]
    public async Task HttpContent_HasString_Passes()
    {
        using var content = new StringContent("hello world", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).HasString("hello world");
    }

    [Fact]
    public async Task HttpRequestMessage_HasHeaderValue_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        request.Headers.Add("X-Custom-Header", "test-value");
        await Stateless.Assert.That(request).HasHeaderValue("X-Custom-Header", "test-value");
    }

    [Fact]
    public async Task HttpRequestMessage_HasString_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(request).HasString("payload");
    }

    [Fact]
    public async Task HttpStatusCode_IsSuccess_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.OK).IsSuccess();
    }

    [Fact]
    public async Task HttpStatusCode_IsNotServerError_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.OK).IsNotServerError();
    }

    [Fact]
    public async Task HttpResponseMessage_HasStatusCode_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.Created);
        await Stateless.Assert.That(response).HasStatusCode(HttpStatusCode.Created);
    }

    [Fact]
    public async Task HttpResponseMessage_HasHeaderValue_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Custom-Header", "test-value");
        await Stateless.Assert.That(response).HasHeaderValue("X-Custom-Header", "test-value");
    }

    [Fact]
    public async Task HttpResponseMessage_HasContentType_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(response).HasContentType("text/plain");
    }

    [Fact]
    public async Task HttpResponseMessage_HasReasonPhrase_fails_when_missing()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).HasReasonPhrase("Created"));
        Assert.Contains("Created", msg);
    }

    [Fact]
    public async Task Uri_HasScheme_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).HasScheme("https");
    }

    [Fact]
    public async Task Uri_HasHost_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).HasHost("example.com");
    }

    [Fact]
    public async Task Uri_HasPort_Passes()
    {
        var uri = new Uri("https://example.com:8080/path");
        await Stateless.Assert.That(uri).HasPort(8080);
    }

    [Fact]
    public async Task Uri_HasPath_Passes()
    {
        var uri = new Uri("https://example.com/api/users");
        await Stateless.Assert.That(uri).HasPath("/api/users");
    }

    [Fact]
    public async Task Uri_HasQueryParameterValue_Passes()
    {
        var uri = new Uri("https://example.com/path?key=value&x=1");
        await Stateless.Assert.That(uri).HasQueryParameterValue("key", "value");
    }

    [Fact]
    public async Task Uri_HasFragment_Passes()
    {
        var uri = new Uri("https://example.com/path#section");
        await Stateless.Assert.That(uri).HasFragment("#section");
    }

    [Fact]
    public async Task HttpContent_HasHeader_Passes()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        content.Headers.Add("X-Content-Header", "value");
        await Stateless.Assert.That(content).HasHeader("X-Content-Header");
    }

    [Fact]
    public async Task HttpContent_HasHeaderValue_Passes()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        content.Headers.Add("X-Content-Header", "value");
        await Stateless.Assert.That(content).HasHeaderValue("X-Content-Header", "value");
    }

    [Fact]
    public async Task HttpContent_DoesNotHaveMediaType_Passes()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).DoesNotHaveMediaType("application/json");
    }

    [Fact]
    public async Task HttpContent_ContainsString_Passes()
    {
        using var content = new StringContent("hello network", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).ContainsString("network");
    }

    [Fact]
    public async Task HttpContent_HasLength_Passes()
    {
        using var content = new StringContent("abc", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).HasLength(3);
    }

    [Fact]
    public async Task HttpContent_IsEmpty_Passes()
    {
        using var content = new StringContent(string.Empty, System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).IsEmpty();
    }

    [Fact]
    public async Task HttpRequestMessage_HasMediaType_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "application/json")
        };
        await Stateless.Assert.That(request).HasMediaType("application/json");
    }

    [Fact]
    public async Task HttpRequestMessage_HasHeader_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        request.Headers.Add("X-Request-Header", "value");
        await Stateless.Assert.That(request).HasHeader("X-Request-Header");
    }

    [Fact]
    public async Task HttpRequestMessage_DoesNotHaveHeader_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        await Stateless.Assert.That(request).DoesNotHaveHeader("X-Request-Header");
    }

    [Fact]
    public async Task HttpRequestMessage_HasHeaderValue_Passes2()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        request.Headers.Add("X-Request-Header", "value");
        await Stateless.Assert.That(request).HasHeaderValue("X-Request-Header", "value");
    }

    [Fact]
    public async Task HttpRequestMessage_ContainsString_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(request).ContainsString("pay");
    }

    [Fact]
    public async Task HttpRequestMessage_HasLength_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("abc", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(request).HasLength(3);
    }

    [Fact]
    public async Task HttpRequestMessage_IsEmpty_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        await Stateless.Assert.That(request).IsEmpty();
    }

    [Fact]
    public async Task HttpStatusCode_IsNot_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.OK).IsNot(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HttpStatusCode_IsRedirection_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.Redirect).IsRedirection();
    }

    [Fact]
    public async Task HttpStatusCode_IsNotRedirection_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.OK).IsNotRedirection();
    }

    [Fact]
    public async Task HttpStatusCode_IsClientError_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.BadRequest).IsClientError();
    }

    [Fact]
    public async Task HttpStatusCode_IsNotClientError_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.OK).IsNotClientError();
    }

    [Fact]
    public async Task HttpStatusCode_IsServerError_Passes()
    {
        await Stateless.Assert.That(HttpStatusCode.InternalServerError).IsServerError();
    }

    [Fact]
    public async Task HttpResponseMessage_IsNotSuccessfull_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        await Stateless.Assert.That(response).IsNotSuccessfull();
    }

    [Fact]
    public async Task HttpResponseMessage_IsNotRedirectionFromOk_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).IsNotRedirection();
    }

    [Fact]
    public async Task HttpResponseMessage_IsNotClientError_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).IsNotClientError();
    }

    [Fact]
    public async Task HttpResponseMessage_IsNotServerError_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).IsNotServerError();
    }

    [Fact]
    public async Task HttpResponseMessage_DoesNotHaveHeader_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).DoesNotHaveHeader("X-Missing");
    }

    [Fact]
    public async Task HttpResponseMessage_DoesNotHaveContentType_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(response).DoesNotHaveContentType("application/json");
    }

    [Fact]
    public async Task HttpResponseMessage_HasVersion_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Version = new Version(1, 1) };
        await Stateless.Assert.That(response).HasVersion(new Version(1, 1));
    }

    [Fact]
    public async Task HttpContent_Is_WithContent_Passes()
    {
        using var expected = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        using var actual = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(actual).IsEquivalentTo(expected);
    }

    [Fact]
    public async Task HttpContent_IsNot_WithDifferentContent_Passes()
    {
        using var unexpected = new StringContent("world", System.Text.Encoding.UTF8, "text/plain");
        using var actual = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(actual).IsNotEquivalentTo(unexpected);
    }

    [Fact]
    public async Task HttpContent_Is_Passes()
    {
        using var content = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).Is("hello");
    }

    [Fact]
    public async Task HttpContent_IsNot_Passes()
    {
        using var content = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).IsNot("world");
    }

    [Fact]
    public async Task HttpResponseMessage_DoesNotHaveStatusCode_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).DoesNotHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task HttpResponseMessage_IsRedirection_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.Redirect);
        await Stateless.Assert.That(response).IsRedirection();
    }

    [Fact]
    public async Task HttpResponseMessage_IsNotRedirectionFromOk_Then_Passes()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        await Stateless.Assert.That(response).IsNotRedirection();
    }

    [Fact]
    public async Task HttpRequestMessage_Is_WithRequest_Passes()
    {
        using var expected = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        using var actual = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(actual).IsEquivalentTo(expected);
    }

    [Fact]
    public async Task HttpRequestMessage_IsNot_WithDifferentRequest_Passes()
    {
        using var unexpected = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        using var actual = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("different", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(actual).IsNotEquivalentTo(unexpected);
    }

    [Fact]
    public async Task HttpRequestMessage_Is_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(request).Is("payload");
    }

    [Fact]
    public async Task HttpRequestMessage_IsNot_Passes()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(request).IsNot("other");
    }

    [Fact]
    public async Task Uri_IsRelative_Passes()
    {
        var uri = new Uri("/path", UriKind.Relative);
        await Stateless.Assert.That(uri).IsRelative();
    }

    [Fact]
    public async Task Uri_IsAbsolute_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).IsAbsolute();
    }

    [Fact]
    public async Task Uri_HasQueryParameter_Passes()
    {
        var uri = new Uri("https://example.com/path?key=value");
        await Stateless.Assert.That(uri).HasQueryParameter("key");
    }

    [Fact]
    public async Task Uri_DoesNotHaveScheme_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).DoesNotHaveScheme("http");
    }

    [Fact]
    public async Task Uri_DoesNotHaveHost_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).DoesNotHaveHost("other.com");
    }

    [Fact]
    public async Task Uri_DoesNotHavePort_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).DoesNotHavePort(8080);
    }

    [Fact]
    public async Task Uri_DoesNotHavePath_Passes()
    {
        var uri = new Uri("https://example.com/path");
        await Stateless.Assert.That(uri).DoesNotHavePath("/other");
    }

    [Fact]
    public async Task Uri_DoesNotHaveQueryParameter_Passes()
    {
        var uri = new Uri("https://example.com/path?key=value");
        await Stateless.Assert.That(uri).DoesNotHaveQueryParameter("missing");
    }

    [Fact]
    public async Task Uri_DoesNotHaveFragment_Passes()
    {
        var uri = new Uri("https://example.com/path#section");
        await Stateless.Assert.That(uri).DoesNotHaveFragment("#other");
    }

    [Fact]
    public async Task Uri_IsRelative_WithAbsoluteUri_Throws()
    {
        var uri = new Uri("https://example.com/path");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).IsRelative());
        Assert.Contains("relative", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task Uri_IsAbsolute_WithRelativeUri_Throws()
    {
        var uri = new Uri("/path", UriKind.Relative);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).IsAbsolute());
        Assert.Contains("absolute", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task Uri_DoesNotHaveScheme_WithMatchingScheme_Throws()
    {
        var uri = new Uri("https://example.com/path");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).DoesNotHaveScheme("https"));
        Assert.Contains("https", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task Uri_DoesNotHaveQueryParameter_WithPresentParameter_Throws()
    {
        var uri = new Uri("https://example.com/path?key=value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).DoesNotHaveQueryParameter("key"));
        Assert.Contains("key", msg);
    }

    [Fact]
    public async Task Uri_HasFragment_Throws_when_fragment_differs()
    {
        var uri = new Uri("https://example.com/path#section");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).HasFragment("#other"));
        Assert.Contains("other", msg);
    }

    [Fact]
    public async Task HttpContent_HasMediaType_Throws_when_wrong_type()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).HasMediaType("application/json"));
        Assert.Contains("application/json", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_HasMediaType_Throws_when_missing_content_type()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).HasMediaType("application/json"));
        Assert.Contains("application/json", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_HasStatusCode_Throws_when_status_differs()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).HasStatusCode(HttpStatusCode.NotFound));
        Assert.Contains("NotFound", msg);
    }

    [Fact]
    public async Task Uri_HasQueryParameterValue_Throws_when_value_differs()
    {
        var uri = new Uri("https://example.com/path?key=value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).HasQueryParameterValue("key", "other"));
        Assert.Contains("other", msg);
    }

    [Fact]
    public async Task HttpContent_HasString_Throws_when_string_missing()
    {
        using var content = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).HasString("missing"));
        Assert.Contains("missing", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_HasHeader_Throws_when_header_missing()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).HasHeader("X-Missing"));
        Assert.Contains("X-Missing", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_DoesNotHaveStatusCode_Throws_when_status_present()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).DoesNotHaveStatusCode(HttpStatusCode.OK));
        Assert.Contains("OK", msg);
    }

    [Fact]
    public async Task Uri_HasQueryParameter_Throws_when_query_parameter_missing()
    {
        var uri = new Uri("https://example.com/path?key=value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).HasQueryParameter("missing"));
        Assert.Contains("missing", msg);
    }

    [Fact]
    public async Task HttpContent_HasHeaderValue_Throws_when_header_value_differs()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        content.Headers.Add("X-Content-Header", "value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).HasHeaderValue("X-Content-Header", "other"));
        Assert.Contains("other", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_HasHeaderValue_Throws_when_header_value_differs()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        request.Headers.Add("X-Request-Header", "value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).HasHeaderValue("X-Request-Header", "other"));
        Assert.Contains("other", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_HasHeaderValue_Throws_when_header_value_differs()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Response-Header", "value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).HasHeaderValue("X-Response-Header", "other"));
        Assert.Contains("other", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_DoesNotHaveHeader_Throws_when_header_present()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        response.Headers.Add("X-Response-Header", "value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).DoesNotHaveHeader("X-Response-Header"));
        Assert.Contains("X-Response-Header", msg);
    }

    [Fact]
    public async Task HttpContent_DoesNotHaveMediaType_Passes_when_content_type_differs()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        await Stateless.Assert.That(content).DoesNotHaveMediaType("application/json");
    }

    [Fact]
    public async Task HttpRequestMessage_DoesNotHaveMediaType_Passes_when_content_type_differs()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain")
        };
        await Stateless.Assert.That(request).DoesNotHaveMediaType("application/json");
    }

    [Fact]
    public async Task HttpResponseMessage_HasVersion_Throws_when_version_differs()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK) { Version = new Version(1, 1) };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).HasVersion(new Version(2, 0) ));
        Assert.Contains("2.0", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_IsSuccessfull_Throws_when_not_success()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).IsSuccessfull());
        Assert.Contains("success", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task HttpContent_HasLength_Throws_when_length_differs()
    {
        using var content = new StringContent("abc", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).HasLength(10));
        Assert.Contains("10", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_HasLength_Throws_when_length_differs()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("abc", System.Text.Encoding.UTF8, "text/plain")
        };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).HasLength(10));
        Assert.Contains("10", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_DoesNotHaveContentType_Throws_when_content_type_present()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain")
        };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).DoesNotHaveContentType("text/plain"));
        Assert.Contains("text/plain", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_Content_IsNotNull_Throws_when_missing()
    {
        // HttpResponseMessage.Content setter replaces null with EmptyContent in .NET 8+;
        // test IsNotNull on a null HttpContent directly instead
        HttpContent? nullContent = null;
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(nullContent).IsNotNull());
        Assert.Contains("Expected a value, but was null", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_RequestMessage_IsNotNull_Throws_when_missing()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).RequestMessage().IsNotNull());
        Assert.Contains("null", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task HttpResponseMessage_Headers_Contains_Throws_when_header_missing()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).Headers().Contains("X-Missing"));
        Assert.Contains("X-Missing", msg);
    }

    [Fact]
    public async Task Uri_Query_IsEmpty_Throws_when_query_present()
    {
        var uri = new Uri("https://example.com/path?key=value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).Query().IsEmpty());
        Assert.Contains("empty", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task HttpContent_HasHeader_Throws_when_header_missing()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).HasHeader("X-Missing"));
        Assert.Contains("X-Missing", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_DoesNotHaveHeader_Throws_when_header_present()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        request.Headers.Add("X-Request-Header", "value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).DoesNotHaveHeader("X-Request-Header"));
        Assert.Contains("X-Request-Header", msg);
    }

    [Fact]
    public async Task Uri_HasScheme_Throws_when_scheme_differs()
    {
        var uri = new Uri("https://example.com/path");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).HasScheme("http"));
        Assert.Contains("http", msg);
    }

    [Fact]
    public async Task Uri_HasHost_Throws_when_host_differs()
    {
        var uri = new Uri("https://example.com/path");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).HasHost("other.com"));
        Assert.Contains("other.com", msg);
    }

    [Fact]
    public async Task Uri_HasPort_Throws_when_port_differs()
    {
        var uri = new Uri("https://example.com:8080/path");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).HasPort(1234));
        Assert.Contains("1234", msg);
    }

    [Fact]
    public async Task HttpContent_Is_WithDifferentContent_Throws_when_strings_differ()
    {
        using var content = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).Is("world"));
        Assert.Contains("world", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_Is_WithDifferentRequest_Throws_when_payload_differs()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).Is("other"));
        Assert.Contains("other", msg);
    }

    [Fact]
    public async Task HttpStatusCode_IsSuccess_Throws_when_not_success()
    {
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(HttpStatusCode.NotFound).IsSuccess());
        Assert.Contains("success", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task HttpResponseMessage_IsNotSuccessfull_Throws_when_successful()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).IsNotSuccessfull());
        Assert.Contains("success", msg.ToLowerInvariant());
    }
    [Fact]
    public async Task HttpContent_DoesNotHaveHeader_Throws_when_header_present()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        content.Headers.Add("X-Content-Header", "value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).DoesNotHaveHeader("X-Content-Header"));
        Assert.Contains("X-Content-Header", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_DoesNotHaveMediaType_Throws_when_content_type_present()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain")
        };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).DoesNotHaveMediaType("text/plain"));
        Assert.Contains("text/plain", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_IsClientError_Throws_when_not_client_error()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).IsClientError());
        Assert.Contains("client", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task Uri_DoesNotHaveQueryParameter_Throws_when_parameter_present()
    {
        var uri = new Uri("https://example.com/path?key=value");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).DoesNotHaveQueryParameter("key"));
        Assert.Contains("key", msg);
    }

    [Fact]
    public async Task HttpContent_HasVersion_Throws_when_not_applicable()
    {
        using var content = new StringContent("body", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(new HttpResponseMessage(HttpStatusCode.OK) { Content = content }).HasVersion(new Version(2, 0)));
        Assert.Contains("2.0", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_HasHeaderValue_Throws_when_header_missing()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).HasHeaderValue("X-Request-Header", "value"));
        Assert.Contains("X-Request-Header", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_ReasonPhrase_Throws_when_missing_reason()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK); // ReasonPhrase defaults to "OK"
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).HasReasonPhrase("NotOK"));
        Assert.Contains("NotOK", msg);
    }

    [Fact]
    public async Task HttpContent_HasLength_Throws_when_length_differs_1()
    {
        using var content = new StringContent("abc", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).HasLength(10));
        Assert.Contains("10", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_HasHeader_Throws_when_header_missing()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).HasHeader("X-Missing"));
        Assert.Contains("X-Missing", msg);
    }

    [Fact]
    public async Task Uri_IsRelative_Throws_when_uri_is_absolute()
    {
        var uri = new Uri("https://example.com/path");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).IsRelative());
        Assert.Contains("relative", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task HttpContent_IsNot_Throws_when_content_matches()
    {
        using var content = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        using var other = new StringContent("hello", System.Text.Encoding.UTF8, "text/plain");
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(content).IsNotEquivalentTo(other));
        Assert.Contains("HttpContent not to be", msg);
    }

    [Fact]
    public async Task HttpRequestMessage_IsNot_Throws_when_request_matches()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        using var other = new HttpRequestMessage(HttpMethod.Post, "https://example.com")
        {
            Content = new StringContent("payload", System.Text.Encoding.UTF8, "text/plain")
        };
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(request).IsNotEquivalentTo(other));
        Assert.Contains("HttpRequestMessage not to be", msg);
    }

    [Fact]
    public async Task HttpResponseMessage_IsNotRedirection_Throws_when_redirection()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.Redirect);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(response).IsNotRedirection());
        Assert.Contains("redirection", msg.ToLowerInvariant());
    }

    [Fact]
    public async Task Uri_IsAbsolute_Throws_when_uri_is_relative()
    {
        var uri = new Uri("/path", UriKind.Relative);
        var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(uri).IsAbsolute());
        Assert.Contains("absolute", msg.ToLowerInvariant());
    }
}




