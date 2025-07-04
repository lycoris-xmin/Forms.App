using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace Forms.App.Main.Models
{
    public class TransferResponse
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public CookieCollection? Cookies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<HeaderParameter>? Headers { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public IReadOnlyCollection<HeaderParameter>? ContentHeaders { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string? ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public ResponseStatus ResponseStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public string? ErrorMessage { get; set; }
    }
}
