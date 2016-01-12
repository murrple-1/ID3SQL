using System;
using System.Collections.Generic;

using Irony.Parsing;

using TagLib;

namespace ID3SQL
{
    public static class ExecutionPlan
    {
        public static Action<IEnumerable<string>, ExecutionPlanOptions> GenerateExecutionPlan(string statement)
        {
            ParseTree parseTree = ID3SQLGrammar.GenerateParseTree(statement);
            ParseTreeNode rootNode = parseTree.Root;
            if (rootNode == null)
            {
                throw new ID3SQLException("Unable to parse statement");
            }

            // TODO actual function
            Func<File, string, ExecutionPlanOptions, bool> whereExecutionFn = (file, filePath, executionPlanOptions) =>
            {
                return true;
            };

            // TODO actual action
            Action<File, string, ExecutionPlanOptions> actionFn = (file, filePath, executionPlanOptions) =>
            {
                Console.WriteLine(filePath);
            };

            return (filePaths, executionPlanOptions) => {
                foreach(string filePath in filePaths)
                {
                    using (File tagFile = File.Create(filePath))
                    {
                        if(whereExecutionFn(tagFile, filePath, executionPlanOptions))
                        {
                            actionFn(tagFile, filePath, executionPlanOptions);
                        }
                    }
                }
            };
        }
    }
}
