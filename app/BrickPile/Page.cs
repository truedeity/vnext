using System;
using System.ComponentModel.DataAnnotations;

namespace BrickPile
{
    /// <summary>
    /// Summary description for Home
    /// </summary>
    public sealed class Page
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        [DataType(DataType.Html), Required]
        public string Body { get; set; }

        public int MyProperty { get; set; }

        public Page()
	    {
		    //
		    // TODO: Add constructor logic here
		    //
	    }
    }
}