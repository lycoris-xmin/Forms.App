using Forms.App.Model;
using Microsoft.Extensions.DependencyInjection;
using WinFormium.Sources.Bootstrapper;
using WinFormium.Sources.WebResource.LocalFile;

namespace Forms.App.Main
{
    internal class Startup : WinFormiumStartup
    {
        protected override MainWindowCreationAction? UseMainWindow(MainWindowOptions opts)
        {
            // 设置应用程序的主窗体
            return opts.UseMainFormium<MainWindow>();
        }
        protected override void WinFormiumMain(string[] args)
        {
            ApplicationConfiguration.Initialize();
        }

        protected override void ConfigurationChromiumEmbedded(ChromiumEnvironmentBuiler cef)
        {
            // 在此处配置 Chromium Embedded Framwork
            //cef.ConfigureCommandLineArguments(cmdLine =>
            //{
            //    cmdLine.AppendArgument("disable-web-security");
            //    cmdLine.AppendSwitch("no-proxy-server");
            //});

            cef.UseCustomUserDataDirectory(Path.Combine(AppContext.BaseDirectory, "data"));
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            // 在这里配置该应用程序的服务
            services.AddLocalFileResource(new LocalFileResourceOptions
            {
                Scheme = "http",
                DomainName = "files.app.local",
                PhysicalFilePath = AppPath.WebRootPath
            });
        }
    }
}
