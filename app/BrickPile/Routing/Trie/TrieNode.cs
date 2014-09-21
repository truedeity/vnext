using Raven.Imports.Newtonsoft.Json;

namespace BrickPile.Routing.Trie
{
    /// <summary>
    /// Summary description for TrieNode
    /// </summary>
    public class TrieNode
    {
        public string Name { get; set; }

        public string PageId { get; set; }

        [JsonIgnore]
        public string ContentId
        {
            get
            {
                return this.PageId + "/content";
            }
        }

        public TrieNode()
	    {
		    //
		    // TODO: Add constructor logic here
		    //
	    }
    }
}