using QTFK.Models;
using QTFK.Services;
using QTFK.Services.EntityMappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Mapping
{
    public static class EntityMapperExtension
    {
        private static IEntityMapper _mapper;

        public static IEntityMapper Mapper
        {
            get
            {
                if (_mapper == null)
                    _mapper = NullEntityMapper.Instance;
                return _mapper;
            }
            set { _mapper = value; }
        }

        public static IEnumerable<TTarget> Map<TSource, TTarget>(this IEnumerable<TSource> items)
        {
            foreach (TSource item in items)
            {
                yield return Map<TSource, TTarget>(item);
            }
        }

        public static TTarger Map<TSource, TTarger>(this TSource item)
        {
            return Mapper.Map<TSource, TTarger>(item);
        }
    }
}
