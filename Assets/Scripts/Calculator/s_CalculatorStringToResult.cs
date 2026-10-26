using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Calculator
{
    public static class s_CalculatorStringToResult
    {
        public static double Evaluate(string expression)
        {
            List<string> tokens = Tokenize(expression);
            List<string> rpn = ConvertToRPN(tokens);
            return CalculateRPN(rpn);
        }
        private static List<string> Tokenize(string input)
        {
            List<string> tokens = new List<string>();
            StringBuilder numberBuffer = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (char.IsWhiteSpace(c)) continue;
                
                if (char.IsDigit(c) || c == '.')
                {
                    numberBuffer.Append(c);
                }
                else
                {
                    if (numberBuffer.Length > 0)
                    {
                        tokens.Add(numberBuffer.ToString());
                        numberBuffer.Clear();
                    }

                    if ("+-*/()".Contains(c))
                    {
                        tokens.Add(c.ToString());
                    }
                    else
                    {
                       Debug.LogError($"Недопустимый символ: {c}");
                    }
                }
            }

            if (numberBuffer.Length > 0)
            {
                tokens.Add(numberBuffer.ToString());
            }

            return tokens;
        }
        
        private static List<string> ConvertToRPN(List<string> tokens)
        {
            List<string> output = new List<string>();
            Stack<string> operators = new Stack<string>();

            foreach (string token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    output.Add(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        output.Add(operators.Pop());
                    }

                    if (operators.Count == 0)
                        Debug.LogError("Нарушен баланс скобок (пропущена открывающая скобка).");

                    operators.Pop();
                }
                else
                {
                    while (operators.Count > 0 && GetPriority(operators.Peek()) >= GetPriority(token))
                    {
                        output.Add(operators.Pop());
                    }
                    operators.Push(token);
                }
            }
            
            while (operators.Count > 0)
            {
                string op = operators.Pop();
                if (op == "(" || op == ")")
                    Debug.LogError("Нарушен баланс скобок (пропущена закрывающая скобка).");
                output.Add(op);
            }

            return output;
        }
        
        private static double CalculateRPN(List<string> rpn)
        {
            Stack<double> rpnStack = new Stack<double>();

            foreach (string token in rpn)
            {
                if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
                {
                    rpnStack.Push(number);
                }
                else
                {
                    if (rpnStack.Count < 2)
                        Debug.LogError("Некорректное выражение.");

                    double b = rpnStack.Pop();
                    double a = rpnStack.Pop();

                    switch (token)
                    {
                        case "+": rpnStack.Push(a + b); break;
                        case "-": rpnStack.Push(a - b); break;
                        case "*": rpnStack.Push(a * b); break;
                        case "/": 
                            if (b == 0) Debug.LogError("Деление на ноль!");
                            rpnStack.Push(a / b); 
                            break;
                        default: Debug.LogError($"Неизвестный оператор: {token}");
                            break;
                    }
                }
            }

            if (rpnStack.Count != 1)
                Debug.LogError("Некорректное выражение.");

            return rpnStack.Pop();
        }
        
        private static int GetPriority(string op)
        {
            switch (op)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }    
    }
}