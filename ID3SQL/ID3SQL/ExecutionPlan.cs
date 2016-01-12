using System;
using System.Collections.Generic;

using Microsoft.VisualBasic.FileIO;

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

            Action<File, string, ExecutionPlanOptions> actionFn = ToAction(rootNode);
            Func<File, string, ExecutionPlanOptions, bool> whereExecutionFn = ToWhereFunc(rootNode);

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

        private static Action<File, string, ExecutionPlanOptions> ToAction(ParseTreeNode rootNode)
        {
            Action<File, string, ExecutionPlanOptions> action;
            switch (rootNode.Term.Name)
            {
                case "selectStmt":
                    {
                        action = ToSelectAction();
                        break;
                    }
                case "updateStmt":
                    {
                        action = ToUpdateAction();
                        break;
                    }
                case "deleteStmt":
                    {
                        action = ToDeleteAction();
                        break;
                    }
                default:
                    {
                        throw new Exception("Unknown root term");
                    }
            }

            return action;
        }

        private static Func<File, string, ExecutionPlanOptions, bool> ToWhereFunc(ParseTreeNode rootNode)
        {
            ParseTreeNode whereNode;
            switch (rootNode.Term.Name)
            {
                case "selectStmt":
                    {
                        whereNode = rootNode.ChildNodes[2];
                        break;
                    }
                case "updateStmt":
                    {
                        whereNode = rootNode.ChildNodes[3];
                        break;
                    }
                case "deleteStmt":
                    {
                        whereNode = rootNode.ChildNodes[1];
                        break;
                    }
                default:
                    {
                        throw new Exception("Unknown root term");
                    }
            }

            Func<File, string, ExecutionPlanOptions, bool> whereExecutionFn;
            if (whereNode.ChildNodes.Count < 1)
            {
                whereExecutionFn = (file, filePath, executionPlanOptions) => true;
            }
            else
            {
                // TODO actual implementation
                whereExecutionFn = (file, filePath, executionPlanOptions) =>
                {
                    return true;
                };
            }

            return whereExecutionFn;
        }

        private static Action<File, string, ExecutionPlanOptions> ToSelectAction()
        {
            // TODO actual implementation
            Action<File, string, ExecutionPlanOptions> action = (file, filePath, executionPlanOptions) =>
            {

            };
            return action;
        }

        private static Action<File, string, ExecutionPlanOptions> ToUpdateAction()
        {
            // TODO actual implementation
            Action<File, string, ExecutionPlanOptions> action = (file, filePath, executionPlanOptions) =>
            {

            };
            return action;
        }

        private static Action<File, string, ExecutionPlanOptions> ToDeleteAction()
        {
            Action<File, string, ExecutionPlanOptions> action = (file, filePath, executionPlanOptions) =>
            {
                if(executionPlanOptions.Recycle)
                {
                    if(!executionPlanOptions.DryRun)
                    {
                        FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }

                    if(executionPlanOptions.Verbose)
                    {
                        Console.WriteLine(string.Format("Recycled {0}", filePath));
                    }
                }
                else
                {
                    if(!executionPlanOptions.DryRun)
                    {
                        System.IO.File.Delete(filePath);
                    }

                    if(executionPlanOptions.Verbose)
                    {
                        Console.WriteLine(string.Format("Deleted {0}", filePath));
                    }
                }
            };
            return action;
        }
    }
}
