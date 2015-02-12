namespace TrackerEnabledDbContext.Common.Extensions
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;

    using TrackerEnabledDbContext.Common.Interfaces;

    public static class TypeExtensions
    {
        static readonly ConcurrentDictionary<string, bool> IsTrackingEnabledCache = new ConcurrentDictionary<string, bool>();

        static readonly ConcurrentDictionary<string, string> PrimaryKeyNameCache = new ConcurrentDictionary<string, string>();

        static readonly ConcurrentDictionary<string, string> TableNameCache = new ConcurrentDictionary<string, string>();

        static readonly ConcurrentDictionary<string, string> ColumnNameCache = new ConcurrentDictionary<string, string>();

        const string ProxyNamespace = @"System.Data.Entity.DynamicProxies";

        public static Type GetEntityType(this Type entityType)
        {
            while (true)
            {
                if (entityType == null || entityType.Namespace != ProxyNamespace)
                {
                    return entityType;
                }

                entityType = entityType.BaseType;
            }
        }

        public static string GetPrimaryKeyName(this Type type)
        {
            var key = type.FullName;

            return GetFromCache(PrimaryKeyNameCache,
                                key,
                                () => {
                                    try
                                    {
                                        var entityType = type.GetEntityType();
                                        return entityType.GetProperties()
                                                         .Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
                                                         .Name;
                                    }
                                    catch (Exception)
                                    {
                                        throw new KeyNotFoundException(string.Format(@"A single primary key attribute is reqiured per entity for tracker to work. The entity '{0}' does not contain a primary key attribute or contains more than one.", type.Name));
                                    }
                                });
        }

        public static bool IsTrackingEnabled(this Type type)
        {
            var key = string.Format("table__{0}", type.FullName);

            return GetFromCache(IsTrackingEnabledCache,
                                key,
                                () => {
                                    var entityType = type.GetEntityType();
                                    var trackChangesAttribute = entityType.GetCustomAttributes(false)
                                                                          .OfType<TrackChangesAttribute>()
                                                                          .SingleOrDefault();

                                    return trackChangesAttribute != null && trackChangesAttribute.Enabled;
                                });
        }

        public static bool IsTrackingEnabled(this Type type, string propertyName)
        {
            var key = string.Format("column__{0}_{1}", type.FullName, propertyName);

            return GetFromCache(IsTrackingEnabledCache,
                                key,
                                () => {
                                    var entityType = type.GetEntityType();
                                    var skipTrackingAttribute = entityType.GetProperty(propertyName)
                                                                          .GetCustomAttributes(false)
                                                                          .OfType<SkipTrackingAttribute>()
                                                                          .SingleOrDefault();

                                    return skipTrackingAttribute == null || !skipTrackingAttribute.Enabled;
                                });
        }

        public static string GetTableName(this Type entityType, ITrackerContext context)
        {
            var key = entityType.FullName;

            return GetFromCache(TableNameCache,
                                key,
                                () => {
                                    var tableAttribute = entityType.GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
                                    var dbsetPropertyName = context.GetType()
                                                                   .GetProperties()
                                                                   .Single(x => x.PropertyType.GenericTypeArguments.Any(y => y == entityType))
                                                                   .Name;

                                    // Get table name (if it has a Table attribute, use that, otherwise dbset property name)
                                    return (tableAttribute != null) ? tableAttribute.Name : dbsetPropertyName;
                                });
        }

        public static string GetColumnName(this Type type, string propertyName)
        {
            var key = string.Format("{0}_{1}", type.FullName, propertyName);

            return GetFromCache(ColumnNameCache,
                                key,
                                () => {
                                    var columnName = propertyName;

                                    var entityType = type.GetEntityType();
                                    var columnAttribute = entityType.GetProperty(propertyName).GetCustomAttribute<ColumnAttribute>(false);
                                    if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
                                    {
                                        columnName = columnAttribute.Name;
                                    }

                                    return columnName;
                                });
        }

        static TValue GetFromCache<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dictionary,
                                                 TKey key,
                                                 Func<TValue> func)
        {
            lock (dictionary)
            {
                if (!dictionary.ContainsKey(key))
                {
                    dictionary[key] = func();
                }

                return dictionary[key];
            }
        }
    }
}