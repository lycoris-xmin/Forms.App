namespace Forms.App.Model.Models.Api
{
    public class ListOutput<T> : BaseOutput
    {
        /// <summary>
        /// 
        /// </summary>
        public List<T>? List { get; set; }
    }
}
