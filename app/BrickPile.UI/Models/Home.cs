using System;
using System.ComponentModel.DataAnnotations;

namespace BrickPile.UI
{
    /// <summary>
    /// Summary description for Home
    /// </summary>
    [Model]
    public class Home
    {
        [Required(ErrorMessage = "Yeah")]
        public string Heading { get; set; }

        //[Display(Name = "Hero image ...")]
        [Format(Formats = new[] { MediaFormat.Desktop, MediaFormat.Mobile })]
        public Image Hero { get;set; }

        public Home()
	    {
		    //
		    // TODO: Add constructor logic here
		    //
	    }
    }
    public class Format : Attribute
    {
        public MediaFormat[] Formats { get;set; }
    }

    public enum MediaFormat
    {
        Mobile = 0,
        Desktop = 1,
        Tablet = 2,
        TabletL = 3,
        MobileL = 4
    }
}