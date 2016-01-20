using System;

namespace ID3SQL
{
    public class ExecutionPlanOptions
    {
        public bool DryRun { get; set; }
        public bool Recycle { get; set; }
        public bool Verbose { get; set; }
        public char StringArraySeparator { get; set; }
        public bool RegexIgnoreCase { get; set; }
        public bool ColumnNames { get; set; }
        public string ColumnSeparator { get; set; }
    }
}
