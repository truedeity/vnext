using BrickPile.Routing.Trie;
using System;

namespace BrickPile.Routing
{
    /// <summary>
    /// Summary description for ResolveResult
    /// </summary>
    public class ResolveResult
    {
        /// <summary>
        ///     Gets or sets the trie node.
        /// </summary>
        /// <value>
        ///     The trie node.
        /// </value>
        public TrieNode TrieNode { get; set; }

        /// <summary>
        ///     Gets or sets the action.
        /// </summary>
        /// <value>
        ///     The action.
        /// </value>
        public string Action { get; set; }

        /// <summary>
        ///     Gets or sets the controller.
        /// </summary>
        /// <value>
        ///     The controller.
        /// </value>
        public string Controller { get; set; }

        /// <summary>
        ///     Gets or sets the controller.
        /// </summary>
        /// <value>
        ///     The controller.
        /// </value>
        public Page CurrentPage { get;set; }

        /// <summary>
        ///     Gets or sets the controller.
        /// </summary>
        /// <value>
        ///     The controller.
        /// </value>
        public dynamic CurrentModel { get;set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResolveResult" /> class.
        /// </summary>
        /// <param name="trieNode">The trie node.</param>
        /// <param name="action">The action.</param>
        public ResolveResult(TrieNode trieNode, Page currentPage, dynamic currentModel, string controller, string action)
        {

            if (trieNode == null)
            {
                throw new ArgumentNullException("trieNode");
            }

            if (currentPage == null)
            {
                throw new ArgumentNullException("currentPage");
            }

            if (currentModel == null)
            {
                throw new ArgumentNullException("currentModel");
            }

            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.TrieNode = trieNode;
            this.CurrentPage = currentPage;
            this.CurrentModel = currentModel;
            this.Controller = controller;
            this.Action = action;
        }
    }
}