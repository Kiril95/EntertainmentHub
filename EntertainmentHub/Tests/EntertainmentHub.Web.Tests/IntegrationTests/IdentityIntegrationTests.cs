namespace EntertainmentHub.Web.Tests.IntegrationTests
{
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class IdentityIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> server;

        public IdentityIntegrationTests(WebApplicationFactory<Program> server)
        {
            this.server = server;
        }

        [Fact]
        public async Task AccountManagePageRequiresAuthorization()
        {
            var client = this.server.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            var response = await client.GetAsync("Identity/Account/Manage");

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}
