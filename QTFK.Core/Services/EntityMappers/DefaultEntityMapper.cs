using System;
using QTFK.Models;
using System.Collections.Generic;

namespace QTFK.Services.EntityMappers
{
    public class DefaultEntityMapper : IEntityMapper
    {
        private readonly IDictionary<KeyValuePair<Type, Type>, object> _entityMaps;

        public DefaultEntityMapper()
        {
            _entityMaps = new Dictionary<KeyValuePair<Type, Type>, object>();
        }

        public TTarget Map<TSource, TTarget>(TSource row)
        {
            Func<TSource, TTarget> mapper = (Func<TSource, TTarget>)_entityMaps[BuildKey<TSource,TTarget>()];
            return mapper(row);
        }

        KeyValuePair<Type, Type> BuildKey<TSource,TTarget>()
        {
            return new KeyValuePair<Type, Type>(typeof(TSource), typeof(TTarget));
        }

        public void Register<TSource, TTarget>(Func<TSource, TTarget> mapper)
        {
            _entityMaps[BuildKey<TSource,TTarget>()] = mapper;
        }
    }
}