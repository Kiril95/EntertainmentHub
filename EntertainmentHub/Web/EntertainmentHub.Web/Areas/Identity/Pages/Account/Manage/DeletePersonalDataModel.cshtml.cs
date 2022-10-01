namespace EntertainmentHub.Web.Areas.Identity.Pages.Account.Manage
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using EntertainmentHub.Data.Common.Repositories;
    using EntertainmentHub.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;

    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<DeletePersonalDataModel> logger;

        private readonly IRepository<Rating> ratingsRepository;
        private readonly IDeletableEntityRepository<MovieComment> movieCommentsRepository;

        public DeletePersonalDataModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            IDeletableEntityRepository<MovieComment> movieCommentsRepository,
            IRepository<Rating> ratingsRepository)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.movieCommentsRepository = movieCommentsRepository;
            this.ratingsRepository = ratingsRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            this.RequirePassword = await this.userManager.HasPasswordAsync(user);
            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            // Remove all references b4 deleting the user
            var roles = await this.userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            await this.userManager.RemoveFromRoleAsync(user, role);

            var ratings = this.ratingsRepository.All().Where(x => x.UserId == user.Id);
            if (ratings is not null)
            {
                foreach (var item in ratings)
                {
                    this.ratingsRepository.Delete(item);
                }

                await this.ratingsRepository.SaveChangesAsync();
            }

            var comments = this.movieCommentsRepository.All().Where(x => x.UserId == user.Id);
            if (comments is not null)
            {
                foreach (var item in comments)
                {
                    this.movieCommentsRepository.HardDelete(item);
                }

                await this.movieCommentsRepository.SaveChangesAsync();
            }

            this.RequirePassword = await this.userManager.HasPasswordAsync(user);
            if (this.RequirePassword)
            {
                if (!await this.userManager.CheckPasswordAsync(user, this.Input.Password))
                {
                    this.ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return this.Page();
                }
            }

            var result = await this.userManager.DeleteAsync(user);
            var userId = await this.userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            await this.signInManager.SignOutAsync();

            this.logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return this.Redirect("~/");
        }
    }
}
