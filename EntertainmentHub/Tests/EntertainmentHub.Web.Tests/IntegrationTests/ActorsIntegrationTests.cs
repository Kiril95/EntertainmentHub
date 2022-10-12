namespace EntertainmentHub.Web.Tests.IntegrationTests
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Testing;
    using Xunit;

    public class ActorsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> server;

        public ActorsIntegrationTests(WebApplicationFactory<Program> server)
        {
            this.server = server;
        }

        [Fact]
        public async Task AllActorsShouldLoadCorrectly()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Actors/All");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Actors List", responseContent);
            Assert.Contains("<td>1</td>", responseContent);
        }

        [Fact]
        public async Task AllActorsReturns404IfTheRouteIsWrong()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Actors/Alll");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>Page not found", responseContent);
        }

        [Fact]
        public async Task ActorDetailsShouldLoadCorrectly()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Actors/Details/283");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Aaron Taylor-Johnson", responseContent);
            Assert.Contains("<a>Details</a>", responseContent);
            Assert.Contains("Notable <span>Movies</span>", responseContent);
        }

        [Fact]
        public async Task ActorDetailsReturns404IfTheIdDoesNotExist()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Actors/Details/100000");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>Page not found", responseContent);
        }

        [Fact]
        public async Task AllActorsSearchShouldFindAllMatchesCorrectly()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Actors/All?SearchWord=elizabeth");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("value=\"elizabeth\"", responseContent);
            Assert.Contains("<span>Elizabeth Olsen</span>", responseContent);
        }

        [Fact]
        public async Task AllActorsPaginationShouldWorkCorrectly()
        {
            var client = this.server.CreateClient();
            var response = await client.GetAsync("/Actors/All?page=2");
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Count: <span>50", responseContent);
            Assert.Contains("<td>26</td>", responseContent);
            Assert.Contains("<span class=\"page-link\">2</span>", responseContent);
        }
    }
}
