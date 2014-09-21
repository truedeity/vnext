using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrickPile
{
    /// <summary>
    /// Summary description for Image
    /// </summary>
    public class Image
    {
        [Display(Name = "Url (/images/hubot.png)")]
        public string Id { get; set; }

        public string Name { get; set; }

        public string AltText { get; set; }

        public string AccessibleText { get; set; }

        [ScaffoldColumn(false)]
        public ICollection<Image> Formats { get;set; }

        public Image()
	    {
            this.Formats = new List<Image>();
	    }
    }
}