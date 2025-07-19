using Lycoris.Autofac.Extensions;
using Lycoris.Common.Helper;

namespace Forms.App.Model.Contexts
{
    [AutofacRegister(ServiceLifeTime.Scoped)]
    public class ClientContext
    {
        private string? _token;

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string Token
        {
            get
            {
                if (_token != null)
                    return _token;

                _token = ConfigHelper.GetValue("Token");
                _token ??= "";
                return _token;
            }
            set
            {
                ConfigHelper.SetValue("Token", value);
                _token = value;
            }
        }

        private string? _refershToken;
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefershToken
        {
            get
            {
                if (_refershToken != null)
                    return _refershToken;

                _refershToken = ConfigHelper.GetValue("RefershToken");
                _refershToken ??= "";
                return _refershToken;
            }
            set
            {
                ConfigHelper.SetValue("RefershToken", value);
                _refershToken = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UserContext User { get; set; } = default!;
    }
}
