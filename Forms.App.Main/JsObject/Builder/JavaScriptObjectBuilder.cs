using Forms.App.Core.Contexts;
using Forms.App.Model.Exceptions;
using Lycoris.Common.Extensions;
using System.Collections;
using System.Reflection;
using WinFormium.CefGlue;
using WinFormium.Sources.JavaScript.JavaScriptEngine;

namespace Forms.App.Main.JsObject.Builder
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class JavaScriptObjectBuilder
    {
        // window.external.
        private JavaScriptObject JsObject = new JavaScriptObject();

        /// <summary>
        /// UI线程执行委托
        /// </summary>
        /// <param name="action"></param>
        public delegate void InvokeOnUIThread(Action action);

        /// <summary>
        /// cef对象
        /// </summary>
        protected readonly CefBrowser? Browser;
        /// <summary>
        /// UI线程执行方法
        /// </summary>
        protected readonly InvokeOnUIThread Invoke;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="invoke"></param>
        protected JavaScriptObjectBuilder(CefBrowser? browser, InvokeOnUIThread invoke)
        {
            Browser = browser;
            Invoke = invoke;
        }

        /// <summary>
        /// 添加只读属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, int value)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 添加只读属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, long value)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value.ToString()));
            return this;
        }

        /// <summary>
        /// 添加只读属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, decimal value)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 添加只读属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, string value)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 添加只读属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, bool value)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 添加只读属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, DateTime value)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(value));
            return this;
        }

        /// <summary>
        /// 添加只读属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddReadOnlyProperty(string propertyName, Func<JavaScriptValue> configure)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), configure);
            return this;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddProperty(string propertyName, Func<int> getter, Action<int> setter)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(getter()), value => setter((int?)value ?? 0));
            return this;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddProperty(string propertyName, Func<long> getter, Action<long> setter)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(getter().ToString()), value =>
            {
                var temp = (string?)value ?? string.Empty;
                setter(temp.ToTryLong() ?? 0);
            });
            return this;
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddProperty(string propertyName, Func<string> getter, Action<string> setter)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => new JavaScriptValue(getter()), value => setter((string?)value ?? string.Empty));
            return this;
        }

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        /// 
        protected JavaScriptObjectBuilder AddMethod(Delegate del) => AddMethod(del.Method.Name, del);

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddMethod(string methodName, Delegate del)
        {
            var returnType = del.Method.ReturnType;

            if (typeof(Task).IsAssignableFrom(returnType))
            {
                return AddAsyncMethodInternal(RemoveAsyncSuffix(methodName).ToCamelCase(), del);
            }
            else
            {
                return AddSyncMethodInternal(RemoveAsyncSuffix(methodName).ToCamelCase(), del);
            }
        }

        /// <summary>
        /// 添加嵌套对象
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected JavaScriptObjectBuilder AddObject(string propertyName, JavaScriptObject obj)
        {
            JsObject.DefineProperty(propertyName.ToCamelCase(), () => obj);
            return this;
        }

        /// <summary>
        /// js对象初始化
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// js对象构建
        /// </summary>
        /// <returns></returns>
        internal JavaScriptObject Build()
        {
            Initialize();
            return JsObject;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected void InvokeAsyncMethod(Func<Task> task) => Invoke(() => task.Invoke().RunSync());

        /// <summary>
        /// 移除尾部的Async字符
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string RemoveAsyncSuffix(string name)
        {
            const string suffix = "Async";
            return name.EndsWith(suffix)
                ? name.Substring(0, name.Length - suffix.Length)
                : name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected bool IsTaskWithResult(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="del"></param>
        /// <returns></returns>
        private JavaScriptObjectBuilder AddSyncMethodInternal(string name, Delegate del)
        {
            var methodInfo = del.Method;
            var paramInfos = methodInfo.GetParameters();
            var returnType = methodInfo.ReturnType;
            var hasReturnValue = returnType != typeof(void);

            JsObject.Add(name, (args) =>
            {
                try
                {
                    var invokeArgs = new object?[paramInfos.Length];
                    for (int i = 0; i < paramInfos.Length; i++)
                    {
                        var targetType = paramInfos[i].ParameterType;
                        var jsValue = args.GetValue(i);
                        invokeArgs[i] = ConvertToParamer(jsValue, targetType);
                    }

                    var result = methodInfo.Invoke(del.Target, invokeArgs);

                    return hasReturnValue
                        ? ConvertCsharpToJavaScriptValue(result)
                        : null;
                }
                catch (Exception ex)
                {
                    FormAppContext.UseLogger<JavaScriptObjectBuilder>().Error($"[SyncError] {ex.Message}", ex);
                    return null;
                }
            });

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="del"></param>
        /// <returns></returns>
        private JavaScriptObjectBuilder AddAsyncMethodInternal(string name, Delegate del)
        {
            var methodInfo = del.Method;
            var paramInfos = methodInfo.GetParameters();
            var returnType = methodInfo.ReturnType;

            JsObject.Add(name, async (args, promise) =>
            {
                try
                {
                    // 参数转换
                    var invokeArgs = new object?[paramInfos.Length];
                    for (int i = 0; i < paramInfos.Length; i++)
                    {
                        var targetType = paramInfos[i].ParameterType;
                        var jsValue = args.GetValue(i);
                        invokeArgs[i] = ConvertToParamer(jsValue, targetType);
                    }

                    // 调用委托
                    var taskObj = methodInfo.Invoke(del.Target, invokeArgs);
                    if (taskObj == null)
                    {
                        promise.Reject("方法返回 null");
                        return;
                    }

                    // 等待任务完成
                    await (Task)taskObj;

                    // 判断是否是 Task<T>
                    if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        // 获取 Task<T>.Result
                        var result = returnType.GetProperty("Result")?.GetValue(taskObj);
                        promise.Resolve(ConvertCsharpToJavaScriptValue(result));
                    }
                    else
                    {
                        // Task（无返回值）
                        promise.Resolve(true);
                    }
                }
                catch (Exception ex)
                {
                    promise.Reject(ex.Message);
                }
            });

            return this;
        }

        /// <summary>
        /// 转换js参数为方法入参
        /// </summary>
        /// <param name="args"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected object?[] ConvertArguments(JavaScriptArray args, MethodInfo method)
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
        /// 转换js参数为方法入参
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        protected object? ConvertToParamer(JavaScriptValue value, Type targetType)
        {
            if (value == null || value.ValueType == JavaScriptValueType.Null || value.ValueType == JavaScriptValueType.Undefined)
                return GetDefaultValue(targetType);

            // 处理 Nullable<T>
            if (Nullable.GetUnderlyingType(targetType) is Type innerType)
                return ConvertToParamer(value, innerType);

            // --- 基本类型转换 ---
            if (value.ValueType == JavaScriptValueType.Number)
            {
                var number = value.GetDouble();

                if (targetType == typeof(int))
                {
                    if (number % 1 != 0)
                        throw new InvalidCastException($"无法将非整数值 {number} 转为 int 类型参数");

                    return Convert.ToInt32(number);
                }

                if (targetType == typeof(long))
                {
                    if (number % 1 != 0)
                        throw new InvalidCastException($"无法将非整数值 {number} 转为 long 类型参数");

                    return Convert.ToInt64(number);
                }

                if (targetType == typeof(float))
                    return Convert.ToSingle(number);

                if (targetType == typeof(double))
                    return number;

                if (targetType == typeof(decimal))
                    return Convert.ToDecimal(number);

                return Convert.ChangeType(number, targetType);
            }

            if (value.ValueType == JavaScriptValueType.String)
            {
                var str = value.GetString();
                var number = str?.ToTryDouble() ?? 0;

                if (targetType == typeof(int))
                {
                    if (number % 1 != 0)
                        throw new InvalidCastException($"无法将非整数值 {number} 转为 int 类型参数");

                    return Convert.ToInt32(number);
                }

                if (targetType == typeof(long))
                {
                    if (number % 1 != 0)
                        throw new InvalidCastException($"无法将非整数值 {number} 转为 long 类型参数");

                    return Convert.ToInt64(number);
                }

                if (targetType == typeof(float))
                    return Convert.ToSingle(number);

                if (targetType == typeof(double))
                    return number;

                if (targetType == typeof(decimal))
                    return Convert.ToDecimal(number);
            }

            if (targetType == typeof(string))
                return value.GetString();

            if (targetType == typeof(bool))
                return value.GetBoolean();

            if (targetType == typeof(DateTime))
                return value.GetDateTime();

            if (targetType == typeof(Guid))
                return value.GetString()!.ToGuid();

            if (targetType.IsEnum)
            {
                var str = value.GetString();

                if (string.IsNullOrEmpty(str))
                    throw new ArgumentException("枚举类型值不能为空");

                return Enum.Parse(targetType, str, ignoreCase: true);
            }

            // --- 类/结构体对象（泛型类等） ---
            if ((targetType.IsClass || targetType.IsValueType) && value.ValueType == JavaScriptValueType.Object)
            {
                var obj = value.ToObject();
                var instance = Activator.CreateInstance(targetType)!;

                var properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite);

                foreach (var prop in properties)
                {
                    if (obj.TryGetValue(prop.Name.ToCamelCase(), out var jsPropValue))
                    {
                        var propValue = ConvertToParamer(jsPropValue, prop.PropertyType);
                        prop.SetValue(instance, propValue);
                    }
                }

                return instance;
            }

            // --- 数组类型 ---
            if (targetType.IsArray && value.ValueType == JavaScriptValueType.Array)
            {
                var elementType = targetType.GetElementType()!;
                var jsArray = value.ToArray();
                var arr = Array.CreateInstance(elementType, jsArray.Count);

                for (int i = 0; i < jsArray.Count; i++)
                {
                    var elem = ConvertToParamer(jsArray[i], elementType);
                    arr.SetValue(elem, i);
                }

                return arr;
            }

            // --- List<T> 泛型列表 ---
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>) && value.ValueType == JavaScriptValueType.Array)
            {
                var elementType = targetType.GetGenericArguments()[0];
                var jsArray = value.ToArray();
                var list = (IList)Activator.CreateInstance(targetType)!;

                foreach (var item in jsArray)
                {
                    list.Add(ConvertToParamer(item, elementType));
                }

                return list;
            }

            // --- 兜底 ---
            throw new NotSupportedException($"不支持的目标类型 {targetType.FullName}");
        }

        /// <summary>
        /// 转换js参数为c#对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected object? ConvertJavaScriptValueToCsharp(JavaScriptValue value)
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
                        result[kv.Key] = ConvertJavaScriptValueToCsharp(kv.Value);
                    }
                    return result;
                case JavaScriptValueType.Array:
                    var array = value.ToArray();
                    var list = new List<object?>();
                    foreach (var item in array)
                    {
                        list.Add(ConvertJavaScriptValueToCsharp(item));
                    }
                    return list;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected object? GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Nullable.GetUnderlyingType(type) != null ? null : Activator.CreateInstance(type);

            return null;
        }

        /// <summary>
        /// 转换C#对象为js对象
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected JavaScriptValue ConvertCsharpToJavaScriptValue(object? result)
        {
            if (result == null)
                return new JavaScriptValue();

            var type = result.GetType();

            if (result is Guid guid)
                return new JavaScriptValue(guid.ToString());

            if (result is TimeSpan ts)
                return new JavaScriptValue(ts.ToString());

            if (result is string str)
                return new JavaScriptValue(str);

            if (result is bool b)
                return new JavaScriptValue(b);

            if (result is int i)
                return new JavaScriptValue(i);

            // js 不支持 long，转 string
            if (result is long l)
                return new JavaScriptValue(l.ToString());

            if (result is float f)
                return new JavaScriptValue(f.ToString("0.00"));

            if (result is double d)
                return new JavaScriptValue(d.ToString("0.00"));

            if (result is decimal dec)
                return new JavaScriptValue(dec.ToString("0.00"));

            if (result is DateTime dt)
                return new JavaScriptValue(dt.ToString("yyyy-MM-dd HH:mm:ss"));

            if (type.IsEnum)
                return new JavaScriptValue((int)result);

            // Nullable<T> 递归处理
            var underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
                return ConvertCsharpToJavaScriptValue(Convert.ChangeType(result, underlying));

            // Dictionary 处理
            if (result is IDictionary dict)
            {
                var jsObj = new JavaScriptObject();

                foreach (DictionaryEntry entry in dict)
                {
                    var key = entry.Key?.ToString() ?? "";
                    jsObj.Add(key.ToCamelCase(), ConvertCsharpToJavaScriptValue(entry.Value));
                }

                return jsObj;
            }
            else if (result is IEnumerable enumerable && type != typeof(string))
            {
                var jsArray = new JavaScriptArray();

                foreach (var item in enumerable)
                    jsArray.Add(ConvertCsharpToJavaScriptValue(item));

                return jsArray;
            }
            else
            {
                // 自定义对象：反射属性
                var jsObj = new JavaScriptObject();
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in props)
                {
                    if (!prop.CanRead)
                        continue;

                    var name = prop.Name.ToCamelCase();
                    var value = prop.GetValue(result);
                    jsObj.Add(name, ConvertCsharpToJavaScriptValue(value));
                }

                return jsObj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        protected object[] ConvertArgs(JavaScriptArray args, MethodInfo method)
        {
            var parameters = method.GetParameters();

            var result = new object?[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var jsVal = args[i];
                var paramType = parameters[i].ParameterType;
                result[i] = ConvertToParamer(jsVal, paramType);
            }

            return result!;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected void HandleException(Exception ex)
        {
            if (ex is FriendlyException friendly)
            {
                FormAppContext.Bridge.Toast.Warn(friendly.Message);
            }
            else if (ex is DataNullableException)
            {
                FormAppContext.Bridge.Toast.Warn("数据不存在，请重新查询数据重试");
            }
            else
            {
                FormAppContext.Bridge.Toast.Error("程序服务异常");
                FormAppContext.UseLogger(GetType()).Error($"api invoke failed:{ex.GetType().FullName}", ex);
            }
        }
    }
}

