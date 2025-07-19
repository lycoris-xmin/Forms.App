namespace Forms.App.Model.Exceptions
{
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException() : base("token is expired") { }

        public TokenExpiredException(string message) : base(message) { }
    }
}
