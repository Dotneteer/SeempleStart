﻿using System;
using Seemplest.Core.DataAccess.Attributes;
using Seemplest.Core.DataAccess.DataRecords;

namespace Younderwater.Services.DiveLog.DataAccess
{
    [TableName("DiveLog")]
    [SchemaName("Dive")]
    public class DiveLogRecord: DataRecord<DiveLogRecord>
    {
        private int _id;
        private string _userId;
        private DateTime _date;
        private string _diveSite;
        private string _location;
        private decimal _maxDepth;
        private int _bottomTime;
        private string _comment;

        [PrimaryKey]
        [AutoGenerated]
        public int Id
        {
            get { return _id; }
            set { _id = Modify(value, "Id"); }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = Modify(value, "UserId"); }
        }

        public DateTime Date
        {
            get { return _date; }
            set { _date = Modify(value, "Date"); }
        }

        public string DiveSite
        {
            get { return _diveSite; }
            set { _diveSite = Modify(value, "DiveSite"); }
        }

        public string Location
        {
            get { return _location; }
            set { _location = Modify(value, "Location"); }
        }

        public decimal MaxDepth
        {
            get { return _maxDepth; }
            set { _maxDepth = Modify(value, "MaxDepth"); }
        }

        public int BottomTime
        {
            get { return _bottomTime; }
            set { _bottomTime = Modify(value, "BottomTime"); }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = Modify(value, "Comment"); }
        }
    }
}