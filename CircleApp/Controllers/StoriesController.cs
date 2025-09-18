using CircleApp.Controllers.Base;
using CircleApp.Data.Helpers.Enums;
using CircleApp.Data.Models;
using CircleApp.Data.Services;
using CircleApp.ViewModels.Stories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CircleApp.Controllers
{
    [Authorize]

    public class StoriesController : BaseController
    {
        private readonly IStoriesService _storiesService;
        private readonly IFilesService _filesService;
        public StoriesController(IStoriesService storiesService, IFilesService filesSerive)
        {
            _storiesService = storiesService;
            _filesService = filesSerive;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryVM storyVM)
        {
            var loggedInUser = GetUserId();

            if (loggedInUser == null)
            {
                return RedirectToLogin();
            }
            var imageUploadPath = await _filesService.UploadImageAsync(storyVM.Image, ImageFileType.StoryImage);

            var newStory = new Story()
            {
               IsDeleted = false,
               DateCreated = DateTime.UtcNow,
               UserId = loggedInUser.Value,
               ImageUrl = imageUploadPath,

            };
            await _storiesService.CreateStoryAsync(newStory);

            return RedirectToAction("Index", "Home");
        }
    }
}
