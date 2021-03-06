﻿using QTFK.Models;
using System.Data.OleDb;
using QTFK.Attributes;

namespace QTFK.Services.DBIO
{
    [OleDB]
    public class OleDBIO : AbstractDBIO<OleDbConnection, OleDbCommand, OleDbDataAdapter>, IOleDBIO
    {
        public OleDBIO(string connectionString)
            : base(connectionString)
        {
        }

        protected override OleDbCommand prv_buildCommand(OleDbConnection connection)
        {
            return new OleDbCommand()
            {
                Connection = connection
            };
        }

        protected override OleDbConnection prv_buildConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }

        protected override OleDbDataAdapter prv_buildDataAdapter(string query, OleDbConnection connection)
        {
            return new OleDbDataAdapter(query, connection);
        }
    }
}
