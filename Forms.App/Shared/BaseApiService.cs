using Forms.App.Core.Logging;
using Forms.App.Model.Models.Api;
using Lycoris.Common.Extensions;
using System.Text;
using WinFormium.Sources.WebResource.Data.@base;

namespace Forms.App.Main.Shared
{
    public class BaseApiService : DataResourceService
    {
        /// <summary>
        /// 
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        public IServerLoggerFactory LoggerFactory { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        protected IServerLogger Logger { get => this.LoggerFactory.CreateLogger(this.GetType()); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected IResourceResult Success()
        {
            var resp = new BaseOutput()
            {
                Code = 0,
                Msg = null
            };

            return Content(resp.ToJson(), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IResourceResult Success<T>(DataOutput<T> data) => Content(data.ToJson(), "application/json", Encoding.UTF8);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IResourceResult Success<T>(ListOutput<T> data) => Content(data.ToJson(), "application/json", Encoding.UTF8);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IResourceResult Success<T>(PageOutput<T> data) => Content(data.ToJson(), "application/json", Encoding.UTF8);


    }
}
