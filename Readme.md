### 底层功能由 [NanUI.WinFormium](https://github.com/lycoris-xmin/NanUI.WinFormium) 改造而来，本地上底层还是属于 [NanUI](https://github.com/XuanchenLin/NanUI)

### 此处感谢大佬的开源代码 [XuanchenLin](https://github.com/XuanchenLin)

### [NanUI.WinFormium](https://github.com/lycoris-xmin/NanUI.WinFormium) 的改造也正是为了适配这套程序，来接入我自己常用的部分扩展和替换 IOC 容器

```csharp
/// <summary>
///  The main entry point for the application.
/// </summary>
[STAThread]
static void Main()
{
    // 配置文件
    SettingManager.JsonConfigurationInitialization(AppPath.JsonFile);

    // 设置Ini配置文件
    ConfigHelper.SetConfigPath(AppPath.Data);

    var hostBuilder = Host.CreateDefaultBuilder();

    // serilog 接管日志组件
    hostBuilder.UseSerilog((context, config) =>
    {
        AppSettings.Serilog.SerilogOverrideOptions.ForEach(OverrideOption => config.MinimumLevel.Override(OverrideOption.Source,OverrideOption.MinLevel.ToEnum<LogEventLevel>()));

        config.MinimumLevel.Is(AppSettings.Serilog.MinLevel.ToEnum<LogEventLevel>());

        if (AppSettings.Serilog.Console)
            config.WriteTo.Console();

        if (AppSettings.IsDebugger)
            config.WriteTo.Debug();

        if (AppSettings.Serilog.File)
        {
            var template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} - {Level:u3} - {SourceContext:l} - {Message:lj}{NewLine}{Exception}";
            config.WriteTo.File(AppPath.LogFile, outputTemplate: template, rollingInterval: RollingInterval.Day, shared: true, fileSizeLimitBytes: 10 * 1025 * 1024,rollOnFileSizeLimit: true);
        }
    });

    // 引入自己的autofac扩展
    hostBuilder.UseAutofacExtensions(opt =>
    {
        opt.AddRegisterModule<ModelModule>();
        opt.AddRegisterModule<CommonModule>();
        opt.AddRegisterModule<EntityFrameworkCoreModule>();
        opt.AddRegisterModule<CoreModule>();
        opt.AddRegisterModule<ServiceModule>();
        opt.AddRegisterModule<MainModule>();
    });

    var appBuilder = WinFormiumApp.CreateBuilder();

    hostBuilder.ConfigureServices(services =>
    {
        // 替换掉 Nanui 自带的 IOC 容器
        appBuilder.UserServiceCollection(services);

        appBuilder.UseEmbeddedBrowser();

        appBuilder.UseCulture("zh-CN");

        appBuilder.UseSingleApplicationInstanceMode(handler =>
        {
            var retval = MessageBox.Show($"已经有一个实例在运行了:{handler.ProcessId}。\r\n是否打开其主窗体？", "单例模式已启用", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (retval == DialogResult.Yes)
                handler.ActiveRunningInstance();
        });

        if (AppSettings.IsDebugger)
            appBuilder.UseDevToolsMenu();

        appBuilder.UseWinFormiumApp<App_Startup>();

        appBuilder!.Build();
    });

    var appHost = hostBuilder.Build();

    // 注册全局 ServiceProvider
    FormAppContext.RegisterServiceProvider(appHost.Services);

    // C#对象转js对象映射
    JavaScriptBuilderExtensions.BuildMap();

    var app = appHost.Services.GetService<WinFormiumApp>();
    // 启动程序
    app!.Run();
}
```
