namespace Forms.App.Main.JsObject.Builder
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class JavaScriptRegisterAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public JavaScriptRegisterAttribute(string name)
        {
            Name = name;
        }
    }
}

