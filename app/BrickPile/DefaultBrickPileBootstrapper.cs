using System;
using Raven.Client;
using Raven.Client.Document;

namespace BrickPile
{
    /// <summary>
    /// Summary description for DefaultBrickPileBootstrapper
    /// </summary>
    public class DefaultBrickPileBootstrapper : IBrickPileBootstrapper
    {
        private static readonly Lazy<IDocumentStore> DocStore = new Lazy<IDocumentStore>(() =>
        {
            var store = new DocumentStore
            {
                Url = "http://localhost:8080",
                DefaultDatabase = "BrickPile"
            };
            store.Initialize();
            return store;
        });

        public static IDocumentStore DocumentStore
        {
            get { return DocStore.Value; }
        }

        public DefaultBrickPileBootstrapper()
	    {
            
        }

        public void Initialise()
        {
            using (IDocumentSession session = DocumentStore.OpenSession())
            {
                var trie = session.Load<Trie>("brickpile/trie");

                if (trie != null) return;

                trie = new Trie();

                var node = new TrieNode { Name = "Home", PageId = "homes/1" };

                trie.Add("/", node);

                node = new TrieNode { Name = "A Page" };

                trie.Add("/a-page", node);

                session.Store(trie);
                session.SaveChanges();
            }
        }
    }
}