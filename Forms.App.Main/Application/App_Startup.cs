using Forms.App.Core.Contexts;
using Forms.App.Core.Logging;
using Forms.App.Model;
using Microsoft.Extensions.DependencyInjection;
using WinFormium.CefGlue;
using WinFormium.Sources.Bootstrapper;
using WinFormium.Sources.WebResource.Data;
using WinFormium.Sources.WebResource.LocalFile;

namespace Forms.App.Main.Application
{
    internal class App_Startup : WinFormiumStartup
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

                return factory.CreateLogger<App_Startup>();
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

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cef"></param>
        protected override void ConfigurationChromiumEmbedded(ChromiumEnvironmentBuiler cef)
        {
            // 在此处配置 Chromium Embedded Framwork
            cef.ConfigureCommandLineArguments(opt =>
            {
                //cmdLine.AppendSwitch("no-proxy-server");
                opt.AppendSwitch("ignore-certificate-errors");
                opt.AppendSwitch("disable-web-security");
                opt.AppendSwitch("enable-media-stream");
                opt.AppendSwitch("enable-print-preview");
                opt.AppendSwitch("enable-gpu");
                opt.AppendSwitch("autoplay-policy", "no-user-gesture-required");
            });

            // 
            cef.ConfigureDefaultSettings(opt =>
            {
                opt.WindowlessRenderingEnabled = true;
            });

            // 
            cef.UseCustomUserDataDirectory(Path.Combine(AppPath.RootPath, "Local"));

            // 
            cef.ConfigureSubprocess(opt =>
            {
                opt.SubprocessFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FormiumClientSubprocess.exe");
                opt.SubprocessNotExists = (architecture, path) =>
                {
                    return MessageBox.Show($"", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                };
            });
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
