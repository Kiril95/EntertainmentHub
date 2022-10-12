namespace EntertainmentHub.Web.Tests.IntegrationTests
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class HomepageIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> server;

        public HomepageIntegrationTests(WebApplicationFactory<Program> server)
        {
            this.server = server;
        }

        [Fact]
        public async Task IndexPageShouldReturnStatusCode200WithTitle()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>", responseContent);
        }
    }
}
