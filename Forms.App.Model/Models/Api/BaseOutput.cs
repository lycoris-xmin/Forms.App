namespace Forms.App.Model.Models.Api
{
    public class BaseOutput
    {
        public int Code { get; set; }

        public string? Msg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsFriendlyCode() => this.Code == -99;
    }
}
