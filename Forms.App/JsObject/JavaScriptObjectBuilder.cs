using Lycoris.Common.Extensions;
using WinFormium.CefGlue;
using WinFormium.Sources.JavaScript.JavaScriptEngine;

namespace Forms.App.Main.JsObject
{
    internal abstract class JavaScriptObjectBuilder
    {
        // window.external.
        private JavaScriptObject JsObject = new JavaScriptObject();

        public delegate void InvokeOnUIThread(Action action);

        internal abstract string JsObjectName { get; }

        protected readonly CefBrowser? Browser;
        protected readonly InvokeOnUIThread Invoke;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="invoke"></param>
        protected JavaScriptObjectBuilder(CefBrowser? browser, InvokeOnUIThread invoke)
        {
            this.Browser = browser;
            this.Invoke = invoke;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, int value)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, long value)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value.ToString()));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, decimal value)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, string value)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, bool value)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, DateTime value)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, Func<JavaScriptValue> configure)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), configure);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddProperty(string propertyName, Func<int> getter, Action<int> setter)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(getter()), value => setter(((int?)value ?? 0)));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddProperty(string propertyName, Func<long> getter, Action<long> setter)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(getter().ToString()), value =>
            {
                var temp = ((string?)value ?? string.Empty);
                setter(temp.ToTryLong() ?? 0);
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddProperty(string propertyName, Func<string> getter, Action<string> setter)
        {
            this.JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(getter()), value => setter(((string?)value ?? string.Empty)));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod(string methodName, Action configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), ((args) =>
            {
                try
                {
                    configure.Invoke();
                    return new JavaScriptValue(true);
                }
                catch (Exception)
                {
                    return new JavaScriptValue(false);
                }
            }));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod(string methodName, Action<JavaScriptArray> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), ((args) =>
            {
                try
                {
                    configure.Invoke(args);
                    return new JavaScriptValue(true);
                }
                catch (Exception)
                {
                    return new JavaScriptValue(false);
                }
            }));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod(string methodName, Func<JavaScriptArray, int> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), ((args) =>
            {
                try
                {
                    var result = configure.Invoke(args);
                    return new JavaScriptValue(result);
                }
                catch (Exception)
                {
                    return null;
                }
            }));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod(string methodName, Func<JavaScriptArray, long> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), ((args) =>
            {
                try
                {
                    var result = configure.Invoke(args);
                    return new JavaScriptValue(result.ToString());
                }
                catch (Exception)
                {
                    return null;
                }
            }));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod(string methodName, Func<JavaScriptArray, string> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), ((args) =>
            {
                try
                {
                    var result = configure.Invoke(args);
                    return new JavaScriptValue(result);
                }
                catch (Exception)
                {
                    return null;
                }
            }));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod(string methodName, Func<JavaScriptArray, DateTime> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), ((args) =>
            {
                try
                {
                    var result = configure.Invoke(args);
                    return new JavaScriptValue(result);
                }
                catch (Exception)
                {
                    return null;
                }
            }));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod<T>(string methodName, Func<JavaScriptArray, T> configure) where T : class, new()
        {
            this.JsObject.Add(methodName.ToCamelCase(), ((args) =>
            {
                try
                {
                    var result = configure.Invoke(args);
                    return new JavaScriptValue(result.ToJson());
                }
                catch (Exception)
                {
                    return null;
                }
            }));

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddAsyncMethod(string methodName, Func<JavaScriptArray, Task> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), async (args, promise) =>
            {
                try
                {
                    await configure.Invoke(args);
                    promise.Resolve();
                }
                catch (Exception ex)
                {
                    promise.Reject(ex.Message);
                }
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddAsyncMethod(string methodName, Func<JavaScriptArray, Task<int>> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), async (args, promise) =>
            {
                try
                {
                    var result = await configure.Invoke(args);
                    promise.Resolve(new JavaScriptValue(result));
                }
                catch (Exception ex)
                {
                    promise.Reject(ex.Message);
                }
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddAsyncMethod(string methodName, Func<JavaScriptArray, Task<long>> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), async (args, promise) =>
            {
                try
                {
                    var result = await configure.Invoke(args);
                    promise.Resolve(new JavaScriptValue(result.ToString()));
                }
                catch (Exception ex)
                {
                    promise.Reject(ex.Message);
                }
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddAsyncMethod(string methodName, Func<JavaScriptArray, Task<string>> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), async (args, promise) =>
            {
                try
                {
                    var result = await configure.Invoke(args);
                    promise.Resolve(new JavaScriptValue(result));
                }
                catch (Exception ex)
                {
                    promise.Reject(ex.Message);
                }
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddAsyncMethod(string methodName, Func<JavaScriptArray, Task<DateTime>> configure)
        {
            this.JsObject.Add(methodName.ToCamelCase(), async (args, promise) =>
            {
                try
                {
                    var result = await configure.Invoke(args);
                    promise.Resolve(new JavaScriptValue(result));
                }
                catch (Exception ex)
                {
                    promise.Reject(ex.Message);
                }
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddAsyncMethod<T>(string methodName, Func<JavaScriptArray, Task<T>> configure) where T : class, new()
        {
            this.JsObject.Add(methodName.ToCamelCase(), async (args, promise) =>
            {
                try
                {
                    var result = await configure.Invoke(args);
                    promise.Resolve(new JavaScriptValue(result.ToJson()));
                }
                catch (Exception ex)
                {
                    promise.Reject(ex.Message);
                }
            });
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formium"></param>
        /// <param name="frame"></param>
        internal JavaScriptObjectMap Build()
        {
            this.Initialize();
            return new JavaScriptObjectMap() { Name = this.JsObjectName, JsObject = this.JsObject };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected void InvokeAsync(Func<Task> task) => this.Invoke(() => task.Invoke().RunSync());
    }
}
