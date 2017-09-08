using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IQueryFactory 
    {
        IDBIO DB { get; }
        string Prefix { get; set; }
    }

    public interface ISelectQueryFactory : IQueryFactory
    {
        IDBQuerySelect NewSelect();
    }
    public interface IInsertQueryFactory : IQueryFactory
    {
        IDBQueryInsert NewInsert();
    }
    public interface IUpdateQueryFactory : IQueryFactory
    {
        IDBQueryUpdate NewUpdate();
    }
    public interface IDeleteQueryFactory : IQueryFactory
    {
        IDBQueryDelete NewDelete();
    }

    public interface IByParamEqualsFilterFactory : IQueryFactory
    {
        IByParamEqualsFilter NewByParamEqualsFilter();
    }

    public interface IByParamBetweenFilterFactory : IQueryFactory
    {
        IByParamBetweenFilter<T> NewByParamBetweenFilter<T>() where T : struct;
    }

}
