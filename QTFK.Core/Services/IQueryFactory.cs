﻿using QTFK.Models;
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
        IDBQuerySelect NewSelect();
        IDBQueryInsert NewInsert();
        IDBQueryUpdate NewUpdate();
        IDBQueryDelete NewDelete();
        string Prefix { get; set; }
    }
}
