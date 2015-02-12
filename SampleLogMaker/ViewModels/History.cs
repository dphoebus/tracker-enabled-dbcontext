namespace SampleLogMaker.ViewModels
{
    using System;
    using System.Collections.Generic;

    public class BaseHistoryVm
    {
        public int LogId { get; set; }

        public int RecordId { get; set; }

        public DateTime Date { get; set; }

        public string UserName { get; set; }

        public string TableName { get; set; }

        public IEnumerable<LogDetail> Details = new List<LogDetail>();
    }

    public class ChangedHistoryVm : BaseHistoryVm
    {
    }

    public class DeletedHistoryVm : BaseHistoryVm
    {
    }

    public class AddedHistoryVm : BaseHistoryVm
    {
    }
}