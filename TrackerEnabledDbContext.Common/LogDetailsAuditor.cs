namespace TrackerEnabledDbContext.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    using TrackerEnabledDbContext.Common.Extensions;
    using TrackerEnabledDbContext.Common.Models;

    public class LogDetailsAuditor : IDisposable
    {
        readonly DbEntityEntry _dbEntry;

        readonly AuditLog _log;

        public LogDetailsAuditor(DbEntityEntry dbEntry, AuditLog log)
        {
            _dbEntry = dbEntry;
            _log = log;
        }

        public IEnumerable<AuditLogDetail> CreateLogDetails()
        {
            var type = _dbEntry.Entity.GetType().GetEntityType();

            return from propertyName in this._dbEntry.OriginalValues.PropertyNames
                   where
                       type.IsTrackingEnabled(propertyName) &&
                       this.IsValueChanged(propertyName)
                   select new AuditLogDetail
                              {
                                  ColumnName = type.GetColumnName(propertyName),
                                  OrginalValue = this.OriginalValue(propertyName),
                                  NewValue = this.CurrentValue(propertyName),
                                  Log = this._log
                              };
        }

        bool IsValueChanged(string propertyName)
        {
            return !Equals(OriginalValue(propertyName), CurrentValue(propertyName));
        }

        string OriginalValue(string propertyName)
        {
            string originalValue;

            if (_dbEntry.State == EntityState.Unchanged)
            {
                originalValue = null;
            }
            else
            {
                var value = _dbEntry.GetDatabaseValue(propertyName);
                originalValue = (value != null) ? value.ToString() : null;
            }

            return originalValue;
        }

        string CurrentValue(string propertyName)
        {
            string newValue;

            try
            {
                var value = _dbEntry.GetCurrentValue(propertyName);
                newValue = (value != null) ? value.ToString() : null;
            }
            catch (InvalidOperationException)
            {
                // It will be invalid operation when its in deleted state. in that case, new value should be null
                newValue = null;
            }

            return newValue;
        }

        public void Dispose()
        {
        }
    }
}