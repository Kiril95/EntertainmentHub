namespace EntertainmentHub.Web.Tests.IntegrationTests
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Net.Http.Headers;
    using Xunit;

    public class ContactIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<AntiForgeryTokenExtractor>
    {
        private readonly WebApplicationFactory<Program> server;
        private readonly AntiForgeryTokenExtractor tokenExtractor;

        public ContactIntegrationTests(
            WebApplicationFactory<Program> server,
            AntiForgeryTokenExtractor tokenExtractor)
        {
            this.server = server;
            this.tokenExtractor = tokenExtractor;
        }

        [Fact]
        public async Task ContactPageShouldReturnStatusCode200WithTitle()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Contact");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>Contact", responseContent);
        }

        [Fact]
        public async Task ContactPageReturns404IfTheRouteIsWrong()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Contacts");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>Page not found", responseContent);
        }

        [Fact]
        public async Task CreateSubmissionSendsCorrectData()
        {
            var client = this.server.CreateClient();
            var initialRes = await client.GetAsync("/Contact");
            (string Cookie, string Field) antiForgeryVal = await this.tokenExtractor.ExtractAntiForgeryValues(initialRes);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Contact");

            postRequest.Headers.Add("Cookie", new CookieHeaderValue(this.tokenExtractor.Cookie, antiForgeryVal.Field).ToString());

            var formModel = new Dictionary<string, string>
            {
                 { this.tokenExtractor.Field, antiForgeryVal.Cookie },
                 { "Name", "Test" },
                 { "Email", "test@gmail.com" },
                 { "Subject", "IntegrationTest" },
                 { "Message", "DoesItWork?" },
            };
            postRequest.Content = new FormUrlEncodedContent(formModel);

            var response = await client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("Thank you for contacting us!", responseString);
        }

        [Fact]
        public async Task CreateSubmissionSendsANotFullyFilledForm()
        {
            var client = this.server.CreateClient();
            var initialRes = await client.GetAsync("/Contact");
            (string Cookie, string Field) antiForgeryVal = await this.tokenExtractor.ExtractAntiForgeryValues(initialRes);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Contact");

            postRequest.Headers.Add("Cookie", new CookieHeaderValue(this.tokenExtractor.Cookie, antiForgeryVal.Field).ToString());

            var formModel = new Dictionary<string, string>
            {
                 { this.tokenExtractor.Field, antiForgeryVal.Cookie },
                 { "Name", "Test" },
                 { "Email", "test@gmail.com" },
                 { "Subject", string.Empty },
                 { "Message", string.Empty },
            };
            postRequest.Content = new FormUrlEncodedContent(formModel);

            var response = await client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("The Subject field is required.", responseString);
        }

        [Fact]
        public async Task CreateSubmissionSendsAFormWithMissingFields()
        {
            var client = this.server.CreateClient();
            var initialRes = await client.GetAsync("/Contact");
            (string Cookie, string Field) antiForgeryVal = await this.tokenExtractor.ExtractAntiForgeryValues(initialRes);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Contact");

            postRequest.Headers.Add("Cookie", new CookieHeaderValue(this.tokenExtractor.Cookie, antiForgeryVal.Field).ToString());

            var formModel = new Dictionary<string, string>
            {
                 { this.tokenExtractor.Field, antiForgeryVal.Cookie },
                 { "Name", "Test" },
                 { "Email", "test@gmail.com" },
            };
            postRequest.Content = new FormUrlEncodedContent(formModel);

            var response = await client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.DoesNotContain("Thank you for contacting us!", responseString);
        }

        [Fact]
        public async Task CreateSubmissionSendsInvalidNameField()
        {
            var client = this.server.CreateClient();
            var initialRes = await client.GetAsync("/Contact");
            (string Cookie, string Field) antiForgeryVal = await this.tokenExtractor.ExtractAntiForgeryValues(initialRes);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Contact");

            postRequest.Headers.Add("Cookie", new CookieHeaderValue(this.tokenExtractor.Cookie, antiForgeryVal.Field).ToString());

            var formModel = new Dictionary<string, string>
            {
                 { this.tokenExtractor.Field, antiForgeryVal.Cookie },
                 { "Name", "T" },
                 { "Email", "test@gmail.com" },
                 { "Subject", "IntegrationTest" },
                 { "Message", "DoesItWork?" },
            };
            postRequest.Content = new FormUrlEncodedContent(formModel);

            var response = await client.SendAsync(postRequest);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Contains("Name should be between 2 and 50 characters!", responseString);
        }
    }
}
