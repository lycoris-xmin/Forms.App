namespace Forms.App.Model.Constants
{
    public class HttpHeaders
    {
        /// <summary>
        /// 请求域名
        /// </summary>
        public const string HOST = "Host";

        /// <summary>
        /// 
        /// </summary>
        public const string FORWARDED = "X-Forwarded-For";

        /// <summary>
        /// Nginx真实来源请求头
        /// </summary>
        public const string NGINX_REMOTE_IP = "X-Real-Ip";

        /// <summary>
        /// 
        /// </summary>
        public const string USER_AGENT = "User-Agent";

        /// <summary>
        /// 
        /// </summary>
        public const string REFERER = "Referer";

        /// <summary>
        /// 
        /// </summary>
        public const string TOKEN = "X-Authorize";

        /// <summary>
        /// 
        /// </summary>
        public const string FINGER_PRINT = "X-Fingerprint";
    }
}
