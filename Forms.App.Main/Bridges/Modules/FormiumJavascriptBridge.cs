using Forms.App.Core.Contexts;
using Forms.App.Core.Logging;
using Forms.App.Model.Bridges.Modules;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using WinFormium.Sources.JavaScript.JavaScriptEngine;

namespace Forms.App.Main.Bridges.Modules
{
    public class FormiumJavascriptBridge : IFormiumJavascriptBridge
    {
        private readonly IServerLogger _logger;
        private readonly MainWindow _mainWindow;

        public FormiumJavascriptBridge(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _logger = FormAppContext.ServiceProvider.GetRequiredService<IServerLoggerFactory>().CreateLogger<FormiumJavascriptBridge>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="url"></param>
        /// <param name="line"></param>
        public void Execute(string code, string url = "", int line = 0)
        {
            try
            {
                _logger.Info($"excete javacripte:{code}");

                _mainWindow.ExecuteJavaScript(code, url, line);
            }
            catch (Exception ex)
            {
                _logger.Error($"excete javacripte:{code} failed", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="url"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(string code, string url = "", int line = 0)
        {
            if (_mainWindow == null)
                return;

            try
            {
                _logger.Info($"excete javacripte:{code}");

                await _mainWindow.EvaluateJavaScriptAsync(code, url, line);
            }
            catch (Exception ex)
            {
                _logger.Error($"excete javacripte:{code} failed", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="url"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public async Task<T?> ExecuteAsync<T>(string code, string url = "", int line = 0)
        {
            if (_mainWindow == null)
                return default;

            try
            {
                _logger.Info($"excete javacripte:{code}");

                var result = await _mainWindow.EvaluateJavaScriptAsync(code, url, line);

                if (result != null && result.Success)
                {
                    var obj = ConvertJavacriptValueToObject(result.ReturnValue);
                    var json = obj.ToJson();

                    _logger.Info($"excete javacripte result - {json}");

                    return json.ToTryObject<T>();
                }

                _logger.Error($"excete javacripte:{code} failed");
                return default;
            }
            catch (Exception ex)
            {
                _logger.Error($"excete javacripte:{code} error:{ex.Message}", ex);
                return default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object? ConvertJavacriptValueToObject(JavaScriptValue value)
        {
            switch (value.ValueType)
            {
                case JavaScriptValueType.Null:
                case JavaScriptValueType.Undefined:
                    return null;
                case JavaScriptValueType.Number:
                    return value.GetDouble();
                case JavaScriptValueType.String:
                    return value.GetString();
                case JavaScriptValueType.Bool:
                    return value.GetBoolean();
                case JavaScriptValueType.Date:
                    return value.GetDateTime();
                case JavaScriptValueType.Object:
                    var obj = value.ToObject();
                    var result = new Dictionary<string, object?>();
                    foreach (var kv in obj)
                    {
                        result[kv.Key] = ConvertJavacriptValueToObject(kv.Value);
                    }
                    return result;
                case JavaScriptValueType.Array:
                    var array = value.ToArray();
                    var list = new List<object?>();
                    foreach (var item in array)
                    {
                        list.Add(ConvertJavacriptValueToObject(item));
                    }
                    return list;
                default:
                    return null;
            }
        }
    }
}
