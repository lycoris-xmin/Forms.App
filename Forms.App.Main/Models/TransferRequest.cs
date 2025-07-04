using Lycoris.Common.Extensions;
using Newtonsoft.Json;

namespace Forms.App.Main.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TransferRequest
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string> QueryString { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, string> FormData { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, (string FileName, byte[] RawDta)> Files { get; private set; } = new Dictionary<string, (string FileName, byte[] RawDta)>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TransferRequest AddQueryParameter(string name, string? value)
        {
            if (value.IsNullOrEmpty())
                return this;

            this.QueryString.TryAdd(name, value!);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TransferRequest AddParameter(string name, string? value)
        {
            if (value.IsNullOrEmpty())
                return this;

            this.FormData.TryAdd(name, value!);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fileName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public TransferRequest AddFile(string name, string fileName, byte[]? data)
        {
            if (data == null || data.Length == 0)
                return this;

            this.Files.Add(name, (fileName, data));
            return this;
        }
    }
}
