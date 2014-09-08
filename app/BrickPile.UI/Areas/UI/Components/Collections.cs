using Microsoft.AspNet.Mvc;
using Raven.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrickPile.UI
{
    /// <summary>
    /// Summary description for Sets
    /// </summary>
    [ViewComponent(Name = "Collections")]
    public class Collections : ViewComponent
    {
        private readonly IDocumentStore _documentStore;

        public Collections(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var collections = await GetCollections();
            return View(collections);
        }

        private async Task<IList<Collection>> GetCollections()
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                return await session.Query<Collection>().ToListAsync();
            }
        }
    }
}