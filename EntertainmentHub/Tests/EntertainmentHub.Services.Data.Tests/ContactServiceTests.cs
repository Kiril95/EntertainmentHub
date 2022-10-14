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
    using EntertainmentHub.Services.Messaging;
    using EntertainmentHub.Web.ViewModels.Contact;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class ContactServiceTests : IDisposable
    {
        private readonly IContactService contactService;
        private EfRepository<ContactForm> contactRepository;
        private SqliteConnection connection;
        private ContactForm contact;

        public ContactServiceTests()
        {
            this.SetupMapper();
            this.SetupDatabase();

            this.contactService = new ContactService(this.contactRepository, null);
        }

        [Fact]
        public async Task UserQueryShouldBeAddedCorrectly()
        {
            var contact = new ContactFormInputModel
            {
                Name = "Miko",
                Email = "test@abv.bg",
                Subject = "Testing",
                Message = "IHeardYourProjectIsCool",
            };

            await this.contactService.GetUserSubmissionAsync(contact);

            var submissions = this.contactService.GetSubmissionsAsQueryable<ContactViewModel>();

            Assert.Equal(1, await submissions.CountAsync());
        }

        [Fact]
        public async Task GetAllSubmissionsShouldWorkCorrectly()
        {
            await this.SeedData();
            var submissions = this.contactService.GetSubmissionsAsQueryable<ContactViewModel>();

            Assert.Equal(1, await submissions.CountAsync());
        }

        [Fact]
        public void VerifyCollectionExistsEvenIfEmpty()
        {
            var submissions = this.contactService.GetSubmissionsAsQueryable<ContactViewModel>();

            Assert.Empty(submissions);
        }

        [Fact]
        public async Task GetAllSubmissionsShouldWorkCorrectlyWithAny()
        {
            await this.SeedData();
            var submissions = this.contactService.GetSubmissionsAsQueryable<ContactViewModel>();

            Assert.True(await submissions.AnyAsync());
        }

        [Fact]
        public async Task GetSubmissionByIdShouldWorkCorrectly()
        {
            await this.SeedData();
            var submission = await this.contactService.GetSubmissionByIdAsync<ContactViewModel>(1);

            Assert.Equal("Kiko", submission.Name);
        }

        [Fact]
        public async Task GetSubmissionByIdShouldWorkCorrectlyWhenWeAddMultipleQueries()
        {
            await this.SeedData();
            await this.contactRepository.AddAsync(new ContactForm
            {
                Name = "Test",
                Email = "test@abv.bg",
                Subject = "Bla",
                Message = "BlaBlaBla",
            });
            await this.contactRepository.SaveChangesAsync();

            var submission = await this.contactService.GetSubmissionByIdAsync<ContactViewModel>(2);

            Assert.Equal("Bla", submission.Subject);
        }

        [Fact]
        public async Task GetSubmissionByIdThrowsExceptionIfQueryIsNotFound()
        {
            await this.SeedData();
            var submission = await this.contactService.GetSubmissionByIdAsync<ContactViewModel>(2);

            Assert.Throws<NullReferenceException>(() => submission.Name);
        }

        [Fact]
        public async Task GetSubmissionByIdReturnsNullIfActorIsNotFound()
        {
            await this.SeedData();
            var submission = await this.contactService.GetSubmissionByIdAsync<ContactViewModel>(2);

            Assert.Null(submission);
        }

        [Fact]
        public async Task DeleteSubmissionShouldWorkCorrectly()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(this.connection)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            var dbContext = new ApplicationDbContext(options.Options);
            dbContext.Database.EnsureCreated();

            using var contactRepo = new EfRepository<ContactForm>(dbContext);
            var service = new ContactService(contactRepo, null);

            var con = new ContactForm
            {
                Name = "Kes",
                Email = "tes@abv.bg",
                Subject = "Tes",
                Message = "Pes?",
            };

            await dbContext.ContactForms.AddAsync(con);
            await dbContext.SaveChangesAsync();

            var sub = await dbContext.ContactForms.FindAsync(con.Id);
            dbContext.Entry(sub).State = EntityState.Detached;

            await service.DeleteSubmissionAsync(sub.Id);

            Assert.Empty(service.GetSubmissionsAsQueryable<ContactViewModel>());
        }

        [Fact]
        public async Task DeleteSubmissionThrowsExceptionIfQueryIsNotFound()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => this.contactService.DeleteSubmissionAsync(1));
        }

        [Fact]
        public async Task ReplyToUserShouldWorkCorrectly()
        {
            var emailMock = new Mock<IEmailSender>();
            var service = new ContactService(this.contactRepository, emailMock.Object);

            var answer = new ReplyModel
            {
                Name = "Miko",
                Email = "kiko@abv.bg",
                To = "piko@abv.bg",
                Subject = "Testing",
                Message = "IsItFun?",
            };

            await service.ReplyToUserAsync(answer);
        }

        [Fact]
        public async Task ReplyToUserThrowsExceptionIfEmailSenderIsMissing()
        {
            var answer = new ReplyModel
            {
                Name = "Miko",
                Email = "kiko@abv.bg",
                To = "piko@abv.bg",
                Subject = "Testing",
                Message = "IsItFun?",
            };

            await Assert.ThrowsAsync<NullReferenceException>(async () => await this.contactService.ReplyToUserAsync(answer));
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
                this.contactRepository?.Dispose();
            }
        }

        private void SetupDatabase()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(this.connection)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            var dbContext = new ApplicationDbContext(options.Options);

            dbContext.Database.EnsureCreated();
            this.contactRepository = new EfRepository<ContactForm>(dbContext);
        }

        private async Task SeedData()
        {
            this.contact = new ContactForm
            {
                Name = "Kiko",
                Email = "test@abv.bg",
                Subject = "Testing",
                Message = "IsItFun?",
            };

            await this.contactRepository.AddAsync(this.contact);
            await this.contactRepository.SaveChangesAsync();
        }

        private void SetupMapper()
        {
            AutoMapperConfig.RegisterMappings(Assembly.Load("EntertainmentHub.Web.ViewModels"));
        }
    }
}
