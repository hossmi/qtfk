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
                    _mapper = new DefaultEntityMapper();
                return _mapper;
            }
            set { _mapper = value; }
        }

        public static IEnumerable<TTarget> Map<TSource, TTarget>(this IEnumerable<TSource> items)
        {
            foreach (TSource item in items)
                yield return Mapper.Map<TSource, TTarget>(item);
        }
    }
}
