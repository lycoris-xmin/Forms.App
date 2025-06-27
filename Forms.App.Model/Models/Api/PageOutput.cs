namespace Forms.App.Model.Models.Api
{
    public class PageOutput<T> : BaseOutput
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public List<T> List { get; set; } = new List<T>();
    }
}
