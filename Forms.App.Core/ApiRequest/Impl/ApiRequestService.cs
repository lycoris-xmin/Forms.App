using Forms.App.Core.ApiRequest.Modules.Authorize;
using Forms.App.Core.ApiRequest.Modules.Authorize.Impl;
using Forms.App.Core.Logging;
using Lycoris.Autofac.Extensions;

namespace Forms.App.Core.ApiRequest.Impl
{
    [AutofacRegister(ServiceLifeTime.Transient)]
    public class ApiRequestService : IApiRequestService
    {
        private readonly ITransferRequestService _request;

        public ApiRequestService(ITransferRequestService request)
        {
            _request = request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public void RegisterLogger(IServerLogger logger) => _request.RegisterLogger(logger);

        private IAuthroizeRequestService? _authorize;
        public IAuthroizeRequestService Authorize
        {
            get
            {
                _authorize ??= new AuthroizeRequestService(_request);
                return _authorize;
            }
        }
    }
}
