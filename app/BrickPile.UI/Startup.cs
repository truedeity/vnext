using Microsoft.AspNet.Builder;
using BrickPile.Extensions;
using System;
using Raven.Client;
using Microsoft.AspNet.Identity;

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
