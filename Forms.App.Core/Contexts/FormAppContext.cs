using Forms.App.Core.Logging;
using Forms.App.Model.Bridges;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Forms.App.Core.Contexts
{
    public class FormAppContext
    {
        /// <summary>
        /// 
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public static IFormiumBridge Bridge { get; set; } = default!;

        /// <summary>
        /// 程序加载
        /// </summary>
        public static LaunchStepEnum LaunchStep { get; set; } = LaunchStepEnum.DEFAULT;

        /// <summary>
        /// 
        /// </summary>
        public static bool SchedulerIsRun { get; private set; } = false;

        /// <summary>
        /// 
        /// </summary>
        private static IServerLoggerFactory AppLoggerFactory { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        private static object? Formium { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private static object? Browser { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        public static void RegisterServiceProvider(IServiceProvider provider)
        {
            ServiceProvider = provider;
            AppLoggerFactory = provider.GetRequiredService<IServerLoggerFactory>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetFormium<T>() where T : IDisposable => Formium == null ? default : (T)Formium;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formium"></param>
        public static void RegisterFormium<T>(T formium) where T : IDisposable => Formium = formium;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T? GetCefBrowser<T>() where T : IDisposable => Browser == null ? default : (T)Browser;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="browser"></param>
        public static void RegisterBrowser<T>(T browser) where T : IDisposable => Browser = browser;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IServerLogger UseLogger<T>() where T : class => AppLoggerFactory.CreateLogger<T>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IServerLogger UseLogger(Type type) => AppLoggerFactory.CreateLogger(type);

        /// <summary>
        /// 
        /// </summary>
        public static void SetSchedulerStatus() => SchedulerIsRun = true;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LaunchStepEnum
    {
        /// <summary>
        /// 程序初始化中
        /// </summary>
        [Description("程序初始化中")]
        DEFAULT = 1,
        /// <summary>
        /// 文件初始化
        /// </summary>
        [Description("文件初始化")]
        FOLDER_INIT = 2,
    }
}

