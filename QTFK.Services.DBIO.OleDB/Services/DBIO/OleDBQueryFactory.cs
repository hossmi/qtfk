using System;
using QTFK.Models;
using QTFK.Models.DBIO;
using QTFK.Models.DBIO.Filters;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.DBIO
{
    public class OleDBQueryFactory : IOleDB, IQueryFactory
    {
        private readonly List<Type> filterTypes;
        private readonly string queryFilterInterfaceTypeFullName;

        public static OleDBQueryFactory buildDefault()
        {
            return new OleDBQueryFactory(new Type[]
            {
                typeof(OleDBByParamEqualsFilter),
            });
        }

        public OleDBQueryFactory(IEnumerable<Type> filterTypes)
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

        public IDBQueryDelete newDelete()
        {
            return new OleDBDeleteQuery() { Prefix = this.Prefix };
        }

        public IDBQueryInsert newInsert()
        {
            return new OleDBInsertQuery() { Prefix = this.Prefix };
        }

        public IDBQuerySelect newSelect()
        {
            return new OleDBSelectQuery() { Prefix = this.Prefix };
        }

        public IDBQueryUpdate newUpdate()
        {
            return new OleDBUpdateQuery() { Prefix = this.Prefix };
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
    }
}
