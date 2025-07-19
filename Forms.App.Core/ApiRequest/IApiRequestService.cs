using Forms.App.Core.ApiRequest.Modules.Authorize;
using Forms.App.Core.Logging;

namespace Forms.App.Core.ApiRequest
{
    public interface IApiRequestService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        void RegisterLogger(IServerLogger logger);

        /// <summary>
        /// 
        /// </summary>
        IAuthroizeRequestService Authorize { get; }
    }
}
