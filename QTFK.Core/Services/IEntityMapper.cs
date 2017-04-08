using QTFK.Models;
using QTFK.Services.EntityMappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IEntityMapper
    {
        TTarget Map<TSource,TTarget>(TSource row);
        void Register<TSource,TTarget>(Func<TSource, TTarget> mapper);
    }
}
