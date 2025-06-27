using Forms.App.Common;
using Forms.App.Core;
using Forms.App.EntityFrameworkCore;
using Forms.App.Model;
using Forms.App.Model.Contexts;
using Forms.App.Services;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.ConfigurationManager;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using WinFormium.Sources;

namespace Forms.App.Main
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 配置文件
            SettingManager.JsonConfigurationInitialization(AppPath.JsonFile);

            var hostBuilder = Host.CreateDefaultBuilder();

            hostBuilder.UseSerilog((context, config) =>
            {
                AppSettings.Serilog.SerilogOverrideOptions.ForEach(OverrideOption => config.MinimumLevel.Override(OverrideOption.Source, OverrideOption.MinLevel.ToEnum<LogEventLevel>()));

                config.MinimumLevel.Is(AppSettings.Serilog.MinLevel.ToEnum<LogEventLevel>());

                if (AppSettings.Serilog.Console)
                    config.WriteTo.Console();

                if (AppSettings.IsDebugger)
                    config.WriteTo.Debug();

                if (AppSettings.Serilog.File)
                {
                    var template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} - {Level:u3} - {SourceContext:l} - {Message:lj}{NewLine}{Exception}";
                    config.WriteTo.File(AppPath.LogFile, outputTemplate: template, rollingInterval: RollingInterval.Day, shared: true, fileSizeLimitBytes: 10 * 1025 * 1024, rollOnFileSizeLimit: true);
                }
            });

            hostBuilder.UseAutofacExtensions(opt =>
            {
                opt.AddRegisterModule<ModelModule>();
                opt.AddRegisterModule<CommonModule>();
                opt.AddRegisterModule<EntityFrameworkCoreModule>();
                opt.AddRegisterModule<CoreModule>();
                opt.AddRegisterModule<ServiceModule>();
            });

            var appBuilder = WinFormiumApp.CreateBuilder();

            hostBuilder.ConfigureServices(services =>
            {
                appBuilder.UserServiceCollection(services);

                appBuilder.UseEmbeddedBrowser();

                appBuilder.UseCulture("zh-CN");

                appBuilder.UseSingleApplicationInstanceMode(handler =>
                {
                    var retval = MessageBox.Show($"已经有一个实例在运行了:{handler.ProcessId}。\r\n是否打开其主窗体？", "单例模式已启用", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (retval == DialogResult.Yes)
                        handler.ActiveRunningInstance();
                });

                appBuilder.UseDevToolsMenu();

                appBuilder.UseWinFormiumApp<Startup>();

                appBuilder!.Build();
            });

            var appHost = hostBuilder.Build();

            FormAppContext.ServiceProvider = appHost.Services;

            var app = appHost.Services.GetService<WinFormiumApp>();

            app!.Run();
        }
    }
}