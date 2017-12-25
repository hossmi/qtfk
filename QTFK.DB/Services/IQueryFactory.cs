using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public delegate string BuildParameterDelegate(string parameterName);

    public interface IQueryFactory 
    {
        string Prefix { get; set; }

        IDBQuerySelect newSelect();
        IDBQueryInsert newInsert();
        IDBQueryUpdate newUpdate();
        IDBQueryDelete newDelete();
        IQueryFilter buildFilter(Type type);
    }
}
