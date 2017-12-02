using System.Collections.Generic;

namespace QTFK.Models
{
    public enum JoinKind
    {
        Inner,
        Left,
        Right,
    }

    public class JoinTable
    {
        public string Table { get; set; }
        public ICollection<JoinMatch> Matches { get; set; }
        public JoinKind Kind { get; set; }
        public ICollection<SelectColumn> Columns { get; set; }
    }

    public class JoinMatch
    {
        public string LeftField { get; set; }
        public string RightField { get; set; }
    }
}
