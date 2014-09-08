using Microsoft.AspNet.Mvc;
using Raven.Client.FileSystem;
using System.Threading.Tasks;

namespace BrickPile.UI
{
    /// <summary>
    /// Summary description for AssetFilter
    /// </summary>
    [ViewComponent(Name = "Directories")]
    public class Directories : ViewComponent
    {
        private readonly IFilesStore _fileStore;

        public Directories(IFilesStore fileStore)
        {
            _fileStore = fileStore;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var directories = await GetDirectories();
            return View(directories);
        }

        private async Task<string[]> GetDirectories()
        {
            using (var session = _fileStore.OpenAsyncSession())
            {
                return await session.Commands.GetDirectoriesAsync();
            }
        }
    }
}