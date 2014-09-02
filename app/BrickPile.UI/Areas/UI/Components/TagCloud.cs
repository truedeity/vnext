using Microsoft.AspNet.Mvc;
using Raven.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BrickPile.UI
{
    /// <summary>
    /// Summary description for TagCloud
    /// </summary>
    [ViewComponent(Name = "Tags"), Area("UI")]
    public class TagCloud : ViewComponent
    {
        private readonly IDocumentStore _documentStore;

        public TagCloud(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var tags = await GetShit();
            return View(tags);
        }

        private async Task<string[]> GetShit()
        {
            using(var session = _documentStore.OpenAsyncSession())
            {
                var shit = await session.Query<Page>().ToListAsync();
                return await Task.FromResult(shit.Select(x => x.Id).ToArray());
            }
            //var shit = new string[] { "Marcus", "Anders", "Erik", "David" };
            
        }
    }
}