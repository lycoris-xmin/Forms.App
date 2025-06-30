using Forms.App.Main.JsObject.Builder;
using Forms.App.Main.Shared;
using Lycoris.Common.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using WinFormium.CefGlue;
using WinFormium.Sources.JavaScript.JavaScriptEngine;
using static Forms.App.Main.JsObject.Builder.JavaScriptObjectBuilder;

namespace Forms.App.Main.JsObject
{
    internal static class JavaScriptBuilderExtensions
    {
        private static List<JavaScriptObjectMap> Map = new();
        private static readonly Dictionary<Type, Func<CefBrowser?, InvokeOnUIThread, JavaScriptObjectBuilder>> FactoryMap = new();

        /// <summary>
        /// 构建注册类映射
        /// </summary>
        internal static void BuildMap()
        {
            Map = Assembly.GetExecutingAssembly().GetTypes()
                          .Where(x => x.IsSubclassFrom<JavaScriptObjectBuilder>())
                          .Where(x => x.IsClass && !x.IsAbstract)
                          .Where(x => x.GetCustomAttribute<JavaScriptRegisterAttribute>(false) != null)
                          .Select(x => new
                          {
                              Attr = x.GetCustomAttribute<JavaScriptRegisterAttribute>(false)!,
                              Type = x,
                          })
                          .Select(x => new JavaScriptObjectMap()
                          {
                              Name = x.Attr.Name.ToCamelCase(),
                              Path = x.Attr.Path,
                              Type = x.Type
                          }).ToList();
        }

        /// <summary>
        /// 构建对象
        /// </summary>
        /// <param name="formuinm"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static Dictionary<string, JavaScriptObject> GetOrCreateJavaScriptObject(this BaseFormium formuinm, string path)
        {
            if (!Map.HasValue())
                return new Dictionary<string, JavaScriptObject>();

            foreach (var item in Map)
            {
                if (item.Path.IsNullOrEmpty() || item.Path!.Equals(path))
                {
                    if (item.Instance != null)
                        continue;

                    // 获取或创建工厂委托
                    if (!FactoryMap.TryGetValue(item.Type, out var factory))
                    {
                        factory = CreateFactory(item.Type);
                        FactoryMap[item.Type] = factory;
                    }

                    var builder = factory(formuinm.BrowserInstance, formuinm.InvokeThead);
                    item.Instance = builder.Build();
                }
            }

            return Map.Where(x => x.Path.IsNullOrEmpty() || x.Path!.Equals(path))
                      .OrderBy(x => (x.Path ?? "").Length)
                      .ToDictionary(x => x.Name, x => x.Instance!) ?? new Dictionary<string, JavaScriptObject>();
        }

        /// <summary>
        /// 解释器创建构造函数委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static Func<CefBrowser?, InvokeOnUIThread, JavaScriptObjectBuilder> CreateFactory(Type type)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var ctorArgs = new[] { typeof(CefBrowser), typeof(InvokeOnUIThread) };

            var ctor = type.GetConstructor(flags, binder: null, ctorArgs, modifiers: null)
                ?? throw new InvalidOperationException($"Constructor not found for type {type.FullName}");

            var browserParam = Expression.Parameter(typeof(CefBrowser), "browser");
            var invokeParam = Expression.Parameter(typeof(InvokeOnUIThread), "invoke");

            var newExpr = Expression.New(ctor, browserParam, invokeParam);

            var lambda = Expression.Lambda<Func<CefBrowser?, InvokeOnUIThread, JavaScriptObjectBuilder>>(newExpr, browserParam, invokeParam);

            return lambda.Compile();
        }
    }
}
