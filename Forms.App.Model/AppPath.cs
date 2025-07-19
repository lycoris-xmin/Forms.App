namespace Forms.App.Model
{
    public class AppPath
    {
        private static string? _RootPath = null;

        /// <summary>
        /// 程序根目录
        /// </summary>
        public static string RootPath
        {
            get
            {
                if (_RootPath != null)
                    return _RootPath;

                _RootPath = AppSettings.IsDebugger ? Directory.GetCurrentDirectory() : AppContext.BaseDirectory;
                _RootPath = _RootPath.TrimEnd('/').TrimEnd('\\').Replace("\\", "/");
                return _RootPath;
            }
        }

        /// <summary>
        /// wwwroot路径
        /// </summary>
        public static string WebRootPath => $"{RootPath}/source";

        /// <summary>
        /// AppData文件
        /// </summary>
        public static string Data { get => $"{RootPath}/data"; }

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string JsonFile
        {
            get
            {
                if (!AppSettings.IsDebugger)
                    return Path.Combine(RootPath, "appsettings.json");

                var localhost = Path.Combine(RootPath, "appsettings.Localhost.json");
                if (File.Exists(localhost))
                    return localhost;

                var devlopment = Path.Combine(RootPath, "appsettings.Development.json");
                if (File.Exists(devlopment))
                    return devlopment;

                return Path.Combine(RootPath, "appsettings.json");
            }
        }

        /// <summary>
        /// 缓存文件夹
        /// </summary>
        public static string Temp => Path.Combine(RootPath, "temp");

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public static string LogFolder => Path.Combine(RootPath, "logs");

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public static string LogFile => Path.Combine(LogFolder, "logs/log.txt");

        /// <summary>
        /// 缓存文件夹
        /// </summary>
        public static string TempFolder => Path.Combine(RootPath, "temp");
    }
}
