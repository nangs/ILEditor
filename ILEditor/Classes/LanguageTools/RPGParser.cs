﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILEditor.Classes.LanguageTools
{
    class RPGParser
    {
        private static string GetTypeFromToken(RPGToken Token)
        {
            string result = "";
            if (Token.Value == "LIKE")
            {
                if (Token.Block != null)
                    if (Token.Block.Count > 0)
                        result = Token.Block[0].Value;
            }
            else
            {
                result = char.ToUpper(Token.Value[0]) + Token.Value.Substring(1).ToLower();
                if (Token.Block != null)
                {
                    result += "(";
                    foreach (RPGToken token in Token.Block)
                        result += token.Value;
                    result += ")";
                }
            }

            return result;
        }

        public static Function[] Parse(string Code)
        {
            RPGLexer lexer;
            List<RPGToken> Tokens;
            List<Function> Procedures = new List<Function>();
            RPGToken token;
            int line = -1;
            string CurrentLine, Type;

            Function CurrentProcedure = new Function("Globals", 0);
            Variable CurrentStruct = null;

            foreach (string Line in Code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                line++;
                CurrentLine = Line.Trim();
                if (CurrentLine == "") continue;

                lexer = new RPGLexer();
                lexer.Lex(CurrentLine);
                Tokens = lexer.GetTokens();

                if (Tokens.Count == 0) continue;

                token = Tokens[0];
                switch (token.Type)
                {
                    case RPGLexer.Type.OPERATION:
                        switch (token.Value)
                        {
                            case "DCL-F":
                                if (Tokens.Count < 2) break;
                                CurrentProcedure.AddVariable(new Variable(Tokens[1].Value, "File", StorageType.File, line));
                                break;
                            case "DCL-S":
                                if (Tokens.Count < 3) break;
                                Type = GetTypeFromToken(Tokens[2]);
                                CurrentProcedure.AddVariable(new Variable(Tokens[1].Value, Type, StorageType.Normal, line));
                                break;
                            case "DCL-C":
                                if (Tokens.Count < 3) break;
                                CurrentProcedure.AddVariable(new Variable(Tokens[1].Value, Tokens[2].Value, StorageType.Const, line));
                                break;
                            case "DCL-DS":
                                if (Tokens.Count < 2) break;
                                CurrentStruct = new Variable(Tokens[1].Value, "Data-Structure", StorageType.Struct, line);

                                if (Tokens.Count < 3) break;
                                for (int x = 2; x < Tokens.Count; x++)
                                {
                                    if (Tokens[x].Value == "LIKEDS")
                                    {
                                        CurrentProcedure.AddVariable(CurrentStruct);
                                        CurrentStruct = null;
                                    }
                                }
                                
                                break;
                            case "DCL-PR":
                            case "DCL-PI":
                                if (Tokens.Count < 2) break;
                                if (Tokens[1].Value == "*N")
                                    Tokens[1].Value = "";

                                switch (token.Value)
                                {
                                    case "DCL-PR":
                                        CurrentStruct = new Variable(Tokens[1].Value, "Prototype", StorageType.Prototype, line);
                                        break;
                                    case "DCL-PI":
                                        CurrentStruct = new Variable(Tokens[1].Value, "Parameters", StorageType.Struct, line);
                                        break;
                                }

                                if (Tokens.Count < 3) break;
                                if (Tokens[2].Value == ";") break;
                                if (Tokens[2].Value.StartsWith("END"))
                                    CurrentStruct = null;
                                else
                                {
                                    Type = GetTypeFromToken(Tokens[2]);
                                    if (CurrentStruct.GetStorageType() != StorageType.Prototype)
                                    {
                                        CurrentProcedure.SetReturnType(Type);
                                    }
                                    else
                                    {
                                        if (CurrentStruct.GetStorageType() == StorageType.Prototype)
                                            CurrentStruct.SetType(Type);
                                    }

                                }

                                if (Tokens.Count < 4) break;
                                if (Tokens[3].Value == ";") break;
                                if (Tokens[2].Value.StartsWith("END"))
                                    CurrentStruct = null;
                                break;

                            case "DCL-SUBF":
                            case "DCL-PARM":
                                if (Tokens.Count < 3) break;
                                Type = GetTypeFromToken(Tokens[2]);
                                CurrentStruct.AddMember(new Variable(Tokens[1].Value, Type, StorageType.Normal, line));
                                break;
                            case "END-PR":
                            case "END-PI":
                            case "END-DS":
                                CurrentProcedure.AddVariable(CurrentStruct);
                                CurrentStruct = null;
                                break;
                            case "BEGSR":
                                if (Tokens.Count < 2) break;
                                CurrentProcedure.AddVariable(new Variable(Tokens[1].Value, "Subroutine", StorageType.Subroutine, line));
                                break;
                            case "DCL-PROC":
                                if (Tokens.Count < 2) break;
                                Procedures.Add(CurrentProcedure);
                                CurrentProcedure = new Function(Tokens[1].Value, line);
                                break;
                        }
                        break;

                    default:
                        if (CurrentStruct != null)
                        {
                            if (Tokens.Count < 2) break;

                            Type = GetTypeFromToken(Tokens[1]);
                            CurrentStruct.AddMember(new Variable(Tokens[0].Value, Type, StorageType.Normal, line));
                        }
                        break;
                }
            }

            Procedures.Add(CurrentProcedure);

            return Procedures.ToArray();
        }
    }
}
