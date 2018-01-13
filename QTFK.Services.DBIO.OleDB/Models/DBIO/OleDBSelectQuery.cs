﻿using QTFK.Attributes;
using QTFK.Services;

namespace QTFK.Models.DBIO
{
    [OleDB]
    internal class OleDBSelectQuery : AbstractSelectQuery
    {
        public OleDBSelectQuery(IParameterBuilderFactory parameterBuilderFactory) : base(parameterBuilderFactory)
        {

        }
    }
}