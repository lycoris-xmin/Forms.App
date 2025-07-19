using Forms.App.Model.Bridges.Modules;

namespace Forms.App.Model.Bridges
{
    public interface IFormiumBridge
    {
        /// <summary>
        /// 
        /// </summary>
        IFormiumPageBridge Page { get; }

        /// <summary>
        /// 
        /// </summary>
        IFormiumJavascriptBridge JavaScript { get; }

        /// <summary>
        /// 
        /// </summary>
        IFormiumToastBridge Toast { get; }

        /// <summary>
        /// 
        /// </summary>
        IFormiumNotifyBridge Notify { get; }

        /// <summary>
        /// 
        /// </summary>
        IFormiumEventBridge Event { get; }
    }
}
