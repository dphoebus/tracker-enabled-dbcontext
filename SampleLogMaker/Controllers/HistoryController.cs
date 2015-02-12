﻿namespace SampleLogMaker.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using SampleLogMaker.Models;
    using SampleLogMaker.ViewModels;

    using TrackerEnabledDbContext.Common.Models;

    [Authorize]
    public class HistoryController : Controller
    {
        //
        // GET: /History/
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            var data = db.AuditLog
                         .OrderByDescending(x => x.EventDateUTC)
                         .ToList();
            var vm = new List<BaseHistoryVm>();

            foreach (var log in data)
            {
                switch (log.EventType)
                {
                    case EventType.Added: // added
                        vm.Add(new AddedHistoryVm
                                   {
                                       Date = log.EventDateUTC.ToLocalTime().DateTime,
                                       LogId = log.AuditLogId,
                                       RecordId = int.Parse(log.RecordId),
                                       TableName = log.TableName,
                                       UserName = log.UserName,
                                       Details = log.LogDetails.Select(x => new LogDetail { PropertyName = x.ColumnName, NewValue = x.NewValue })
                                   });
                        break;

                    case EventType.Deleted: //deleted
                        vm.Add(new DeletedHistoryVm
                                   {
                                       Date = log.EventDateUTC.ToLocalTime().DateTime,
                                       LogId = log.AuditLogId,
                                       RecordId = int.Parse(log.RecordId),
                                       TableName = log.TableName,
                                       UserName = log.UserName,
                                       Details = log.LogDetails.Select(x => new LogDetail { PropertyName = x.ColumnName, OldValue = x.OrginalValue })
                                   });
                        break;

                    case EventType.Modified: //modified
                        vm.Add(new ChangedHistoryVm
                                   {
                                       Date = log.EventDateUTC.ToLocalTime().DateTime,
                                       LogId = log.AuditLogId,
                                       RecordId = int.Parse(log.RecordId),
                                       TableName = log.TableName,
                                       UserName = log.UserName,
                                       Details = log.LogDetails.Select(x => new LogDetail { PropertyName = x.ColumnName, NewValue = x.NewValue, OldValue = x.OrginalValue })
                                   });
                        break;
                }
            }

            return View(vm);
        }
    }
}