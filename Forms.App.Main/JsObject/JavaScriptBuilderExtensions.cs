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
        /// ����ע����ӳ��
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
                              Host = x.Attr.Host,
                              Path = x.Attr.Path,
                              Type = x.Type
                          }).ToList();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="formuinm"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        internal static Dictionary<string, JavaScriptObject> GetOrCreateJavaScriptObject(this BaseFormiumWindow formuinm, Uri uri)
        {
            if (!Map.HasValue())
                return new Dictionary<string, JavaScriptObject>();

            var filterMaps = new Dictionary<string, JavaScriptObject>();

            foreach (var item in Map)
            {
                if (!item.Host.IsNullOrEmpty() && !uri.Host!.Contains(item.Host!))
                    continue;

                if (!item.Path.IsNullOrEmpty() && !item.Path!.Equals(uri.AbsolutePath))
                    continue;

                if (item.Instance == null)
                {
                    // ��ȡ�򴴽�����ί��
                    if (!FactoryMap.TryGetValue(item.Type, out var factory))
                    {
                        factory = CreateFactory(item.Type);
                        FactoryMap[item.Type] = factory;
                    }

                    var builder = factory(formuinm.BrowserInstance, formuinm.InvokeThead);

                    item.Instance = builder.Build();
                }

                filterMaps.Add(item.Name, item.Instance);
            }

            return filterMaps ?? new Dictionary<string, JavaScriptObject>();
        }

        /// <summary>
        /// �������������캯��ί��
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

