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
        protected readonly IParameterBuilderFactory parameterBuilderFactory;

        public AbstractQueryFactory(IEnumerable<Type> filterTypes, IParameterBuilderFactory parameterBuilderFactory)
        {
            this.queryFilterInterfaceTypeFullName = typeof(IQueryFilter).FullName;
            this.parameterBuilderFactory = parameterBuilderFactory;
            this.filterTypes = filterTypes.ToList();

            foreach (Type t in this.filterTypes)
            {
                Asserts.isNotNull(t);
                Asserts.isFalse(t.IsInterface);
                Asserts.isNotNull(t.GetInterface(this.queryFilterInterfaceTypeFullName));
            }
            Asserts.isNotNull(parameterBuilderFactory);
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

        public IQueryFilter buildFilter(Type interfaceType)
        {
            Type[] filterTypes;
            Type filterType;
            IQueryFilter instance;
            //ConstructorInfo[] ctors;
            //ConstructorInfo paramBuilderCtor;

            Asserts.isTrue(interfaceType.IsInterface);
            Asserts.isNotNull(interfaceType.GetInterface(this.queryFilterInterfaceTypeFullName));

            filterTypes = this.filterTypes
                .Where(concreteFiltertype => concreteFiltertype.implementsInterface(interfaceType))
                .ToArray()
                ;

            Asserts.isTrue(filterTypes.Length == 1);

            filterType = filterTypes[0];
            //ctors = filterType.GetConstructors();
            //paramBuilderCtor = ctors
            //    .FirstOrDefault(c =>
            //    {
            //        ParameterInfo[] args;

            //        args = c.GetParameters();

            //        return args.Length == 1
            //            && args[0].ParameterType == typeof(IParameterBuilderFactory);
            //    });

            //if (paramBuilderCtor != null)
            //    instance = (IQueryFilter)paramBuilderCtor.Invoke(new object[] { this.parameterBuilderFactory });
            //else
                instance = (IQueryFilter)Activator.CreateInstance(filterType);

            return instance;
        }

        private static T prv_newQuery<T>(string prefix, Func<T> builder) where T: IDBQuery
        {
            T query;

            query = builder();

            if (!string.IsNullOrWhiteSpace(prefix))
                query.Prefix = prefix;

            return query;
        }
    }
}
