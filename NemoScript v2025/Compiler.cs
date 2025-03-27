using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace NemoScript
{
    public class Compiler
    {
        List<int> NumVariables = new List<int>();
        List<string> NumVariablesNames = new List<string>();
        List<double> NumfVariables = new List<double>();
        List<string> NumfVariablesNames = new List<string>();
        List<string> TextVariables = new List<string>();
        List<string> TextVariablesNames = new List<string>();
        List<bool> BooleanVariables = new List<bool>();
        List<string> BooleanVariablesNames = new List<string>();
        List<string> Code = new List<string>();

        public Compiler(List<string> Code)
        {
            this.Code = Code;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("How to use NemoScript CLI.\nNemoScript.exe [filename].nemo");
                Console.ReadKey();
                System.Environment.Exit(0);
            }

            List<string> Code = new List<string>();
            try
            {
                foreach (string f in File.ReadAllLines(args[0]))
                {
                    Code.Add(f);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"{args[0]} does not exist!");
                System.Environment.Exit(0);
            }

            Compiler compiler = new Compiler(Code);
            compiler.Compile();
        }

        public void Compile()
        {
            int Line = 0;
            while(Line < Code.Count)
            {
                switch(Code[Line])
                {
                    case "say>":
                        Line++;
                        string Args = "";

                        if (Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@""""))
                        {
                            Args = Code[Line].Replace(@"""", "");
                        } 
						else
                        {
                            string VarName = Code[Line];
                            switch (GetVarType(VarName))
                            {
                                case "num":
                                    Args = Convert.ToString(NumVariables[IndexOf(VarName)]);
                                    break;
                                case "numf":
                                    Args = NumfVariables[IndexOf(VarName)].ToString();
                                    break;
                                case "text":
                                    Args = TextVariables[IndexOf(VarName)];
                                    break;
                                case "boolean":
                                    Args = Convert.ToString(BooleanVariables[IndexOf(VarName)]);
                                    break;
                                case "not found":
                                    Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. {VarName} does not exist in current context.");
                                    End();
                                    break;
                            }
                        }

                        Line++;
                        Console.WriteLine(Args);
                        Line++;
                        break;
                    case "var>":
                        Line++;
                        int Placeholder1;
                        double Placholder2;
                        bool NameIsText = Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@"""") ? true : false;
                        bool NameIsFunc = Code[Line].EndsWith(@">");
                        bool NameIsBoolean = Code[Line].ToLower() == "true" || Code[Line].ToLower() == "false" ? true : false;
                        bool NameIsNum = int.TryParse(Code[Line], out Placeholder1);
                        bool NameIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.InvariantCulture, out Placholder2);
                        switch (Code[Line])
                        {
                            case "num":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if(NameIsNum || NameIsNumf || NameIsBoolean || NameIsText || NameIsFunc)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                        End();
                                    } else
                                    {
                                        NumVariablesNames.Add(Code[Line]);
                                    }
                                } else
                                {
                                    Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                int LineNum;
                                bool LineIsNum = int.TryParse(Code[Line], out LineNum);

                                if (Code[Line] == "waitinput")
                                {
                                    try
                                    {
                                        NumVariables.Add(Convert.ToInt32(Console.ReadLine()));
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Invalid written data type.");
                                        End();
                                    }
                                }
                                else if (LineIsNum)
                                {
                                    NumVariables.Add(Convert.ToInt32(Code[Line]));
                                }
                                else
                                {
                                    if (GetVarType(Code[Line]) == "num")
                                    {
                                      NumVariables.Add(NumVariables[IndexOf(Code[Line])]);
                                    }
                                    else if (GetVarType(Code[Line]) == "not found")
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                        End();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                        End();
                                    }
                                }

                                Line++;
                                break;
                            case "numf":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if (NameIsNum || NameIsNumf || NameIsBoolean || NameIsText || NameIsFunc)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                        End();
                                    }
                                    else
                                    {
                                        NumfVariablesNames.Add(Code[Line]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                double LineNumf;
                                bool LineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);

                                if (Code[Line] == "waitinput")
                                {
                                    try
                                    {
                                        NumfVariables.Add(Convert.ToDouble(Console.ReadLine()));
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Invalid written data type.");
                                        End();
                                    }
                                }
                                else if(LineIsNumf)
                                {
                                    NumfVariables.Add(Convert.ToDouble(Code[Line]));
                                }
                                else
                                {
                                    if (GetVarType(Code[Line]) == "numf")
                                    {
                                        NumfVariables.Add(NumfVariables[IndexOf(Code[Line])]);
                                    }
                                    else if (GetVarType(Code[Line]) == "not found")
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                        End();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                        End();
                                    }
                                }

                                Line++;
                                break;
                            case "text":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if (NameIsNum || NameIsNumf || NameIsBoolean || NameIsText || NameIsFunc)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                    }
                                    else
                                    {
                                        TextVariablesNames.Add(Code[Line]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                if (Code[Line] == "waitinput")
                                {
                                    TextVariables.Add(Console.ReadLine());
                                }
                                else if (Code[Line].StartsWith(@"""") && Code[Line].StartsWith(@""""))
                                {
                                    TextVariables.Add(Code[Line].Replace(@"""", ""));
                                }
                                else
                                {
                                    if (GetVarType(Code[Line]) == "text")
                                    {
                                        TextVariables.Add(TextVariables[IndexOf(Code[Line])]);
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) != "not found")
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                            End();
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                }
                                Line++;
                                break;
                            case "boolean":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if (NameIsNum || NameIsNumf || NameIsBoolean || NameIsText || NameIsFunc)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                    }
                                    else
                                    {
                                        BooleanVariablesNames.Add(Code[Line]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                bool LineBool;
                                bool LineIsBool = bool.TryParse(Code[Line], out LineBool);
                                if (LineIsBool)
                                {
                                    BooleanVariables.Add(Convert.ToBoolean(Code[Line]));
                                } else
                                {
                                    if (GetVarType(Code[Line]) == "boolean")
                                    {
                                        BooleanVariables.Add(BooleanVariables[IndexOf(Code[Line])]);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Data type mismatch or variable does not exist in current context.");
                                        End();
                                    }
                                }
                                Line++;
                                break;
                            default:
                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                End();
                                break;
                        }
                        Line++;
                        break;
                    case "varcalc>":
                        Line++;
                        switch (GetVarType(Code[Line]))
                        {
                            case "num":
                                int LineNum;
                                string FirstVar = Code[Line];
                                Line++;
                                switch(Code[Line])
                                {
                                    case "+":
                                        Line++;
                                        bool LineIsNumPLUS = int.TryParse(Code[Line], out LineNum);
                                        if(LineIsNumPLUS)
                                        {
                                            NumVariables[IndexOf(FirstVar)] += LineNum;
                                        } else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] += NumVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "-":
                                        Line++;
                                        bool LineIsNumMINUS = int.TryParse(Code[Line], out LineNum);
                                        if (LineIsNumMINUS)
                                        {
                                            NumVariables[IndexOf(FirstVar)] -= LineNum;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] -= NumVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "/":
                                        Line++;
                                        bool LineIsNumDIVIDE = int.TryParse(Code[Line], out LineNum);
                                        if (LineIsNumDIVIDE)
                                        {
                                            NumVariables[IndexOf(FirstVar)] /= LineNum;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] /= NumVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "*":
                                        Line++;
                                        bool LineIsNumMULTIPLY = int.TryParse(Code[Line], out LineNum);
                                        if (LineIsNumMULTIPLY)
                                        {
                                            NumVariables[IndexOf(FirstVar)] *= LineNum;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] *= NumVariables[IndexOf(Code[Line])];
                                            } else
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    default:
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid math operation.");
                                        End();
                                        break;
                                }
                                Line++;
                                break;
                            case "numf":
                                double LineNumf;
                                string FirstVarNumf = Code[Line];
                                Line++;
                                switch (Code[Line])
                                {
                                    case "+":
                                        Line++;
                                        bool LineIsNumPLUS = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumPLUS)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] += LineNumf;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "numf")
                                            {
                                                NumfVariables[IndexOf(FirstVarNumf)] += NumfVariables[IndexOf(Code[Line])];
                                            } else
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "-":
                                        Line++;
                                        bool LineIsNumMINUS = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumMINUS)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] -= LineNumf;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "numf")
                                            {
                                                NumfVariables[IndexOf(FirstVarNumf)] -= NumfVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "/":
                                        Line++;
                                        bool LineIsNumDIVIDE = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumDIVIDE)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] /= LineNumf;
                                        }
                                        else
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] /= NumfVariables[IndexOf(Code[Line])];
                                        }
                                        break;
                                    case "*":
                                        Line++;
                                        bool LineIsNumMULTIPLY = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumMULTIPLY)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] *= LineNumf;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "numf")
                                            {
                                                NumfVariables[IndexOf(FirstVarNumf)] *= NumfVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    default:
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid math operation.");
                                        End();
                                        break;
                                }
                                Line++;
                                break;
                            case "text":
                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                End();
                                break;
                            case "boolean":
                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                End();
                                break;
                            default:
                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                End();
                                break;
                        }
                        Line++;
                        break;
                    case "while>":
                        {
                            Line++;
                            bool FirstLineIsNumVar = GetVarType(Code[Line]) == "num" ? true : false;
                            bool SecondLineIsNumVar = GetVarType(Code[Line + 2]) == "num" ? true : false;
                            bool FirstLineIsNumfVar = GetVarType(Code[Line]) == "numf" ? true : false;
                            bool SecondLineIsNumfVar = GetVarType(Code[Line + 2]) == "numf" ? true : false;
                            bool FirstLineIsTextVar = GetVarType(Code[Line]) == "text" ? true : false;
                            bool SecondLineIsTextVar = GetVarType(Code[Line + 2]) == "text" ? true : false;
                            bool FirstLineIsBooleanVar = GetVarType(Code[Line]) == "boolean" ? true : false;
                            bool SecondLineIsBooleanVar = GetVarType(Code[Line + 2]) == "boolean" ? true : false;
                            int FirstLineNum;
                            double FirstLineNumf;
                            bool FirstLineIsNum = int.TryParse(Code[Line], out FirstLineNum);
                            bool FirstLineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.InvariantCulture, out FirstLineNumf);
                            int SecondLineNum;
                            bool SecondLineIsNum = int.TryParse(Code[Line + 2], out SecondLineNum);
                            double SecondLineNumf;
                            bool SecondLineIsNumf = double.TryParse(Code[Line + 2], NumberStyles.Any, CultureInfo.InvariantCulture, out SecondLineNumf);
                            bool FirstLineIsText = Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@"""") ? true : false;
                            bool SecondLineIsText = Code[Line + 2].StartsWith(@"""") && Code[Line + 2].EndsWith(@"""") ? true : false;
                            bool FirstLineBoolean;
                            bool SecondLineBoolean;
                            bool FirstLineIsBoolean = bool.TryParse(Code[Line], out FirstLineBoolean);
                            bool SecondLineIsBoolean = bool.TryParse(Code[Line + 2], out SecondLineBoolean);
                            List<string> CodeForWhile = new List<string>();
                            Line++;
                            int WhileCount = 0;

                            for (int i = Line + 2; i < Code.Count; i++)
                            {
                                if (Code[i] == "while>")
                                {
                                    WhileCount++;
                                }

                                if (Code[i] == "<endwhile")
                                {
                                    if (WhileCount >= 1)
                                    {
                                        WhileCount--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                CodeForWhile.Add(Code[i]);
                            }

                            int InCodePosition = Line + 2;
                            switch (Code[Line])
                            {
                                case ">":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (FirstLineNumf > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "<":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (FirstLineNumf < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (FirstLineIsNum && SecondLineIsNumf || FirstLineIsNumf && SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context. FIRST");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        while (Code[Line - 2] == Code[Line])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (Code[Line - 2].Replace(@"""", "") == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] == Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        while (FirstLineBoolean == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (FirstLineBoolean == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                case "!=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        while (Code[Line - 2].Replace(@"""", "") != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (Code[Line - 2].Replace(@"""", "") != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        while (FirstLineBoolean != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (FirstLineBoolean != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid statement.");
                                    End();
                                    break;
                            }
                            Line += CodeForWhile.Count + 2;
                        }
                        break;
                    case "if>":
                        {
                            Line++;
                            bool FirstLineIsNumVar = GetVarType(Code[Line]) == "num" ? true : false;
                            bool SecondLineIsNumVar = GetVarType(Code[Line + 2]) == "num" ? true : false;
                            bool FirstLineIsNumfVar = GetVarType(Code[Line]) == "numf" ? true : false;
                            bool SecondLineIsNumfVar = GetVarType(Code[Line + 2]) == "numf" ? true : false;
                            bool FirstLineIsTextVar = GetVarType(Code[Line]) == "text" ? true : false;
                            bool SecondLineIsTextVar = GetVarType(Code[Line + 2]) == "text" ? true : false;
                            bool FirstLineIsBooleanVar = GetVarType(Code[Line]) == "boolean" ? true : false;
                            bool SecondLineIsBooleanVar = GetVarType(Code[Line + 2]) == "boolean" ? true : false;
                            int FirstLineNum;
                            double FirstLineNumf;
                            bool FirstLineIsNum = int.TryParse(Code[Line], out FirstLineNum);
                            bool FirstLineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.InvariantCulture, out FirstLineNumf);
                            int SecondLineNum;
                            bool SecondLineIsNum = int.TryParse(Code[Line + 2], out SecondLineNum);
                            double SecondLineNumf;
                            bool SecondLineIsNumf = double.TryParse(Code[Line + 2], NumberStyles.Any, CultureInfo.InvariantCulture, out SecondLineNumf);
                            bool FirstLineIsText = Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@"""") ? true : false;
                            bool SecondLineIsText = Code[Line + 2].StartsWith(@"""") && Code[Line + 2].EndsWith(@"""") ? true : false;
                            bool FirstLineBoolean;
                            bool SecondLineBoolean;
                            bool FirstLineIsBoolean = bool.TryParse(Code[Line], out FirstLineBoolean);
                            bool SecondLineIsBoolean = bool.TryParse(Code[Line + 2], out SecondLineBoolean);
                            List<string> CodeForIf = new List<string>();
                            Line++;
                            int IfCount = 0;

                            for (int i = Line + 2; i < Code.Count; i++)
                            {
                                if (Code[i] == "if>")
                                {
                                    IfCount++;
                                }

                                if (Code[i] == "<endif")
                                {
                                    if (IfCount >= 1)
                                    {
                                        IfCount--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                CodeForIf.Add(Code[i]);
                            }

                            int InCodePosition = Line + 2;
                            switch (Code[Line])
                            {
                                case ">":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (FirstLineNumf > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "<":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (FirstLineNumf < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (FirstLineIsNum && SecondLineIsNumf || FirstLineIsNumf && SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context. FIRST");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        if (Code[Line - 2] == Code[Line])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (Code[Line - 2].Replace(@"""", "") == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] == Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        if (FirstLineBoolean == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (FirstLineBoolean == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                case "!=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        if (Code[Line - 2].Replace(@"""", "") != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (Code[Line - 2].Replace(@"""", "") != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        if (FirstLineBoolean != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (FirstLineBoolean != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePosition);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{Line - 1} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"{Line + 1}  >  {Code[Line]} <-- Compile-Time error. Invalid data type");
                                    End();
                                    break;
                            }
                            Line += CodeForIf.Count + 2;
                        }
                        break;
                    /* case "goto>": (coming soon)
                        Line++;
                        int GoToLine = Convert.ToInt32(Code[Line]) - 1;
                        Line++;
                        if(GoToLine <= Code.Count)
                        {
                            if (Code[GoToLine].EndsWith(">"))
                            {
                                Line = GoToLine;
                            }
                            else
                            {
                                Console.WriteLine($"{Line} > {Code[Line - 1]} <-- Compile-Time error. goto> function can send the code only to other functions (for example: say>).");
                                End();
                            }
                        }
                        else
                        {

                        }
                        Line++;
                        break; */
                    case "sleep>":
                        Line++;
                        int ToSleep;
                        bool SleepLineIsNum = int.TryParse(Code[Line], out ToSleep);
                        if(SleepLineIsNum)
                        {
                            Thread.Sleep(ToSleep);
                        }
                        else if (Code[Line].StartsWith("-"))
                        {
                            Console.WriteLine($"{Line + 1}  >  {Code[Line]} <-- Compile-Time error. sleep> accepts only non-negative numbers");
                            End();
                        }
                        else
                        {
                            if (GetVarType(Code[Line]) == "num")
                            {
                                Thread.Sleep(NumVariables[IndexOf(Code[Line])]);
                            } 
                            else if (GetVarType(Code[Line]) == "not found")
                            {
                                Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                End();
                            }
                            else
                            {
                                Console.WriteLine($"{Line + 1}  >  {Code[Line]} <-- Compile-Time error. Invalid data type");
                                End();
                            }
                        }
                        Line += 2;
                        break;
                    case "waitinput":
                        Console.ReadLine();
                        Line++;
                        break;
                    case "waitkey":
                        Console.ReadKey();
                        Line++;
                        break;
                    case "endprog":
                        End();
                        break;
                    default:
                        if (Code[Line].EndsWith('>'))
                        {
                            string VarName = Code[Line].Replace(">", "");
                            switch (GetVarType(VarName))
                            {
                                case "num":
                                    Line++;
                                    int LineNum;
                                    bool LineIsNum = int.TryParse(Code[Line], out LineNum);

                                    if (LineIsNum)
                                    {
                                        NumVariables[IndexOf(VarName)] = LineNum;
                                    }
                                    else if (Code[Line] == "waitinput")
                                    {
                                        try
                                        {
                                            NumVariables[IndexOf(VarName)] = Convert.ToInt32(Console.ReadLine());
                                        }
                                        catch (FormatException)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Invalid written data type.");
                                            End();
                                        }
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) == "num")
                                        {
                                            NumVariables[IndexOf(VarName)] = NumVariables[IndexOf(Code[Line])];
                                        }
                                        else if (Code[Line] == "waitinput")
                                        {
                                            NumVariables[IndexOf(VarName)] = Convert.ToInt32(Console.ReadLine());
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    Line += 2;
                                    break;
                                case "numf":
                                    Line++;
                                    double LineNumf;
                                    bool LineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);

                                    if (LineIsNumf)
                                    {
                                        NumfVariables[IndexOf(VarName)] = LineNumf;
                                    }
                                    else if (Code[Line] == "waitinput")
                                    {
                                        try
                                        {
                                            NumfVariables[IndexOf(VarName)] = Convert.ToDouble(Console.ReadLine());
                                        }
                                        catch (FormatException)
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Invalid written data type.");
                                            End();
                                        }
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) == "numf")
                                        {
                                            NumfVariables[IndexOf(VarName)] = NumfVariables[IndexOf(Code[Line])];
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    Line += 2;
                                    break;
                                case "text":
                                    Line++;

                                    if (Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@""""))
                                    {
                                        TextVariables[IndexOf(VarName)] = Code[Line].Replace(@"""", "");
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) == "text")
                                        {
                                            TextVariables[IndexOf(VarName)] = TextVariables[IndexOf(Code[Line])];
                                        }
                                        else if (Code[Line] == "waitinput")
                                        {
                                            TextVariables[IndexOf(VarName)] = Console.ReadLine();
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }

                                    Line += 2;
                                    break;
                                case "boolean":
                                    Line++;
                                    bool LineBoolean;
                                    bool LineIsBoolean = bool.TryParse(Code[Line], out LineBoolean);

                                    if(LineIsBoolean)
                                    {
                                        BooleanVariables[IndexOf(VarName)] = Convert.ToBoolean(Code[Line]);
                                    } else
                                    {
                                        if (GetVarType(Code[Line]) == "boolean")
                                        {
                                            BooleanVariables[IndexOf(VarName)] = BooleanVariables[IndexOf(Code[Line])];
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }

                                    Line += 2;
                                    break;
                                default:
                                    Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Compile-Time error. {VarName} does not exist in current context.");
                                    End();
                                    break;
                            }
                        } 
                        else
                        {
                            Console.WriteLine($"{Line + 1} > {Code[Line].Replace(@"""", "")} <-- Compile-Time error. Unknown function");
                            End();
                        }
                        break;
                }
            }

            NumVariables.Clear();
            NumVariablesNames.Clear();
            NumfVariables.Clear();
            NumfVariablesNames.Clear();
            TextVariables.Clear();
            TextVariablesNames.Clear();
            BooleanVariables.Clear();
            BooleanVariablesNames.Clear();
        }

        void End()
        {
            System.Environment.Exit(0);
        }

        string GetVarType(string VarName)
        {
            if (NumVariablesNames.Contains(VarName))
            {
                return "num";
            }
            else if (NumfVariablesNames.Contains(VarName))
            {
                return "numf";
            }
            else if (TextVariablesNames.Contains(VarName))
            {
                return "text";
            }
            else if (BooleanVariablesNames.Contains(VarName))
            {
                return "boolean";
            }

            return "not found";
        }

        int IndexOf(string VarName)
        {
            switch (GetVarType(VarName))
            {
                case "num":
                    return NumVariablesNames.IndexOf(VarName);
                case "numf":
                    return NumfVariablesNames.IndexOf(VarName);
                case "text":
                    return TextVariablesNames.IndexOf(VarName);
                case "boolean":
                    return BooleanVariablesNames.IndexOf(VarName);
            }

            return -1;
        }

        int CheckExistant(string VarName)
        {
            int i = 0;

            if (NumVariablesNames.Contains(VarName))
            {
                i++;
            }
            else if (NumfVariablesNames.Contains(VarName))
            {
                i++;
            }
            else if (TextVariablesNames.Contains(VarName))
            {
                i++;
            }
            else if (BooleanVariablesNames.Contains(VarName))
            {
                i++;
            }

            return i;
        }

        void CompileForIfOrWhile(List<string> Code, int InCodePosition)
        {
            int Line = 0;
            while (Line < Code.Count)
            {
                switch (Code[Line])
                {
                    case "say>":
                        Line++;
                        string Args = "";

                        if (Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@""""))
                        {
                            Args = Code[Line].Replace(@"""", "");
                        }
                        else
                        {
                            string VarName = Code[Line];
                            switch (GetVarType(VarName))
                            {
                                case "num":
                                    Args = Convert.ToString(NumVariables[IndexOf(VarName)]);
                                    break;
                                case "numf":
                                    Args = NumfVariables[IndexOf(VarName)].ToString();
                                    break;
                                case "text":
                                    Args = TextVariables[IndexOf(VarName)];
                                    break;
                                case "boolean":
                                    Args = Convert.ToString(BooleanVariables[IndexOf(VarName)]);
                                    break;
                                case "not found":
                                    Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. {VarName} does not exist in current context.");
                                    End();
                                    break;
                            }
                        }

                        Line++;
                        Console.WriteLine(Args);
                        Line++;
                        break;
                    case "var>":
                        Line++;
                        int Placeholder1;
                        double Placholder2;
                        bool Placholder3;
                        bool LineIsText = Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@"""") ? true : false;
                        bool LineIsFunc = Code[Line].EndsWith(@">");
                        bool NameIsNum = int.TryParse(Code[Line], out Placeholder1);
                        bool NameIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.InvariantCulture, out Placholder2);
                        bool NameIsBoolean = bool.TryParse(Code[Line], out Placholder3);
                        switch (Code[Line])
                        {
                            case "num":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if (NameIsNum || NameIsNumf || NameIsBoolean || LineIsText || LineIsFunc)
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                        End();
                                    }
                                    else
                                    {
                                        NumVariablesNames.Add(Code[Line]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                int LineNum;
                                bool LineIsNum = int.TryParse(Code[Line], out LineNum);

                                if (Code[Line] == "waitinput")
                                {
                                    try
                                    {
                                        NumVariables.Add(Convert.ToInt32(Console.ReadLine()));
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Invalid written data type.");
                                        End();
                                    }
                                }
                                else if (LineIsNum)
                                {
                                    NumVariables.Add(Convert.ToInt32(Code[Line]));
                                }
                                else
                                {
                                    if (GetVarType(Code[Line]) == "num")
                                    {
                                        NumVariables.Add(NumVariables[IndexOf(Code[Line])]);
                                    }
                                    else if (GetVarType(Code[Line]) == "not found")
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                        End();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                        End();
                                    }
                                }

                                Line++;
                                break;
                            case "numf":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if (NameIsNum || NameIsNumf || NameIsBoolean || LineIsText)
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                        End();
                                    }
                                    else
                                    {
                                        NumfVariablesNames.Add(Code[Line]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                double LineNumf;
                                bool LineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);

                                if (Code[Line] == "waitinput")
                                {
                                    try
                                    {
                                        NumfVariables.Add(Convert.ToDouble(Console.ReadLine()));
                                    }
                                    catch (FormatException)
                                    {
                                        Console.WriteLine($"{Line + 1} > {Code[Line]} <-- Invalid written data type.");
                                        End();
                                    }
                                }
                                else if (LineIsNumf)
                                {
                                    NumfVariables.Add(Convert.ToDouble(Code[Line]));
                                }
                                else
                                {
                                    if (GetVarType(Code[Line]) == "numf")
                                    {
                                        NumfVariables.Add(NumfVariables[IndexOf(Code[Line])]);
                                    }
                                    else if (GetVarType(Code[Line]) == "not found")
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                        End();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                        End();
                                    }
                                }

                                Line++;
                                break;
                            case "text":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if (NameIsNum || NameIsNumf || NameIsBoolean || LineIsText)
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                        End();
                                    }
                                    else
                                    {
                                        TextVariablesNames.Add(Code[Line]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                if (Code[Line] == "waitinput")
                                {
                                    TextVariables.Add(Console.ReadLine());
                                }
                                else if (Code[Line].StartsWith(@"""") && Code[Line].StartsWith(@""""))
                                {
                                    TextVariables.Add(Code[Line]);
                                }
                                else
                                {
                                    if (GetVarType(Code[Line]) == "text")
                                    {
                                        TextVariables.Add(TextVariables[IndexOf(Code[Line])]);
                                    }
                                    else if (GetVarType(Code[Line]) == "not found")
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                        End();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                        End();
                                    }
                                }
                                Line++;
                                break;
                            case "boolean":
                                Line++;
                                if (CheckExistant(Code[Line]) == 0)
                                {
                                    if (NameIsNum || NameIsNumf || NameIsBoolean || LineIsText)
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid variable. Use a name that will not conflicting with variable conditions. Example: justvar");
                                        End();
                                    }
                                    else
                                    {
                                        BooleanVariablesNames.Add(Code[Line]);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable {Code[Line]} already exist in current context.");
                                    End();
                                }
                                Line++;
                                bool LineBool;
                                bool LineIsBool = bool.TryParse(Code[Line], out LineBool);
                                if (LineIsBool)
                                {
                                    BooleanVariables.Add(Convert.ToBoolean(Code[Line]));
                                }
                                else
                                {
                                    if (GetVarType(Code[Line]) == "boolean")
                                    {
                                        BooleanVariables.Add(BooleanVariables[IndexOf(Code[Line])]);
                                    }
                                    else if (GetVarType(Code[Line]) == "not found")
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                        End();
                                    }
                                    else
                                    {
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Data type mismatch.");
                                        End();
                                    }
                                }
                                Line++;
                                break;
                            default:
                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                End();
                                break;
                        }
                        Line++;
                        break;
                    case "varcalc>":
                        Line++;
                        switch (GetVarType(Code[Line]))
                        {
                            case "num":
                                int LineNum;
                                string FirstVar = Code[Line];
                                Line++;
                                switch (Code[Line])
                                {
                                    case "+":
                                        Line++;
                                        bool LineIsNumPLUS = int.TryParse(Code[Line], out LineNum);
                                        if (LineIsNumPLUS)
                                        {
                                            NumVariables[IndexOf(FirstVar)] += LineNum;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] += NumVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "-":
                                        Line++;
                                        bool LineIsNumMINUS = int.TryParse(Code[Line], out LineNum);
                                        if (LineIsNumMINUS)
                                        {
                                            NumVariables[IndexOf(FirstVar)] -= LineNum;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] -= NumVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "/":
                                        Line++;
                                        bool LineIsNumDIVIDE = int.TryParse(Code[Line], out LineNum);
                                        if (LineIsNumDIVIDE)
                                        {
                                            NumVariables[IndexOf(FirstVar)] /= LineNum;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] /= NumVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "*":
                                        Line++;
                                        bool LineIsNumMULTIPLY = int.TryParse(Code[Line], out LineNum);
                                        if (LineIsNumMULTIPLY)
                                        {
                                            NumVariables[IndexOf(FirstVar)] *= LineNum;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "num")
                                            {
                                                NumVariables[IndexOf(FirstVar)] *= NumVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    default:
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid math operation.");
                                        End();
                                        break;
                                }
                                Line++;
                                break;
                            case "numf":
                                double LineNumf;
                                string FirstVarNumf = Code[Line];
                                Line++;
                                switch (Code[Line])
                                {
                                    case "+":
                                        Line++;
                                        bool LineIsNumPLUS = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumPLUS)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] += LineNumf;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "numf")
                                            {
                                                NumfVariables[IndexOf(FirstVarNumf)] += NumfVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "-":
                                        Line++;
                                        bool LineIsNumMINUS = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumMINUS)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] -= LineNumf;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "numf")
                                            {
                                                NumfVariables[IndexOf(FirstVarNumf)] -= NumfVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    case "/":
                                        Line++;
                                        bool LineIsNumDIVIDE = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumDIVIDE)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] /= LineNumf;
                                        }
                                        else
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] /= NumfVariables[IndexOf(Code[Line])];
                                        }
                                        break;
                                    case "*":
                                        Line++;
                                        bool LineIsNumMULTIPLY = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);
                                        if (LineIsNumMULTIPLY)
                                        {
                                            NumfVariables[IndexOf(FirstVarNumf)] *= LineNumf;
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line]) == "numf")
                                            {
                                                NumfVariables[IndexOf(FirstVarNumf)] *= NumfVariables[IndexOf(Code[Line])];
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                                End();
                                            }
                                        }
                                        break;
                                    default:
                                        Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid math operation.");
                                        End();
                                        break;
                                }
                                Line++;
                                break;
                            case "text":
                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                End();
                                break;
                            case "boolean":
                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                End();
                                break;
                            default:
                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                End();
                                break;
                        }
                        Line++;
                        break;
                    case "while>":
                        {
                            Line++;
                            bool FirstLineIsNumVar = GetVarType(Code[Line]) == "num" ? true : false;
                            bool SecondLineIsNumVar = GetVarType(Code[Line + 2]) == "num" ? true : false;
                            bool FirstLineIsNumfVar = GetVarType(Code[Line]) == "numf" ? true : false;
                            bool SecondLineIsNumfVar = GetVarType(Code[Line + 2]) == "numf" ? true : false;
                            bool FirstLineIsTextVar = GetVarType(Code[Line]) == "text" ? true : false;
                            bool SecondLineIsTextVar = GetVarType(Code[Line + 2]) == "text" ? true : false;
                            bool FirstLineIsBooleanVar = GetVarType(Code[Line]) == "boolean" ? true : false;
                            bool SecondLineIsBooleanVar = GetVarType(Code[Line + 2]) == "boolean" ? true : false;
                            int FirstLineNum;
                            double FirstLineNumf;
                            bool FirstLineIsNum = int.TryParse(Code[Line], out FirstLineNum);
                            bool FirstLineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.InvariantCulture, out FirstLineNumf);
                            int SecondLineNum;
                            bool SecondLineIsNum = int.TryParse(Code[Line + 2], out SecondLineNum);
                            double SecondLineNumf;
                            bool SecondLineIsNumf = double.TryParse(Code[Line + 2], NumberStyles.Any, CultureInfo.InvariantCulture, out SecondLineNumf);
                            bool FirstLineIsText = Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@"""") ? true : false;
                            bool SecondLineIsText = Code[Line + 2].StartsWith(@"""") && Code[Line + 2].EndsWith(@"""") ? true : false;
                            bool FirstLineBoolean;
                            bool SecondLineBoolean;
                            bool FirstLineIsBoolean = bool.TryParse(Code[Line], out FirstLineBoolean);
                            bool SecondLineIsBoolean = bool.TryParse(Code[Line + 2], out SecondLineBoolean);
                            List<string> CodeForWhile = new List<string>();
                            Line++;
                            int WhileCount = 0;

                            for (int i = Line + 2; i < Code.Count; i++)
                            {
                                if (Code[i] == "while>")
                                {
                                    WhileCount++;
                                }

                                if (Code[i] == "<endwhile")
                                {
                                    if (WhileCount >= 1)
                                    {
                                        WhileCount--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                CodeForWhile.Add(Code[i]);
                            }

                            int InCodePos = Line + 2;
                            switch (Code[Line])
                            {
                                case ">":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (FirstLineNumf > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "<":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (FirstLineNumf < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (FirstLineIsNum && SecondLineIsNumf || FirstLineIsNumf && SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context. FIRST");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        while (Code[Line - 2] == Code[Line])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (Code[Line - 2].Replace(@"""", "") == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] == Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        while (FirstLineBoolean == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (FirstLineBoolean == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                case "!=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNum != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (FirstLineNum != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        while (NumVariables[IndexOf(Code[Line - 2])] != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        while (FirstLineNumf != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        while (NumfVariables[IndexOf(Code[Line - 2])] != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        while (Code[Line - 2].Replace(@"""", "") != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (Code[Line - 2].Replace(@"""", "") != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        while (TextVariables[IndexOf(Code[Line - 2])] != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        while (FirstLineBoolean != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (FirstLineBoolean != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        while (BooleanVariables[IndexOf(Code[Line - 2])] != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForWhile, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid statement.");
                                    End();
                                    break;
                            }
                            Line += CodeForWhile.Count + 2;
                        }
                        break;
                    case "if>":
                        {
                            Line++;
                            bool FirstLineIsNumVar = GetVarType(Code[Line]) == "num" ? true : false;
                            bool SecondLineIsNumVar = GetVarType(Code[Line + 2]) == "num" ? true : false;
                            bool FirstLineIsNumfVar = GetVarType(Code[Line]) == "numf" ? true : false;
                            bool SecondLineIsNumfVar = GetVarType(Code[Line + 2]) == "numf" ? true : false;
                            bool FirstLineIsTextVar = GetVarType(Code[Line]) == "text" ? true : false;
                            bool SecondLineIsTextVar = GetVarType(Code[Line + 2]) == "text" ? true : false;
                            bool FirstLineIsBooleanVar = GetVarType(Code[Line]) == "boolean" ? true : false;
                            bool SecondLineIsBooleanVar = GetVarType(Code[Line + 2]) == "boolean" ? true : false;
                            int FirstLineNum;
                            double FirstLineNumf;
                            bool FirstLineIsNum = int.TryParse(Code[Line], out FirstLineNum);
                            bool FirstLineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.InvariantCulture, out FirstLineNumf);
                            int SecondLineNum;
                            bool SecondLineIsNum = int.TryParse(Code[Line + 2], out SecondLineNum);
                            double SecondLineNumf;
                            bool SecondLineIsNumf = double.TryParse(Code[Line + 2], NumberStyles.Any, CultureInfo.InvariantCulture, out SecondLineNumf);
                            bool FirstLineIsText = Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@"""") ? true : false;
                            bool SecondLineIsText = Code[Line + 2].StartsWith(@"""") && Code[Line + 2].EndsWith(@"""") ? true : false;
                            bool FirstLineBoolean;
                            bool SecondLineBoolean;
                            bool FirstLineIsBoolean = bool.TryParse(Code[Line], out FirstLineBoolean);
                            bool SecondLineIsBoolean = bool.TryParse(Code[Line + 2], out SecondLineBoolean);
                            List<string> CodeForIf = new List<string>();
                            Line++;
                            int IfCount = 0;

                            for (int i = Line + 2; i < Code.Count; i++)
                            {
                                if (Code[i] == "if>")
                                {
                                    IfCount++;
                                }

                                if (Code[i] == "<endif")
                                {
                                    if (IfCount >= 1)
                                    {
                                        IfCount--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                CodeForIf.Add(Code[i]);
                            }

                            int InCodePos = Line + 2;
                            switch (Code[Line])
                            {
                                case ">":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] > SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] > NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (FirstLineNumf > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] > SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] > NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "<":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] < SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] < NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && !FirstLineIsNum && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (FirstLineNumf < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] < SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] < NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (FirstLineIsNum && SecondLineIsNumf || FirstLineIsNumf && SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context. FIRST");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else
                                        {
                                            if (GetVarType(Code[Line - 2]) != "num" && GetVarType(Code[Line - 2]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                            else if (GetVarType(Code[Line]) != "num" && GetVarType(Code[Line]) != "numf")
                                            {
                                                Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                                End();
                                            }
                                        }
                                    }

                                    break;
                                case "=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] == SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] == NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] == SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] == NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        if (Code[Line - 2] == Code[Line])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (Code[Line - 2].Replace(@"""", "") == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] == Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] == TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        if (FirstLineBoolean == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (FirstLineBoolean == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] == SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] == BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                case "!=":
                                    // num variables
                                    if (FirstLineIsNum && SecondLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNum != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNum && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (FirstLineNum != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNum && FirstLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] != SecondLineNum)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumVar && SecondLineIsNumVar)
                                    {
                                        Line++;
                                        if (NumVariables[IndexOf(Code[Line - 2])] != NumVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // numf variables
                                    else if (FirstLineIsNumf && SecondLineIsNumf && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumf && SecondLineIsNumfVar && !FirstLineIsNum)
                                    {
                                        Line++;
                                        if (FirstLineNumf != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsNumf && !SecondLineIsNum && FirstLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] != SecondLineNumf)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsNumfVar && SecondLineIsNumfVar)
                                    {
                                        Line++;
                                        if (NumfVariables[IndexOf(Code[Line - 2])] != NumfVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // text variables
                                    else if (FirstLineIsText && SecondLineIsText)
                                    {
                                        Line++;
                                        if (Code[Line - 2].Replace(@"""", "") != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsText && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (Code[Line - 2].Replace(@"""", "") != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsText && FirstLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] != Code[Line].Replace(@"""", ""))
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsTextVar && SecondLineIsTextVar)
                                    {
                                        Line++;
                                        if (TextVariables[IndexOf(Code[Line - 2])] != TextVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    // boolean variables
                                    else if (FirstLineIsBoolean && SecondLineIsBoolean)
                                    {
                                        Line++;
                                        if (FirstLineBoolean != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBoolean && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (FirstLineBoolean != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (SecondLineIsBoolean && FirstLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] != SecondLineBoolean)
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else if (FirstLineIsBooleanVar && SecondLineIsBooleanVar)
                                    {
                                        Line++;
                                        if (BooleanVariables[IndexOf(Code[Line - 2])] != BooleanVariables[IndexOf(Code[Line])])
                                        {
                                            CompileForIfOrWhile(CodeForIf, InCodePos);
                                        }
                                    }
                                    else
                                    {
                                        Line++;
                                        if (SecondLineIsNumf || SecondLineIsNum)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line - 2]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line - 2} > {Code[Line - 2]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                        else if (CheckExistant(Code[Line]) == 0)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    break;
                                default:
                                    Console.WriteLine($"{InCodePosition + Line}  >  {Code[Line]} <-- Compile-Time error. Invalid data type");
                                    End();
                                    break;
                            }
                            Line += CodeForIf.Count + 2;
                        }
                        break;
                    /* case "goto>": (coming soon)
                        Line++;
                        int GoToLine = Convert.ToInt32(Code[Line]) - 1;
                        Line++;
                        if(GoToLine > Code.Count)
                        {
                            if(GoToLine <= this.Code.Count)
                            {
                                if (this.Code[GoToLine].EndsWith(">"))
                                {

                                }
                                else
                                {
                                    Console.WriteLine($"{Line} > {Code[Line - 1]} <-- Compile-Time error. goto> function can send the code only to other functions (for example: say>).");
                                    End();
                                }
                            }
                        }
                        if (Code[GoToLine].EndsWith(">"))
                        {
                            Line = GoToLine;
                        }
                        else
                        {
                            Console.WriteLine($"{Line} > {Code[Line - 1]} <-- Compile-Time error. goto> function can send the code only to other functions (for example: say>).");
                            End();
                        }
                        Line++;
                        break; */
                    case "sleep>":
                        Line++;
                        int ToSleep;
                        bool SleepLineIsNum = int.TryParse(Code[Line], out ToSleep);
                        if (SleepLineIsNum)
                        {
                            Thread.Sleep(ToSleep);
                        }
                        else if (Code[Line].StartsWith("-"))
                        {
                            Console.WriteLine($"{this.Code.Count - Code.Count + Line}  >  {Code[Line]} <-- Compile-Time error. sleep> accepts only non-negative numbers");
                            End();
                        }
                        else
                        {
                            if (GetVarType(Code[Line]) == "num")
                            {
                                Thread.Sleep(NumVariables[IndexOf(Code[Line])]);
                            }
                            else if (GetVarType(Code[Line]) == "not found")
                            {
                                Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Variable does not exist in current context.");
                                End();
                            }
                            else
                            {
                                Console.WriteLine($"{this.Code.Count - Code.Count + Line}  >  {Code[Line]} <-- Compile-Time error. Invalid data type");
                                End();
                            }
                        }
                        Line += 2;
                        break;
                    case "waitinput":
                        Console.ReadLine();
                        Line++;
                        break;
                    case "waitkey":
                        Console.ReadKey();
                        Line++;
                        break;
                    case "endprog":
                        End();
                        break;
                    default:
                        if (Code[Line].EndsWith('>'))
                        {
                            string VarName = Code[Line].Replace(">", "");
                            switch (GetVarType(VarName))
                            {
                                case "num":
                                    Line++;
                                    int LineNum;
                                    bool LineIsNum = int.TryParse(Code[Line], out LineNum);

                                    if (LineIsNum)
                                    {
                                        NumVariables[IndexOf(VarName)] = LineNum;
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) == "num")
                                        {
                                            NumVariables[IndexOf(VarName)] = NumVariables[IndexOf(Code[Line])];
                                        }
                                        else if (Code[Line] == "waitinput")
                                        {
                                            try
                                            {
                                                NumVariables[IndexOf(VarName)] = Convert.ToInt32(Console.ReadLine());
                                            }
                                            catch(FormatException)
                                            {
                                                Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Invalid written data type.");
                                                End();
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    Line += 2;
                                    break;
                                case "numf":
                                    Line++;
                                    double LineNumf;
                                    bool LineIsNumf = double.TryParse(Code[Line], NumberStyles.Any, CultureInfo.CurrentCulture, out LineNumf);

                                    if (LineIsNumf)
                                    {
                                        NumfVariables[IndexOf(VarName)] = LineNumf;
                                    }
                                    else if (Code[Line] == "waitinput")
                                    {
                                        try
                                        {
                                            NumfVariables[IndexOf(VarName)] = Convert.ToDouble(Console.ReadLine());
                                        }
                                        catch (FormatException)
                                        {
                                            Console.WriteLine($"{InCodePosition + Line} > {Code[Line]} <-- Invalid written data type.");
                                            End();
                                        }
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) == "numf")
                                        {
                                            NumfVariables[IndexOf(VarName)] = NumfVariables[IndexOf(Code[Line])];
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }
                                    Line += 2;
                                    break;
                                case "text":
                                    Line++;

                                    if (Code[Line].StartsWith(@"""") && Code[Line].EndsWith(@""""))
                                    {
                                        TextVariables[IndexOf(VarName)] = Code[Line].Replace(@"""", "");
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) == "text")
                                        {
                                            TextVariables[IndexOf(VarName)] = TextVariables[IndexOf(Code[Line])];
                                        }
                                        else if (Code[Line] == "waitinput")
                                        {
                                            TextVariables[IndexOf(VarName)] = Console.ReadLine();
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }

                                    Line += 2;
                                    break;
                                case "boolean":
                                    Line++;
                                    bool LineBoolean;
                                    bool LineIsBoolean = bool.TryParse(Code[Line], out LineBoolean);

                                    if (LineIsBoolean)
                                    {
                                        BooleanVariables[IndexOf(VarName)] = Convert.ToBoolean(Code[Line]);
                                    }
                                    else
                                    {
                                        if (GetVarType(Code[Line]) == "boolean")
                                        {
                                            BooleanVariables[IndexOf(VarName)] = BooleanVariables[IndexOf(Code[Line])];
                                        }
                                        else
                                        {
                                            Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. Invalid data type or variable does not exist in current context.");
                                            End();
                                        }
                                    }

                                    Line += 2;
                                    break;
                                default:
                                    Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line]} <-- Compile-Time error. {VarName} does not exist in current context.");
                                    End();
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{this.Code.Count - Code.Count + Line} > {Code[Line].Replace(@"""", "")} <-- Compile-Time error. Unknown function");
                            End();
                        }
                        break;
                }
            }
        }
    }
}
