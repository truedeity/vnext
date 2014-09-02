using Microsoft.AspNet.Mvc;
//using BrickPile;
using System;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BrickPile.UI.Controllers
{    
    public class TempController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index(Home currentModel, Page currentPage)
        {

            //using (var session = DefaultBrickPileBootstrapper.DocumentStore.OpenSession())
            //{
            //    var trie = session.Load<Trie>("brickpile/trie");

            //    TrieNode parent;
            //    trie.TryGetValue("/a-page", out parent);

            //    trie.Add("/a-page/another-page", new TrieNode { Name = "Another page" });

            //    var ancestors = trie.AncestorsOf("/a-page/another-page");
            //    var children = trie.ChildrenOf("/");

            //    var currentTrie = trie.BuildTrie("/a-page/another-page");

            //}

            return Content("This is the Temp/Index action... =)"
                + Environment.NewLine + "Heading: " + currentModel.Heading
                + Environment.NewLine + "Id: " + currentPage.Id);
        }

        public IActionResult Test()
        {
            //using (var session = DefaultBrickPileBootstrapper.DocumentStore.OpenSession())
            //{
            //    //var trie = session.Load<Trie>("brickpile/trie");

            //    //TrieNode parent;
            //    //trie.TryGetValue("/", out parent);

            //    //trie.Add("/a-page/another-page", new TrieNode { Name = "Another page" });

            //    //var ancestors = trie.AncestorsOf("/a-page/another-page");
            //    //var children = trie.ChildrenOf("/");

            //    //var currentTrie = trie.BuildTrie("/a-page/another-page");

            //    //var page = new Page();
            //    //session.Store(page);
            //    //session.SaveChanges();

            //}

            return Content("This is the Temp/Test action... ");
        }

    }
}
