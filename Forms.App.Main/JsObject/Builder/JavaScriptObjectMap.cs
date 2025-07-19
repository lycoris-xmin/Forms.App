using WinFormium.Sources.JavaScript.JavaScriptEngine;

namespace Forms.App.Main.JsObject.Builder
{
    /// <summary>
    /// 
    /// </summary>
    internal class JavaScriptObjectMap
    {
        public string Name { get; set; } = default!;

        public string? Host { get; set; }

        public string? Path { get; set; }

        public Type Type { get; set; } = default!;

        public JavaScriptObject? Instance { get; set; }
    }
}

