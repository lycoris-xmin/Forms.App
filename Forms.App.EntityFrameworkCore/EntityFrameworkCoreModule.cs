using Forms.App.EntityFrameworkCore.Contexts;
using Forms.App.Model;
using Lycoris.Autofac.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Forms.App.EntityFrameworkCore
{
    public class EntityFrameworkCoreModule : AutofacRegisterModule
    {
        public override void SerivceRegister(IServiceCollection services)
        {
            services.AddScoped(sp => sp.GetRequiredService<SqlServerContextScopedFactory>().CreateDbContext());

            services.AddPooledDbContextFactory<SqlServerContext>((sp, opt) =>
            {
                opt.UseSqlite(AppSettings.Sql.ConnectionString, builder =>
                {
                    builder.MinBatchSize(4);
                    builder.CommandTimeout(60);
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });

                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

                opt.EnableSensitiveDataLogging();

                opt.EnableDetailedErrors();

                //opt.LogTo(Console.WriteLine, LogLevel.Information);
            });

            base.SerivceRegister(services);
        }
    }
}
