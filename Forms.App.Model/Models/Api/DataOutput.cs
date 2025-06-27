namespace Forms.App.Model.Models.Api
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataOutput<T> : BaseOutput
    {
        public T? Data { get; set; }
    }
}
