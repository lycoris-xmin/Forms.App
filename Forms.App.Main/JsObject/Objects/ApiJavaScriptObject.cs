using Castle.DynamicProxy.Internal;
using Forms.App.Core.Contexts;
using Forms.App.Main.JsObject.Builder;
using Forms.App.Main.Shared;
using Lycoris.Autofac.Extensions;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
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

            var apiTypes = GetApiTypes(apiClasses);
            if (!apiTypes.HasValue())
                return;

            foreach (var api in apiTypes)
            {
                var jsObj = new JavaScriptObject();
                var serviceType = api.InterfaceType ?? api.ClassType;

                // 异步无返回值
                async Task HandleAsyncNoReturn(JavaScriptArray args, JavaScriptPromise promise, MethodInfo method)
                {
                    try
                    {
                        using var scope = FormAppContext.ServiceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService(serviceType);
                        var task = (Task)method.Invoke(service, ConvertArgs(args, method))!;
                        await task;
                        promise.Resolve(true);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                        promise.Reject(ex.Message);
                    }
                }

                // 异步有返回值
                async Task HandleAsyncWithReturn(JavaScriptArray args, JavaScriptPromise promise, MethodInfo method)
                {
                    try
                    {
                        using var scope = FormAppContext.ServiceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService(serviceType);
                        var input = ConvertArgs(args, method);
                        var task = method.Invoke(service, input)!;
                        await (Task)task;

                        var result = task.GetType().GetProperty("Result")?.GetValue(task);
                        promise.Resolve(ConvertCsharpToJavaScriptValue(result));
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                        promise.Reject(ex.Message);
                    }
                }

                // 同步有返回值
                JavaScriptValue HandleSyncWithReturn(JavaScriptArray args, MethodInfo method)
                {
                    try
                    {
                        using var scope = FormAppContext.ServiceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService(serviceType);
                        var result = method.Invoke(service, ConvertArgs(args, method));
                        return ConvertCsharpToJavaScriptValue(result);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                        return ConvertCsharpToJavaScriptValue(null);
                    }
                }

                // 同步无返回值
                JavaScriptValue HandleSyncNoReturn(JavaScriptArray args, MethodInfo method)
                {
                    try
                    {
                        using var scope = FormAppContext.ServiceProvider.CreateScope();
                        var service = scope.ServiceProvider.GetRequiredService(serviceType);
                        method.Invoke(service, ConvertArgs(args, method));
                        return new JavaScriptValue(true);
                    }
                    catch (Exception ex)
                    {
                        HandleException(ex);
                        return new JavaScriptValue(false);
                    }
                }

                foreach (var method in api.Methods!)
                {
                    var hasReturn = method.MethodInfo.ReturnType != typeof(void) && method.MethodInfo.ReturnType != typeof(Task);
                    if (method.MethodInfo.IsAsync())
                    {
                        if (hasReturn)
                            jsObj.Add(method.ApiMethodName, async (args, promise) => await HandleAsyncWithReturn(args, promise, method.MethodInfo));
                        else
                            jsObj.Add(method.ApiMethodName, async (args, promise) => await HandleAsyncNoReturn(args, promise, method.MethodInfo));
                    }
                    else
                    {
                        if (hasReturn)
                            jsObj.Add(method.ApiMethodName, args => HandleSyncWithReturn(args, method.MethodInfo));
                        else
                            jsObj.Add(method.ApiMethodName, args => HandleSyncNoReturn(args, method.MethodInfo));
                    }
                }

                AddObject(api.Name!, jsObj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Type[] GetApiClass()
        {
            return GetType().Assembly.GetTypes()
                       .Where(x => x.IsClass && x.IsPublic && x.IsSubclassFrom<BaseApiService>())
                       .ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        private ApiType[] GetApiTypes(Type[] types)
        {
            var results = new List<ApiType>();

            foreach (var type in types)
            {
                var registerAttr = type.GetCustomAttribute<AutofacRegisterAttribute>(false);
                if (registerAttr == null) continue;

                var api = new ApiType
                {
                    ClassType = type,
                    Name = type.GetCustomAttribute<ApiModuleAttribute>(false)?.Name?.ToCamelCase()
                };

                if (!registerAttr.Self)
                {
                    var interfaces = type.GetAllInterfaces();
                    var matched = interfaces.FirstOrDefault(x => x.Name.EndsWith(type.Name)) ?? interfaces.LastOrDefault();
                    if (matched != null)
                        api.InterfaceType = matched;
                }

                var methods = GetApiMethods(api.ClassType, api.InterfaceType);
                if (methods.HasValue())
                {
                    api.Methods = methods;
                    results.Add(api);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="implType"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        private List<ApiMethodType>? GetApiMethods(Type implType, Type? interfaceType)
        {
            var list = new List<ApiMethodType>();

            var methods = implType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            if (!methods.Any())
                return null;

            foreach (var implMethod in methods)
            {
                ApiMethodAttribute? attr = null;

                // 如果有接口映射，先从接口方法找特性
                MethodInfo? finalMethod = implMethod;

                if (interfaceType != null)
                {
                    var map = implType.GetInterfaceMap(interfaceType);

                    for (int i = 0; i < map.TargetMethods.Length; i++)
                    {
                        if (map.TargetMethods[i] == implMethod)
                        {
                            var interfaceMethod = map.InterfaceMethods[i];

                            attr = interfaceMethod.GetCustomAttribute<ApiMethodAttribute>(false);
                            attr ??= implMethod.GetCustomAttribute<ApiMethodAttribute>(true);
                            finalMethod = attr != null ? interfaceMethod : null;

                            break;
                        }
                    }
                }

                // 若接口没有标注特性，再尝试实现类方法本身
                attr ??= implMethod.GetCustomAttribute<ApiMethodAttribute>(false);

                // 没有特性就跳过
                if (attr == null || finalMethod == null)
                    continue;

                var apiName = string.IsNullOrWhiteSpace(attr.Name) ? RemoveAsyncSuffix(implMethod.Name) : attr.Name;

                list.Add(new ApiMethodType
                {
                    ApiMethodName = apiName.ToCamelCase(),
                    MethodInfo = finalMethod
                });
            }

            return list;
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
            public Type? InterfaceType { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string? Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public List<ApiMethodType>? Methods { get; set; }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public ApiModuleAttribute(string name) => Name = name.EndsWith("ApiService") ? name[..^"ApiService".Length] : name;
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

