using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Raven.Client.Document;
using System;
using Xunit;
using FakeItEasy;

namespace BrickPile.Tests
{
    public class Class1
    {

        [Fact]
        public void Can_Resolve_Route()
        {

            //var router = A.Fake<IRouter>();
            //var context = A.Fake<RouteContext>();

            using (var store = new DocumentStore() { Url = "http://localhost:8080" } )
            {
                using (var session = store.OpenSession())
                {
                    //var trie = session.Load<Trie>("brickpile/trie");

                    //if (trie != null) return;

                    //trie = new Trie();

                    //var node = new TrieNode { Name = "Home", PageId = "homes/1" };

                    //trie.Add("/", node);

                    //node = new TrieNode { Name = "A Page" };

                    //trie.Add("/a-page", node);

                    //session.Store(trie);
                    //session.SaveChanges();

                    //var route = new DefaultRoute(router);

                    //var result = route.RouteAsync(context);

                    Assert.True(false);
                }
            }
        }
    }

}
