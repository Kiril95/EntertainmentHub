namespace EntertainmentHub.Services.Data.Tests
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using EntertainmentHub.Data;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Data.Repositories;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Web.ViewModels.Actors;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ActorsServiceTests : IDisposable
    {
        private readonly IActorsService actorsService;
        private EfDeletableEntityRepository<Actor> actorsRepository;
        private SqliteConnection connection;
        private Actor testActor;

        public ActorsServiceTests()
        {
            this.SetupMapper();
            this.SetupDatabase();

            this.actorsService = new ActorsService(this.actorsRepository);
        }

        [Fact]
        public async Task GetActorByIdShouldWorkCorrectly()
        {
            await this.SeedData();
            var actor = await this.actorsService.GetActorByIdAsync<ActorSimpleViewModel>(1);

            Assert.Equal("Hugh Jackman", actor.Name);
        }

        [Fact]
        public async Task GetActorByIdShouldWorkCorrectlyWhenWeAddMultipleActors()
        {
            await this.SeedData();
            await this.actorsRepository.AddAsync(new Actor { Name = "Test", Biography = "..." });
            await this.actorsRepository.SaveChangesAsync();

            var actor = await this.actorsService.GetActorByIdAsync<ActorSimpleViewModel>(2);

            Assert.Equal("Test", actor.Name);
        }

        [Fact]
        public async Task GetActorByIdThrowsExceptionIfActorIsNotFound()
        {
            await this.SeedData();
            var actor = await this.actorsService.GetActorByIdAsync<ActorSimpleViewModel>(2);

            Assert.Throws<NullReferenceException>(() => actor.Name);
        }

        [Fact]
        public async Task GetActorByIdReturnsNullIfActorIsNotFound()
        {
            await this.SeedData();
            var actor = await this.actorsService.GetActorByIdAsync<ActorSimpleViewModel>(2);

            Assert.Null(actor);
        }

        [Fact]
        public void VerifyCollectionExistsEvenIfEmpty()
        {
            var actors = this.actorsService.GetAllActorsAsQueryable<ActorSimpleViewModel>();

            Assert.Empty(actors);
        }

        [Fact]
        public async Task GetАllActorsShouldWorkCorrectlyWithAny()
        {
            await this.SeedData();
            var actors = this.actorsService.GetAllActorsAsQueryable<ActorSimpleViewModel>();

            Assert.True(await actors.AnyAsync());
        }

        [Fact]
        public async Task GetАllActorсShouldWorkCorrectly()
        {
            await this.SeedData();
            var actors = this.actorsService.GetAllActorsAsQueryable<ActorSimpleViewModel>();

            Assert.Equal(1, await actors.CountAsync());
        }

        [Fact]
        public async Task GetAllActorsShouldWorkCorrectlyWhenWeAddMultipleActors()
        {
            await this.SeedData();
            await this.actorsRepository.AddAsync(new Actor { Name = "Test", Biography = "..." });
            await this.actorsRepository.SaveChangesAsync();

            var actors = this.actorsService.GetAllActorsAsQueryable<ActorSimpleViewModel>();

            Assert.Equal(2, await actors.CountAsync());
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.connection.Close();
                this.connection.Dispose();
                this.actorsRepository?.Dispose();
            }
        }

        private void SetupDatabase()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(this.connection);
            var dbContext = new ApplicationDbContext(options.Options);

            dbContext.Database.EnsureCreated();
            this.actorsRepository = new EfDeletableEntityRepository<Actor>(dbContext);
        }

        private async Task SeedData()
        {
            this.testActor = new Actor
            {
                Name = "Hugh Jackman",
                Biography = "test",
            };

            await this.actorsRepository.AddAsync(this.testActor);
            await this.actorsRepository.SaveChangesAsync();
        }

        private void SetupMapper()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("EntertainmentHub.Web.ViewModels"));
        }
    }
}
