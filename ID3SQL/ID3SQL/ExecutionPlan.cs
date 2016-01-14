using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

#if DEBUG
            PrintParseTree(rootNode, 0);
#endif

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

#if DEBUG
        private static void PrintParseTree(ParseTreeNode parseTreeNode, int level)
        {
            for(int i = 0; i < level; i++)
            {
                Console.Write('\t');
            }
            Console.WriteLine(parseTreeNode);

            foreach(ParseTreeNode childNode in parseTreeNode.ChildNodes)
            {
                PrintParseTree(childNode, level + 1);
            }
        }
#endif

        private static Action<File, string, ExecutionPlanOptions> ToAction(ParseTreeNode rootNode)
        {
            Action<File, string, ExecutionPlanOptions> action;
            switch (rootNode.Term.Name)
            {
                case ID3SQLGrammar.SelectStatementNonTermName:
                    {
                        action = ToSelectAction();
                        break;
                    }
                case ID3SQLGrammar.UpdateStatementNonTermName:
                    {
                        action = ToUpdateAction();
                        break;
                    }
                case ID3SQLGrammar.DeleteStatementNonTermName:
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
                case ID3SQLGrammar.SelectStatementNonTermName:
                    {
                        whereNode = rootNode.ChildNodes[2];
                        break;
                    }
                case ID3SQLGrammar.UpdateStatementNonTermName:
                    {
                        whereNode = rootNode.ChildNodes[3];
                        break;
                    }
                case ID3SQLGrammar.DeleteStatementNonTermName:
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
            if (whereNode.ChildNodes.Count == 0)
            {
                whereExecutionFn = (file, filePath, executionPlanOptions) => true;
            }
            else if(whereNode.ChildNodes.Count == 2)
            {
                ParseTreeNode expressionNode = whereNode.ChildNodes[1];

                Func<File, string, ExecutionPlanOptions, object> expressionFn = ToExpressionFunc(expressionNode);

                whereExecutionFn = (file, filePath, executionPlanOptions) =>
                {
                    return Convert.ToBoolean(expressionFn(file, filePath, executionPlanOptions));
                };
            }
            else
            {
                throw new Exception("Unknown where non-term");
            }

            return whereExecutionFn;
        }

        private static Func<File, string, ExecutionPlanOptions, object> ToExpressionFunc(ParseTreeNode node)
        {
            Func<File, string, ExecutionPlanOptions, object> expressionFn;
            switch (node.Term.Name)
            {
                case ID3SQLGrammar.IdTermName:
                    {
                        string propertyName = node.Token.Text;
                        Func<File, string, object>  getFn = TagFunctionManager.GetFunction(propertyName);
                        expressionFn = (file, filePath, executionPlanOptions) =>
                        {
                            return getFn(file, filePath);
                        };
                        break;
                    }
                case ID3SQLGrammar.NumberTermName:
                    {
                        string numberStr = node.Token.Text;
                        decimal number = decimal.Parse(numberStr);
                        expressionFn = (file, filePath, executionPlanOptions) =>
                        {
                            return number;
                        };
                        break;
                    }
                case ID3SQLGrammar.StringLiteralTermName:
                    {
                        string str = node.Token.Text;
                        expressionFn = (file, filePath, executionPlanOptions) =>
                        {
                            return Convert.ToBoolean(str);
                        };
                        break;
                    }
                case ID3SQLGrammar.UnaryExpressionNonTermName:
                    {
                        ParseTreeNode termNode = node.ChildNodes[1];

                        Func<File, string, ExecutionPlanOptions, object> unaryExpressionFn;
                        switch(termNode.Token.Text)
                        {
                            case ID3SQLGrammar.TermNonTermName:
                                {
                                    string propertyName = termNode.Token.Text;
                                    Func<File, string, object> getFn = TagFunctionManager.GetFunction(propertyName);
                                    unaryExpressionFn = (file, filePath, executionPlanOptions) =>
                                    {
                                        return getFn(file, filePath);
                                    };
                                    break;
                                }
                            case ID3SQLGrammar.NumberTermName:
                                {
                                    string numberStr = termNode.Token.Text;
                                    decimal number = decimal.Parse(numberStr);
                                    unaryExpressionFn = (file, filePath, executionPlanOptions) =>
                                    {
                                        return number;
                                    };
                                    break;
                                }
                            case ID3SQLGrammar.StringLiteralTermName:
                                {
                                    string str = termNode.Token.Text;
                                    unaryExpressionFn = (file, filePath, executionPlanOptions) =>
                                    {
                                        return str;
                                    };
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("Unknown unary expression non-term");
                                }
                        }

                        string unaryOperand = node.ChildNodes[0].ChildNodes[0].Token.Text;
                        switch(unaryOperand)
                        {
                            case "NOT":
                                {
                                    expressionFn = (file, filePath, executionPlanOptions) =>
                                    {
                                        object obj = unaryExpressionFn(file, filePath, executionPlanOptions);

                                        bool b = Convert.ToBoolean(obj);

                                        return !b;
                                    };
                                    break;
                                }
                            case "-":
                                {
                                    expressionFn = (file, filePath, executionPlanOptions) =>
                                    {
                                        object obj = unaryExpressionFn(file, filePath, executionPlanOptions);

                                        if(!IsNumeric(obj))
                                        {
                                            throw new Exception("Cannot compare non-numerics");
                                        }

                                        decimal number = Convert.ToDecimal(obj);
                                        return -number;
                                    };
                                    break;
                                }
                            default:
                                {
                                    throw new Exception("Unknown unary expression non-term");
                                }
                        }
                        break;
                    }
                case ID3SQLGrammar.BinaryExpressionNonTermName:
                    {
                        if(node.ChildNodes.Count == 3)
                        {
                            Func<File, string, ExecutionPlanOptions, object> expressionFn1 = ToExpressionFunc(node.ChildNodes[0]);
                            Func<File, string, ExecutionPlanOptions, object> expressionFn2 = ToExpressionFunc(node.ChildNodes[2]);

                            string binaryOperand = node.ChildNodes[1].ChildNodes[0].Token.Text;
                            switch (binaryOperand)
                            {
                                case "=":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            if(IsNumeric(obj1) && IsNumeric(obj2))
                                            {
                                                decimal number1 = Convert.ToDecimal(obj1);
                                                decimal number2 = Convert.ToDecimal(obj2);

                                                return number1 == number2;
                                            }
                                            else
                                            {
                                                return obj1 == obj2 || object.Equals(obj1, obj2);
                                            }
                                        };
                                        break;
                                    }
                                case "<":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            if(!IsNumeric(obj1) || !IsNumeric(obj2))
                                            {
                                                throw new Exception("Cannot compare non-numerics");
                                            }

                                            decimal number1 = Convert.ToDecimal(obj1);
                                            decimal number2 = Convert.ToDecimal(obj2);

                                            return number1 < number2;
                                        };
                                        break;
                                    }
                                case ">":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            if (!IsNumeric(obj1) || !IsNumeric(obj2))
                                            {
                                                throw new Exception("Cannot compare non-numerics");
                                            }

                                            decimal number1 = Convert.ToDecimal(obj1);
                                            decimal number2 = Convert.ToDecimal(obj2);

                                            return number1 > number2;
                                        };
                                        break;
                                    }
                                case "<=":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            if (!IsNumeric(obj1) || !IsNumeric(obj2))
                                            {
                                                throw new Exception("Cannot compare non-numerics");
                                            }

                                            decimal number1 = Convert.ToDecimal(obj1);
                                            decimal number2 = Convert.ToDecimal(obj2);

                                            return number1 <= number2;
                                        };
                                        break;
                                    }
                                case ">=":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            if (!IsNumeric(obj1) || !IsNumeric(obj2))
                                            {
                                                throw new Exception("Cannot compare non-numerics");
                                            }

                                            decimal number1 = Convert.ToDecimal(obj1);
                                            decimal number2 = Convert.ToDecimal(obj2);

                                            return number1 >= number2;
                                        };
                                        break;
                                    }
                                case "!=":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            return obj1 != obj2 && !object.Equals(obj1, obj2);
                                        };
                                        break;
                                    }
                                case "AND":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            bool bool1 = Convert.ToBoolean(obj1);
                                            bool bool2 = Convert.ToBoolean(obj2);

                                            return bool1 && bool2;
                                        };
                                        break;
                                    }
                                case "OR":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            bool bool1 = Convert.ToBoolean(obj1);
                                            bool bool2 = Convert.ToBoolean(obj2);

                                            return bool1 || bool2;
                                        };
                                        break;
                                    }
                                case "LIKE":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            string str1 = obj1 as string;
                                            string str2 = obj2 as string;

                                            if(str1 == null || str2 == null)
                                            {
                                                throw new Exception("Cannot compare non-strings");
                                            }

                                            Regex regex = new Regex(str2);

                                            return regex.IsMatch(str1);
                                        };
                                        break;
                                    }
                                case "IN":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            IEnumerable<object> collection = obj2 as IEnumerable<object>;
                                            if (collection == null)
                                            {
                                                throw new Exception("Cannot compare non-enumerables");
                                            }

                                            return collection.Contains(obj1);
                                        };
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception("Unknown binary expression non-term");
                                    }

                            }
                        }
                        else if(node.ChildNodes.Count == 4)
                        {
                            Func<File, string, ExecutionPlanOptions, object> expressionFn1 = ToExpressionFunc(node.ChildNodes[0]);
                            Func<File, string, ExecutionPlanOptions, object> expressionFn2 = ToExpressionFunc(node.ChildNodes[3]);

                            string binaryOperand = string.Format("{0} {1}", node.ChildNodes[1].Token.Text, node.ChildNodes[2].Token.Text);
                            switch (binaryOperand)
                            {
                                case "NOT LIKE":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            string str1 = obj1 as string;
                                            string str2 = obj2 as string;

                                            if(str1 == null || str2 == null)
                                            {
                                                throw new Exception("Cannot compare non-strings");
                                            }

                                            Regex regex = new Regex(str2);

                                            return !regex.IsMatch(str1);
                                        };
                                        break;
                                    }
                                case "NOT IN":
                                    {
                                        expressionFn = (file, filePath, executionPlanOptions) =>
                                        {
                                            object obj1 = expressionFn1(file, filePath, executionPlanOptions);
                                            object obj2 = expressionFn2(file, filePath, executionPlanOptions);

                                            IEnumerable<object> collection = obj2 as IEnumerable<object>;
                                            if (collection == null)
                                            {
                                                throw new Exception("Cannot compare non-enumerables");
                                            }

                                            return !collection.Contains(obj1);
                                        };
                                        break;
                                    }
                                default:
                                    {
                                        throw new Exception("Unknown binary expression non-term");
                                    }
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown binary expression non-term");
                        }
                        
                        break;
                    }
                default:
                    {
                        throw new Exception("Unknown expression non-term");
                    }
            }
            return expressionFn;
        }

        private static bool IsNumeric(object value)
        {
            return value is sbyte
            || value is byte
            || value is short
            || value is ushort
            || value is int
            || value is uint
            || value is long
            || value is ulong
            || value is float
            || value is double
            || value is decimal;
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
