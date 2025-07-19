namespace Forms.App.Model.Exceptions
{
    public class DataNullableException : Exception
    {
        public DataNullableException() : base("data nullable") { }

        public DataNullableException(string propName) : base($"{propName} data nullable") { }

        public DataNullableException(string propName, long id) : base($"{propName} data nullable with id :{id}") { }

        public DataNullableException(string propName, string id) : base($"{propName} data nullable with id :{id}") { }
    }
}
