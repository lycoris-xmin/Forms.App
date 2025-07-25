﻿using Lycoris.Common.ConfigurationManager;
using Lycoris.Common.Extensions;

namespace Forms.App.Model
{
    public class AppSettings
    {
        private static bool? _IsDebugger = null;

        /// <summary>
        /// 是否开发环境
        /// </summary>
        public static bool IsDebugger
        {
            get
            {
                if (_IsDebugger.HasValue)
                    return _IsDebugger.Value;

                _IsDebugger = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "DevelopmentServer";
                return _IsDebugger.Value;
            }
        }

        public class Route
        {
            /// <summary>
            /// 
            /// </summary>
            public static string MainUrl
            {
                get
                {
                    if (IsDebugger)
                    {
                        return "http://127.0.0.1:7777";
                    }
                    else
                    {
                        return "http://files.app.local";
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public static string LaunchUrl
            {
                get
                {
                    if (IsDebugger)
                    {
                        return "http://127.0.0.1:7777";
                    }
                    else
                    {
                        return "http://files.app.local";
                    }
                }
            }
        }

        public class Application
        {
            private static string? _Host = null;
            /// <summary>
            /// 
            /// </summary>
            public static string Host
            {
                get
                {
                    if (_Host != null)
                        return _Host;

                    _Host = "Application:Host".TryGetConfig();
                    _Host ??= "http://127.0.0.1:9999";
                    return _Host;
                }
            }

            private static int? _LocalPort = null;
            /// <summary>
            /// 
            /// </summary>
            public static int LocalPort
            {
                get
                {
                    if (_LocalPort.HasValue)
                        return _LocalPort.Value;

                    var tmp = "Application:LocalPort".TryGetConfig();
                    _LocalPort = tmp.ToTryInt() ?? 5004;
                    return _LocalPort.Value;
                }
            }

            private static int? _ProxyPort = null;
            /// <summary>
            /// 
            /// </summary>
            public static int ProxyPort
            {
                get
                {
                    if (_ProxyPort.HasValue)
                        return _ProxyPort.Value;

                    var tmp = "Application:ProxyPort".TryGetConfig();
                    _ProxyPort = tmp.ToTryInt() ?? 5004;
                    return _ProxyPort.Value;
                }
            }
        }

        /// <summary>
        /// Sql数据库配置
        /// </summary>
        public class Sql
        {
            /// <summary>
            /// sql 链接字符串
            /// </summary>
            public static string ConnectionString
            {
                get
                {
                    if (!Directory.Exists(AppPath.Data))
                        Directory.CreateDirectory(AppPath.Data);

                    return $"Data Source={Path.Combine(AppPath.Data, "source/data/app.db")}";
                }
            }

            private static string? _TablePrefix = null;

            /// <summary>
            /// 
            /// </summary>
            public static string TablePrefix
            {
                get
                {
                    if (_TablePrefix != null)
                        return _TablePrefix;

                    _TablePrefix = "Sql:TablePrefix".TryGetConfig();
                    _TablePrefix ??= "";
                    _TablePrefix = _TablePrefix.Trim();

                    return _TablePrefix;
                }
            }

            private static bool? _ShowSqlToConsole = null;
            /// <summary>
            /// 打印至控制台
            /// </summary>
            public static bool ShowSqlToConsole
            {
                get
                {
                    if (_ShowSqlToConsole.HasValue)
                        return _ShowSqlToConsole.Value;

                    var val = "Sql:ShowSqlToConsole".TryGetConfig();
                    if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool _temp))
                        _ShowSqlToConsole = _temp;

                    _ShowSqlToConsole ??= false;
                    return _ShowSqlToConsole.Value;
                }
            }
        }

        /// <summary>
        /// Serilog配置
        /// </summary>
        public static class Serilog
        {
            private static string? _MinLevel = null;

            /// <summary>
            /// 允许记录日志的最小等级
            /// </summary>
            public static string MinLevel
            {
                get
                {
                    if (_MinLevel != null)
                        return _MinLevel;

                    _MinLevel = "Serilog:MinLevel".TryGetConfig();
                    _MinLevel ??= "Information";
                    return _MinLevel;
                }
            }

            private static bool? _Console = null;

            /// <summary>
            /// 打印至控制台
            /// </summary>
            public static bool Console
            {
                get
                {
                    if (_Console.HasValue)
                        return _Console.Value;

                    var val = "Serilog:Console".TryGetConfig();
                    if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool _temp))
                        _Console = _temp;

                    _Console ??= false;
                    return _Console.Value;
                }
            }

            private static bool? _File = null;

            /// <summary>
            /// 是否开启日志文件记录
            /// </summary>
            public static bool File
            {
                get
                {
                    if (_File.HasValue)
                        return _File.Value;

                    var val = "Serilog:File".TryGetConfig();
                    if (!val.IsNullOrEmpty() && bool.TryParse(val, out bool _temp))
                        _File = _temp;

                    _File ??= false;
                    return _File.Value;
                }
            }

            private static List<SerilogOverrideOptions>? _SerilogOverrideOptions { get; set; }

            /// <summary>
            /// 忽略程序集日志列表
            /// </summary>
            public static List<SerilogOverrideOptions> SerilogOverrideOptions
            {
                get
                {
                    if (_SerilogOverrideOptions != null)
                        return _SerilogOverrideOptions;

                    _SerilogOverrideOptions = SettingManager.GetSection<List<SerilogOverrideOptions>>("Serilog:Overrides");
                    _SerilogOverrideOptions ??= new List<SerilogOverrideOptions>();
                    return _SerilogOverrideOptions;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SerilogOverrideOptions
    {
        /// <summary>
        /// 程序集
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 允许记录的最小等级
        /// </summary>
        public string MinLevel { get; set; } = string.Empty;
    }
}
