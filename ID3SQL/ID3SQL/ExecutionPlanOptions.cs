﻿using System;

namespace ID3SQL
{
    public class ExecutionPlanOptions
    {
        public bool DryRun { get; set; } = false;
        public bool Recycle { get; set; } = false;
        public bool Verbose { get; set; } = false;
        public char StringArraySeparator { get; set; } = ';';
        public bool RegexIgnoreCase { get; set; } = true;
    }
}
