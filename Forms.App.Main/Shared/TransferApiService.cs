using Forms.App.Core.Logging;
using Forms.App.Main.Models;
using Lycoris.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using System.Net;
using WinFormium.Sources.WebResource.Data.@base;

namespace Forms.App.Main.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public class TransferApiService : DataResourceService
    {
        protected readonly IServiceProvider ServiceProvider;
        protected readonly IServerLogger Logger;

        public TransferApiService(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;

            var factory = serviceProvider.GetRequiredService<IServerLoggerFactory>();
            this.Logger = factory.CreateLogger(this.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual string BaseTransferUrl { get; set; } = default!;

        /// <summary>
        /// 
        /// </summary>
        protected virtual string TransferUrl { get => $"{this.BaseTransferUrl.TrimEnd('/')}/{this.Context.Request.Uri.AbsolutePath.TrimStart('/')}"; }

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, string> RequestHeaders
        {
            get
            {
                if (Request.Headers == null || !Request.Headers.HasKeys())
                    return new Dictionary<string, string>();

                var dic = new Dictionary<string, string>();

                for (int i = 0; i < Request.Headers.Keys.Count; i++)
                    dic.Add(Request.Headers.Keys[i]!, Request.Headers[Request.Headers.Keys[i]]!);

                return dic;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected async Task<ResourceResult> TransferRequestAsync()
        {
            try
            {
                var client = new RestClient(this.BaseTransferUrl);

                var request = new RestRequest(this.Context.Request.Uri.AbsolutePath, Method.Get);

                if (this.RequestHeaders.HasValue())
                {
                    foreach (var item in this.RequestHeaders)
                        request.AddHeader(item.Key, item.Value);
                }

                if (!this.Request.ContentType.IsNullOrEmpty())
                    request.AddOrUpdateHeader("Content-Type", this.Request.ContentType);

                if (this.Request.QueryString != null && this.Request.QueryString.HasKeys())
                {
                    for (int i = 0; i < this.Request.QueryString.Keys.Count; i++)
                        request.AddQueryParameter(this.Request.QueryString.Keys[i]!, this.Request.QueryString[this.Request.QueryString.Keys[i]]!);
                }

                if (this.Request.FormData != null && this.Request.FormData.HasKeys())
                {
                    for (int i = 0; i < this.Request.FormData.Keys.Count; i++)
                        request.AddOrUpdateParameter(this.Request.FormData.Keys[i]!, this.Request.FormData[this.Request.FormData.Keys[i]]!);
                }
                else
                {
                    if (this.Request.JsonData != null)
                        request.AddJsonBody(this.Request.JsonData);

                    this.Logger.Info($"transfer request -> {this.TransferUrl} - {this.Request.JsonData ?? "{}"}");
                }

                var response = await client.ExecuteAsync(request);

                this.Logger.Info($"transfer response -> {response?.Content ?? ""} - {response?.StatusCode ?? 0}");

                if (response == null)
                    return StatusCode(HttpStatusCode.InternalServerError);

                if (!response.ErrorMessage.IsNullOrEmpty())
                    this.Logger.Error($"transfer response exception -> {response.ErrorException}", response.ErrorException);

                if (response.Headers.HasValue())
                {
                    foreach (var item in response.Headers!)
                        this.Context.Response.Headers.Add(item.Name, item.Value);
                }

                if (!response.IsSuccessStatusCode)
                    return StatusCode(response.StatusCode);

                return Content(response.Content ?? "", response.ContentType ?? "application/json");
            }
            catch (Exception ex)
            {
                this.Logger.Error($"transfer request error -> {this.TransferUrl}", ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected async Task<ResourceResult> TransferRequestAsync(TransferRequest input)
        {
            try
            {
                var client = new RestClient(this.BaseTransferUrl);

                var request = new RestRequest(this.Context.Request.Uri.AbsolutePath, Method.Get)
                {
                    Timeout = TimeSpan.FromSeconds(180)
                };

                if (this.RequestHeaders.HasValue())
                {
                    foreach (var item in this.RequestHeaders)
                        request.AddHeader(item.Key, item.Value);
                }

                if (!this.Request.ContentType.IsNullOrEmpty())
                    request.AddOrUpdateHeader("Content-Type", this.Request.ContentType);

                if (input.QueryString.HasValue())
                {
                    foreach (var item in input.QueryString)
                        request.AddQueryParameter(item.Key, item.Value);
                }

                if (input.FormData.HasValue())
                {
                    foreach (var item in input.FormData)
                        request.AddQueryParameter(item.Key, item.Value);

                    if (input.Files.HasValue())
                    {
                        foreach (var item in input.Files)
                        {
                            request.AddFile(item.Key, item.Value.RawDta, item.Value.FileName);
                            input.AddParameter(item.Key, $"[{item.Value.FileName}]");
                        }
                    }

                    this.Logger.Info($"transfer request -> {this.TransferUrl} - {input.FormData.ToJson()}");
                }
                else
                {
                    var json = input.ToJson();

                    json = json.Equals("{}") ? "" : json;

                    if (!json.IsNullOrEmpty())
                        request.AddJsonBody(json);

                    this.Logger.Info($"transfer request -> {this.TransferUrl} - {json}");
                }

                var response = await client.ExecuteAsync(request);

                this.Logger.Info($"transfer response -> {response?.Content ?? ""} - {response?.StatusCode ?? 0}");

                if (response == null)
                    return StatusCode(HttpStatusCode.InternalServerError);

                if (!response.ErrorMessage.IsNullOrEmpty())
                    this.Logger.Error($"transfer response exception -> {response.ErrorException}", response.ErrorException);

                if (response.Headers.HasValue())
                {
                    foreach (var item in response.Headers!)
                        this.Context.Response.Headers.Add(item.Name, item.Value);
                }

                if (!response.IsSuccessStatusCode)
                    return StatusCode(response.StatusCode);

                return Content(response.Content ?? "", response.ContentType ?? "application/json");
            }
            catch (Exception ex)
            {
                this.Logger.Error($"transfer request error -> {this.TransferUrl}", ex);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
    }
}
