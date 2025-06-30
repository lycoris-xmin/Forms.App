using Forms.App.Core.Logging;
using Forms.App.Model;
using Forms.App.Model.Contexts;
using Microsoft.Extensions.DependencyInjection;
using WinFormium.CefGlue;
using WinFormium.Sources.Bootstrapper;
using WinFormium.Sources.WebResource.Data;
using WinFormium.Sources.WebResource.LocalFile;

namespace Forms.App.Main
{
    internal class Startup : WinFormiumStartup
    {
        /// <summary>
        /// 
        /// </summary>
        private IServerLogger? _logger
        {
            get
            {
                var factory = FormAppContext.ServiceProvider?.GetRequiredService<IServerLoggerFactory>();
                if (factory == null)
                    return null;

                return factory.CreateLogger<Startup>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        protected override MainWindowCreationAction? UseMainWindow(MainWindowOptions opts)
        {
            // 设置应用程序的主窗体
            return opts.UseMainFormium<MainWindow>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void WinFormiumMain(string[] args)
        {
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cef"></param>
        protected override void ConfigurationChromiumEmbedded(ChromiumEnvironmentBuiler cef)
        {
            // 在此处配置 Chromium Embedded Framwork
            cef.ConfigureCommandLineArguments(cmdLine =>
            {
                cmdLine.AppendSwitch("enable-gpu");
                //cmdLine.AppendArgument("disable-web-security");
                //cmdLine.AppendSwitch("no-proxy-server");
            });

            cef.ConfigureDefaultSettings(settings =>
            {
                settings.WindowlessRenderingEnabled = true;
            });

            cef.UseCustomUserDataDirectory(Path.Combine(AppPath.RootPath, "Local"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        protected override void ConfigureServices(IServiceCollection services)
        {
            // 在这里配置该应用程序的服务
            services.AddLocalFileResource(new LocalFileResourceOptions()
            {
                Scheme = "http",
                DomainName = "files.app.local",
                PhysicalFilePath = Path.Combine(AppPath.RootPath, "Source")
            });

            services.AddDataResource("http", "api.app.local", opt => opt.ImportFromCurrentAssembly());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            {
                var ex = e.ExceptionObject as Exception;
                _logger?.Error($"CurrentDomain_UnhandledException:{ex?.Message}", ex);
            }

            try
            {
                CefRuntime.QuitMessageLoop();
                CefRuntime.Shutdown();
            }
            catch (Exception ex)
            {
                _logger?.Error($"CurrentDomain_UnhandledException:{ex?.Message}", ex);
            }
            finally
            {
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            _logger?.Error($"Application_ThreadException:{e?.Exception?.Message}", e?.Exception);

            try
            {
                CefRuntime.QuitMessageLoop();
                CefRuntime.Shutdown();
            }
            catch (Exception ex)
            {
                _logger?.Error($"CurrentDomain_UnhandledException:{ex?.Message}", ex);
            }
            finally
            {
                Environment.Exit(0);
            }
        }
    }
}
