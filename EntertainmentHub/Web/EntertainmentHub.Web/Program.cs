namespace EntertainmentHub.Web
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;

    using EntertainmentHub.Data;
    using EntertainmentHub.Data.Common;
    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using EntertainmentHub.Data.Repositories;
    using EntertainmentHub.Data.Seeding;
    using EntertainmentHub.Services.Data;
    using EntertainmentHub.Services.Data.Contracts;
    using EntertainmentHub.Services.Data.DataAPI;
    using EntertainmentHub.Services.Mapping;
    using EntertainmentHub.Services.Messaging;
    using EntertainmentHub.Web.ViewModels;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            // .AddEntityFrameworkStores<ApplicationDbContext>();
            ConfigureServices(builder.Services, builder.Configuration);

            builder.Services.Configure<TMDBKeyModel>(builder.Configuration.GetSection("TMDB:ApiKey"));

            var app = builder.Build();

            Configure(app);
            app.UseAuthentication();
            app.UseStatusCodePages(ctx =>
            {
                if (ctx.HttpContext.Response.StatusCode == 405)
                {
                    ctx.HttpContext.Response.StatusCode = 404;
                }

                return Task.CompletedTask;
            });

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                options.SchemaName = "dbo";
                options.TableName = "CacheRecords";
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            })
            .AddRazorRuntimeCompilation();

            services.AddRazorPages();
            services.AddControllers();
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-CSRF-TOKEN";
            });

            services.AddSingleton(configuration);
            services.AddHttpClient();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Application services
            services.AddTransient<IEmailSender>(x => new SendGridEmailSender(configuration["SendGrid:ApiKey"]));
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<ICollectService, CollectService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IMoviesService, MoviesService>();
            services.AddTransient<IGenresService, GenresService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<ICommentsService, CommentsService>();
            services.AddTransient<IReviewsService, ReviewsService>();
            services.AddTransient<IActorsService, ActorsService>();
            services.AddTransient<IRatingsService, RatingsService>();
            services.AddTransient<ISearchService, SearchService>();
        }

        private static void Configure(WebApplication app)
        {
            // Seed data on application startup
            using (var serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider).GetAwaiter().GetResult();
            }

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithRedirects("/Home/ErrorView/{0}"); // Attaches the status code after the error

            app.UseHttpsRedirection();
            app.UseCookiePolicy();

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=604800"); // 7days
                    ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(7).ToString("R", CultureInfo.InvariantCulture));
                },
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();
        }
    }
}
