using Forms.App.Main.JsObject.Builder;
using Forms.App.Main.Shared;
using Forms.App.Model.Contexts;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;
using Vanara.Extensions;
using WinFormium.CefGlue;
using WinFormium.Sources.JavaScript.JavaScriptEngine;

namespace Forms.App.Main.JsObject.Objects
{
    [JavaScriptRegister("Api")]
    public class ApiJavaScriptObject : JavaScriptObjectBuilder
    {
        public ApiJavaScriptObject(CefBrowser? browser, InvokeOnUIThread invoke) : base(browser, invoke)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Initialize()
        {
            var apiClasses = GetApiClass();
            if (!apiClasses.HasValue())
                return;

            var apiMethods = GetApiMenthods(apiClasses);
            if (!apiMethods.HasValue())
                return;

            foreach (var @class in apiMethods)
            {
                var jsObj = new JavaScriptObject();

                object?[] ConvertArgs(JavaScriptArray args, MethodInfo method) => ConvertArguments(args, method);

                async Task HandleAsyncNoReturn(JavaScriptArray args, JavaScriptPromise promise, MethodInfo method)
                {
                    try
                    {
                        using var scope = FormAppContext.ServiceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService(@class.ClassType);

                        var argValues = ConvertArgs(args, method);
                        var task = method.Invoke(service, argValues) as Task;

                        if (task != null)
                            await task;

                        promise.Resolve(true);
                    }
                    catch (Exception ex)
                    {
                        promise.Reject(ex.Message);
                    }
                }

                async Task HandleAsyncWithReturn(JavaScriptArray args, JavaScriptPromise promise, MethodInfo method)
                {
                    try
                    {
                        using var scope = FormAppContext.ServiceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService(@class.ClassType);

                        var argValues = ConvertArgs(args, method);
                        var task = method.Invoke(service, argValues);

                        await (Task)task!;

                        var resultProperty = task.GetType().GetProperty("Result");
                        var result = resultProperty?.GetValue(task);

                        promise.Resolve(ConvertReturnValue(method.ReturnType, result));
                    }
                    catch (Exception ex)
                    {
                        promise.Reject(ex.Message);
                    }
                }

                JavaScriptValue HandleSyncNoReturn(JavaScriptArray args, MethodInfo method)
                {
                    try
                    {
                        using var scope = FormAppContext.ServiceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService(@class.ClassType);

                        method.Invoke(service, ConvertArgs(args, method));
                        return new JavaScriptValue(true);
                    }
                    catch
                    {
                        return new JavaScriptValue(false);
                    }
                }

                JavaScriptValue HandleSyncWithReturn(JavaScriptArray args, MethodInfo method)
                {
                    using var scope = FormAppContext.ServiceProvider.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService(@class.ClassType);

                    var result = method.Invoke(service, ConvertArgs(args, method));
                    return ConvertReturnValue(method.ReturnType, result);
                }

                foreach (var method in @class.Methods)
                {
                    var hasReturnValue = method.MethodInfo.ReturnType != typeof(void) && method.MethodInfo.ReturnType != typeof(Task);

                    if (method.MethodInfo.IsAsync())
                    {
                        if (!hasReturnValue)
                        {
                            // 异步无返回值
                            jsObj.Add(method.ApiMethodName, async (args, promise) => await HandleAsyncNoReturn(args, promise, method.MethodInfo));
                        }
                        else
                        {
                            // 异步有返回值
                            jsObj.Add(method.ApiMethodName, async (args, promise) => await HandleAsyncWithReturn(args, promise, method.MethodInfo));
                        }
                    }
                    else
                    {
                        if (!hasReturnValue)
                        {
                            // 同步无返回值
                            jsObj.Add(method.ApiMethodName, args => HandleSyncNoReturn(args, method.MethodInfo));
                        }
                        else
                        {
                            // 同步有返回值
                            jsObj.Add(method.ApiMethodName, args => HandleSyncWithReturn(args, method.MethodInfo));
                        }
                    }
                }

                this.AddObject(@class.Name!, jsObj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Type[] GetApiClass()
        {
            var filter = this.GetType().Assembly.GetTypes()
                             .Where(x => x.IsClass && x.IsPublic)
                             .Where(x => x.IsSubclassFrom<BaseApiService>());

            return [.. filter];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private ApiType[] GetApiMenthods(Type[] types)
        {
            var filter = types.Select(x => new ApiType()
            {
                ClassType = x,
                Name = x.GetCustomAttribute<ApiModuleAttribute>(false)?.Name
            });

            var array = filter.ToArray();

            if (!array.HasValue())
                return Array.Empty<ApiType>();

            foreach (var type in array)
            {
                type.Name = type.Name!.ToCamelCase();

                var methods = type.ClassType.GetMethods().Where(x => x.IsPublic).ToArray();

                if (!methods.HasValue())
                    continue;

                type.Methods = new List<ApiMethodType>();

                foreach (var item in methods)
                {
                    var attr = item.GetCustomAttribute<ApiMethodAttribute>(false) ?? new ApiMethodAttribute();

                    if (attr.Name.IsNullOrEmpty())
                        attr.Name = RemoveAsyncSuffix(item.Name);

                    type.Methods.Add(new ApiMethodType()
                    {
                        ApiMethodName = !attr.Name.IsNullOrEmpty() ? attr.Name!.ToCamelCase() : "",
                        MethodInfo = item
                    });
                }
            }

            return [.. array.Where(x => x.Methods != null && x.Methods.Count != 0)];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string RemoveAsyncSuffix(string name)
        {
            const string suffix = "Async";
            return name.EndsWith(suffix)
                ? name.Substring(0, name.Length - suffix.Length)
                : name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static object?[] ConvertArguments(JavaScriptArray args, MethodInfo method)
        {
            var paramer = method.GetParameters();

            var result = new object?[paramer.Length];

            for (int i = 0; i < paramer.Length; i++)
            {
                var targetType = paramer[i];

                var jsValue = args[i];

                result[i] = ConvertToParamer(jsValue, targetType.ParameterType);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object? ConvertToParamer(JavaScriptValue value, Type targetType)
        {
            if (value == null)
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            if (targetType == typeof(string))
                return value.GetString();
            else if (targetType == typeof(int))
                return value.GetInt();
            else if (targetType == typeof(float))
                return value.GetFloat();
            else if (targetType == typeof(double))
                return value.GetDouble();
            else if (targetType == typeof(bool))
                return value.GetBoolean();
            else if (targetType == typeof(decimal))
                return value.GetDecimal();
            else if (targetType == typeof(DateTime))
                return value.GetDateTime();

            // 先转为 JSON 字符串再反序列化为目标类型
            var json = value.ToJson();

            return JsonConvert.DeserializeObject(json, targetType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private JavaScriptValue ConvertReturnValue(Type returnType, object? result)
        {
            if (result == null)
            {
                if (returnType == typeof(string))
                    return new JavaScriptValue("");

                return new JavaScriptValue();
            }
            else if (returnType == typeof(string))
            {
                return new JavaScriptValue((string)result);
            }
            else if (returnType == typeof(int))
            {
                return new JavaScriptValue((int)result);
            }
            else if (returnType == typeof(long))
            {
                return new JavaScriptValue(result.ToString()!);
            }
            else if (returnType == typeof(double))
            {
                return new JavaScriptValue(((double)result).ToString("0.00")!);
            }
            else if (returnType == typeof(decimal))
            {
                return new JavaScriptValue(((decimal)result).ToString("0.00")!);
            }
            else if (returnType == typeof(DateTime))
            {
                return new JavaScriptValue(((DateTime)result).ToString("yyyy-MM-dd HH:mm:ss")!);
            }

            var js = new JavaScriptObject();

            var props = result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                var value = prop.GetValue(result);
                js.Add(prop.Name.ToCamelCase(), ConvertReturnValue(prop.PropertyType, value));
            }

            return js;
        }

        /// <summary>
        /// 
        /// </summary>
        private class ApiType
        {
            /// <summary>
            /// 
            /// </summary>
            public Type ClassType { get; set; } = default!;

            /// <summary>
            /// 
            /// </summary>
            public string? Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<ApiMethodType> Methods { get; set; } = default!;
        }

        /// <summary>
        /// 
        /// </summary>
        private class ApiMethodType
        {
            /// <summary>
            /// 
            /// </summary>
            public string ApiMethodName { get; set; } = default!;

            public MethodInfo MethodInfo { get; set; } = default!;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ApiModuleAttribute : Attribute
    {
        public string Name { get; set; }

        public ApiModuleAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ApiMethodAttribute : Attribute
    {
        public string? Name { get; set; }

        public ApiMethodAttribute() { }

        public ApiMethodAttribute(string name)
        {
            Name = name;
        }
    }
}
