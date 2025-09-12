using CircleApp.Data.Models;
using Microsoft.AspNetCore.Http;


namespace CircleApp.Data.Services
{
    public interface IStoriesService
    {
        Task<List<Story>> GetAllStoriesAsync();
        Task<Story> CreateStoryAsync(Story story);
    }
}
