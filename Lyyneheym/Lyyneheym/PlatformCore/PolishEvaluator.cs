using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Yuri.PlatformCore
{
    /// <summary>
    /// 逆波兰式求值器
    /// </summary>
    internal static class PolishEvaluator
    {
        /// <summary>
        /// 计算表达式
        /// </summary>
        /// <param name="polish">表达式的逆波兰式</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>计算结果的值（Double/字符串）</returns>
        public static object Evaluate(string polish, StackMachine vsm)
        {
            List<PolishItem> calcList = PolishEvaluator.GetPolishItemList(polish, vsm);
            if (calcList.Count == 0)
            {
                return null;
            }
            Stack<PolishItem> calcStack = new Stack<PolishItem>();
            foreach (PolishItem poi in calcList)
            {
                // 操作数压栈
                if (poi.ItemType < PolishItemType.CAL_PLUS)
                {
                    calcStack.Push(poi);
                    continue;
                }
                // 下面开始是运算符
                if (poi.ItemType == PolishItemType.CAL_NOT && calcStack.Count >= 1)
                {
                    PolishItem peeker = calcStack.Peek();
                    if (peeker.ItemType == PolishItemType.CONSTANT || peeker.ItemType == PolishItemType.VAR_NUM)
                    {
                        calcStack.Pop();
                        double notres = Math.Abs(peeker.Number) < 1e-15 ? 1.0 : 0.0;
                        PolishItem np = new PolishItem()
                        {
                            Number = notres,
                            Reference = notres
                        };
                        calcStack.Push(np);
                        continue;
                    }
                    else if (peeker.ItemType == PolishItemType.STRING || peeker.ItemType == PolishItemType.VAR_STRING)
                    {
                        calcStack.Pop();
                        double notres = peeker.Cluster == String.Empty ? 1.0 : 0.0;

                        PolishItem np = new PolishItem()
                        {
                            Number = notres,
                            Reference = notres
                        };
                        calcStack.Push(np);
                        continue;
                    }
                }
                if (calcStack.Count >= 2)
                {
                    PolishItem operand2 = calcStack.Pop();
                    PolishItem operand1 = calcStack.Pop();
                    if (PolishItem.isOperatable(operand1, operand2) == true)
                    {
                        PolishItem newPoi = null;
                        double tempDouble = 0;
                        string tempString = String.Empty;
                        switch (poi.ItemType)
                        {
                            case PolishItemType.CAL_PLUS:
                                if (operand1.Reference is string)
                                {
                                    tempString = (string)operand1.Reference + (string)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Cluster = tempString,
                                        Reference = tempString
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (double)operand1.Reference + (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_MINUS:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference - (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：减法");
                                }
                                break;
                            case PolishItemType.CAL_MULTI:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference * (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：乘法");
                                }
                                break;
                            case PolishItemType.CAL_DIV:
                                if (operand1.Reference is double)
                                {
                                    if (Math.Abs((double)operand2.Reference) < 0)
                                    {
                                        throw new Exception("除零错误");
                                    }
                                    tempDouble = (double)operand1.Reference / (double)operand2.Reference;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：除法");
                                }
                                break;
                            case PolishItemType.CAL_ANDAND:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (Math.Abs((double)operand1.Reference) > 0 && Math.Abs((double)operand2.Reference) > 0) ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：&&");
                                }
                                break;
                            case PolishItemType.CAL_OROR:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (Math.Abs((double)operand1.Reference) > 0 || Math.Abs((double)operand2.Reference) > 0) ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    throw new Exception("字符串异常操作：||");
                                }
                                break;
                            case PolishItemType.CAL_EQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference == (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (string)operand1.Reference == (string)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_NOTEQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference != (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = (string)operand1.Reference != (string)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_BIG:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference > (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) > 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_BIGEQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference >= (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) >= 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_SMALL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference < (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) < 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            case PolishItemType.CAL_SMALLEQUAL:
                                if (operand1.Reference is double)
                                {
                                    tempDouble = (double)operand1.Reference <= (double)operand2.Reference ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                else
                                {
                                    tempDouble = String.Compare((string)operand1.Reference, (string)operand2.Reference) <= 0 ? 1 : 0;
                                    newPoi = new PolishItem()
                                    {
                                        Number = tempDouble,
                                        Reference = tempDouble
                                    };
                                    calcStack.Push(newPoi);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (calcStack.Count != 1)
            {
                Utils.CommonUtils.ConsoleLine("求值器无法计算逆波兰式：" + polish, "PolishEvaluator", Utils.OutputStyle.Error);
                throw new Exception("表达式有错误");
            }
            return calcStack.Peek().Reference;
        }

        /// <summary>
        /// 计算表达式的真值
        /// </summary>
        /// <param name="polish">表达式的逆波兰式</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>表达式的真值</returns>
        public static bool EvaluateBoolean(string polish, StackMachine vsm)
        {
            return Convert.ToBoolean(PolishEvaluator.Evaluate(polish, vsm));
        }
        
        /// <summary>
        /// 将逆波兰式转化为可计算的项
        /// </summary>
        /// <param name="polish">逆波兰式字符串</param>
        /// <param name="vsm">关于哪个调用堆栈做动作</param>
        /// <returns>可计算项目向量</returns>
        private static List<PolishItem> GetPolishItemList(string polish, StackMachine vsm)
        {
            List<PolishItem> resVec = new List<PolishItem>();
            string[] polishItem = polish.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in polishItem)
            {
                PolishItem poi = null;
                Regex floatRegEx = new Regex(@"^(\-|\+)?\d+(\.\d+)?$");
                // 常数项
                if (floatRegEx.IsMatch(item))
                {
                    double numitem = Convert.ToDouble(item);
                    poi = new PolishItem()
                    {
                        Number = numitem,
                        Cluster = null,
                        ItemType = PolishItemType.CONSTANT,
                        Reference = numitem
                    };
                }
                // 字符串
                else if (item.StartsWith("\"") && item.EndsWith("\""))
                {
                    string trimItem = item.Substring(1, item.Length - 2);
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = trimItem,
                        ItemType = PolishItemType.STRING,
                        Reference = trimItem
                    };
                }
                // 变量时
                else if ((item.StartsWith("&") || item.StartsWith("$")) && item.Length > 1)
                {
                    object varRef = Director.RunMana.Fetch(item, vsm);
                    if (varRef is double)
                    {
                        poi = new PolishItem()
                        {
                            Number = (double)varRef,
                            Cluster = null,
                            ItemType = PolishItemType.VAR_NUM,
                            Reference = varRef
                        };
                    }
                    else
                    {
                        poi = new PolishItem()
                        {
                            Number = 0.0f,
                            Cluster = Convert.ToString(varRef),
                            ItemType = PolishItemType.VAR_STRING,
                            Reference = varRef
                        };
                    }
                }
                else if (item == "+")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_PLUS,
                        Reference = null
                    };
                }
                else if (item == "-")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_MINUS,
                        Reference = null
                    };
                }
                else if (item == "*")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_MULTI,
                        Reference = null
                    };
                }
                else if (item == "/")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_DIV,
                        Reference = null
                    };
                }
                else if (item == "!")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_NOT,
                        Reference = null
                    };
                }
                else if (item == "&&")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_ANDAND,
                        Reference = null
                    };
                }
                else if (item == "||")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_OROR,
                        Reference = null
                    };
                }
                else if (item == "<>")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_NOTEQUAL,
                        Reference = null
                    };
                }
                else if (item == "==")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_EQUAL,
                        Reference = null
                    };
                }
                else if (item == ">")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_BIG,
                        Reference = null
                    };
                }
                else if (item == "<")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_SMALL,
                        Reference = null
                    };
                }
                else if (item == ">=")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_BIGEQUAL,
                        Reference = null
                    };
                }
                else if (item == "<=")
                {
                    poi = new PolishItem()
                    {
                        Number = 0.0f,
                        Cluster = null,
                        ItemType = PolishItemType.CAL_SMALLEQUAL,
                        Reference = null
                    };
                }
                if (poi != null)
                {
                    resVec.Add(poi);
                }
            }
            return resVec;
        }
    }
}
