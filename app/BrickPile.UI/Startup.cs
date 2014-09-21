using Microsoft.AspNet.Builder;
using BrickPile.Extensions;

namespace BrickPile.UI
{
    public class Startup
    {
        public void Configure(IBuilder app)
        {
            app.UseBrickPile();
        }
    }
}
