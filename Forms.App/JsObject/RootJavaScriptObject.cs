namespace Forms.App.Main.JsObject
{
    internal class RootJavaScriptObject : JavaScriptObjectBuilder
    {
        internal override string JsObjectName => "root";

        protected override void Initialize()
        {
            this.AddReadOnlyProperty("Demo", () => DateTime.Now);

            this.AddMethod("TestMethod", () =>
            {
                Console.WriteLine("1");
            });
        }
    }
}
