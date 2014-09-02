using System.ComponentModel.DataAnnotations;

namespace BrickPile.UI
{
    /// <summary>
    /// Summary description for Home
    /// </summary>
    [Model]
    public class Home
    {
        [Required(ErrorMessage = "Yeah, and fuck you tooooo...")]
        public string Heading { get; set; }

        [Display(Name = "Hero image ..."), Required]
        public Image Hero { get;set; }

        public Home()
	    {
		    //
		    // TODO: Add constructor logic here
		    //
	    }

        public class Image
        {
            [Display(Name ="Alt-text")]
            public string AlternativText { get; set; }
        }
    }
}