using System;
using BlazeQuartz.Core.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlazeQuartz
{
    public static class BlazingQuartzBuilderExtensions
    {
        public static IApplicationBuilder UseBlazingQuartzUI(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<BlazingQuartzUIOptions>>().Value;
                if (options.AutoMigrateDb)
                {
                    var db = scope.ServiceProvider.GetRequiredService<BlazingQuartzDbContext>();
                    db.Database.Migrate();
                }
            }

            return app;
        }
    }
}

