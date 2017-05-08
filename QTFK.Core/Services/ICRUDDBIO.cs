using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface ICRUDDBIO : IDBIO
    {
        IDBQuerySelect NewSelect();
        IDBQueryInsert NewInsert();
        IDBQueryUpdate NewUpdate();
        IDBQueryDelete NewDelete();
    }
}
