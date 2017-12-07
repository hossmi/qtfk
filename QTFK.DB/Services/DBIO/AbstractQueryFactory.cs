using System;
using QTFK.Models;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.DBIO
{
    public abstract class AbstractQueryFactory : IQueryFactory
    {
        protected readonly List<Type> filterTypes;
        protected readonly string queryFilterInterfaceTypeFullName;

        public AbstractQueryFactory(IEnumerable<Type> filterTypes)
        {
            this.queryFilterInterfaceTypeFullName = typeof(IQueryFilter).FullName;
            this.filterTypes = filterTypes.ToList();

            foreach (Type t in this.filterTypes)
            {
                Asserts.isSomething(t, $"Type element at parameter 'filterTypes' is null!");
                Asserts.check(t.IsInterface == false, $"Type '{t.FullName}' at parameter 'filterTypes' cannot be interface.");
                Asserts.check(t.GetInterface(this.queryFilterInterfaceTypeFullName) != null, $"Type '{t.FullName}' at parameter 'filterTypes' does not implements '{this.queryFilterInterfaceTypeFullName}'.");
            }
        }

        public string Prefix { get; set; }

        protected abstract IDBQueryDelete prv_newDelete();
        protected abstract IDBQueryInsert prv_newInsert();
        protected abstract IDBQuerySelect prv_newSelect();
        protected abstract IDBQueryUpdate prv_newUpdate();

        public IDBQueryDelete newDelete()
        {
            return prv_newQuery(this.Prefix, prv_newDelete);
        }

        public IDBQueryInsert newInsert()
        {
            return prv_newQuery(this.Prefix, prv_newInsert);
        }

        public IDBQuerySelect newSelect()
        {
            return prv_newQuery(this.Prefix, prv_newSelect);
        }

        public IDBQueryUpdate newUpdate()
        {
            return prv_newQuery(this.Prefix, prv_newUpdate);
        }

        public IQueryFilter buildFilter(Type type)
        {
            Type filterType;
            object instance;

            Asserts.check(type.IsInterface, $"Type '{type.FullName}' is not an interface.");
            Asserts.check(type.GetInterface(this.queryFilterInterfaceTypeFullName) != null, $"Type '{type.FullName}' does not inherits from '{this.queryFilterInterfaceTypeFullName}'.");

            filterType = this.filterTypes
                .Single(t => t.IsAssignableFrom(type));

            instance = Activator.CreateInstance(filterType);
            return (IQueryFilter)instance;
        }

        private static T prv_newQuery<T>(string prefix, Func<T> builder) where T: IDBQuery
        {
            T query;

            query = builder();
            query.Prefix = prefix;

            return query;
        }
    }
}
