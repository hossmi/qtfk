using System;
using QTFK.Models;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.Types;

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

        protected virtual string prv_buildParameter(string name)
        {
            string result;
            string parameter;
            char[] validChars;

            validChars = name
                .Where(c => c > 32)
                .ToArray();

            parameter = new string(validChars);
            result = $"@{parameter}";

            return result;
        }

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

        public IQueryFilter buildFilter(Type interfaceType)
        {
            Type[] filterTypes;
            Type filterType;
            object instance;

            Asserts.check(interfaceType.IsInterface, $"Type '{interfaceType.FullName}' is not an interface.");
            Asserts.check(interfaceType.GetInterface(this.queryFilterInterfaceTypeFullName) != null, $"Type '{interfaceType.FullName}' does not inherits from '{this.queryFilterInterfaceTypeFullName}'.");

            filterTypes = this.filterTypes
                .Where(concreteFiltertype => concreteFiltertype.implementsInterface(interfaceType))
                .ToArray()
                ;

            Asserts.check(filterTypes.Length == 1, $"There is zero or more than one type implementing {interfaceType.FullName}");

            filterType = filterTypes[0];
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
