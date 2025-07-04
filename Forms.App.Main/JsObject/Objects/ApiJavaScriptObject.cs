using Forms.App.Main.JsObject.Builder;
using Forms.App.Main.Shared;
using Forms.App.Model.Contexts;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections;
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
            if (value == null || value.ValueType == JavaScriptValueType.Undefined)
                return GetDefaultValue(targetType);

            // 基础类型和直接支持类型
            if (targetType == typeof(string))
                return value.GetString();
            if (targetType == typeof(int))
                return value.GetInt();
            if (targetType == typeof(float))
                return value.GetFloat();
            if (targetType == typeof(double))
                return value.GetDouble();
            if (targetType == typeof(bool))
                return value.GetBoolean();
            if (targetType == typeof(decimal))
                return value.GetDecimal();
            if (targetType == typeof(DateTime))
                return value.GetDateTime();

            // 枚举
            if (targetType.IsEnum)
            {
                var str = value.GetString();
                return Enum.Parse(targetType, str!, ignoreCase: true);
            }

            // Nullable<T>
            if (Nullable.GetUnderlyingType(targetType) is Type innerType)
                return ConvertToParamer(value, innerType);

            // 数组或 List<T>
            if (typeof(IEnumerable).IsAssignableFrom(targetType) && targetType != typeof(string))
            {
                var json = value.ToJson();
                return JsonConvert.DeserializeObject(json, targetType);
            }

            // Dictionary<string, T> 或其他复杂对象
            if (targetType.IsClass || targetType.IsValueType)
            {
                var json = value.ToJson();
                return JsonConvert.DeserializeObject(json, targetType);
            }

            throw new NotSupportedException($"暂不支持将 JavaScriptValue 转换为 {targetType.Name}");
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Nullable.GetUnderlyingType(type) != null
                    ? null
                    : Activator.CreateInstance(type);

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private JavaScriptValue ConvertReturnValue(Type returnType, object? result)
        {
            if (result == null)
                return returnType == typeof(string) ? new JavaScriptValue("") : new JavaScriptValue();

            if (result is string str)
                return new JavaScriptValue(str);
            if (result is int i)
                return new JavaScriptValue(i);
            if (result is long l)
                return new JavaScriptValue(l.ToString());
            if (result is double d)
                return new JavaScriptValue(d.ToString("0.00"));
            if (result is decimal dec)
                return new JavaScriptValue(dec.ToString("0.00"));
            if (result is bool b)
                return new JavaScriptValue(b);
            if (result is DateTime dt)
                return new JavaScriptValue(dt.ToString("yyyy-MM-dd HH:mm:ss"));

            // 处理数组、List
            if (typeof(IEnumerable).IsAssignableFrom(returnType) && returnType != typeof(string))
            {
                var jsArray = new JavaScriptArray();

                foreach (var item in (IEnumerable)result)
                    jsArray.Add(ConvertReturnValue(item?.GetType() ?? typeof(object), item));

                return jsArray;
            }

            // 处理 Dictionary<string, T>
            if (result is IDictionary dict)
            {
                var jsObj = new JavaScriptObject();

                foreach (DictionaryEntry entry in dict)
                {
                    var key = entry.Key?.ToString() ?? "";
                    jsObj.Add(key.ToCamelCase(), ConvertReturnValue(entry.Value?.GetType() ?? typeof(object), entry.Value));
                }

                return jsObj;
            }

            // 处理自定义对象
            var obj = new JavaScriptObject();
            var props = returnType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
            {
                if (!prop.CanRead)
                    continue;

                var value = prop.GetValue(result);
                var valueType = prop.PropertyType;
                obj.Add(prop.Name.ToCamelCase(), ConvertReturnValue(valueType, value));
            }

            return obj;
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
