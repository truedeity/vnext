using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace BrickPile
{
    /// <summary>
    /// Summary description for TestModelBinder
    /// </summary>
    public class TestModelBinder : IModelBinder
    {
	    public TestModelBinder()
	    {
		    //
		    // TODO: Add constructor logic here
		    //
	    }

        public Task<bool> BindModelAsync(ModelBindingContext bindingContext)
        {
            throw new NotImplementedException();
        }
    }
}