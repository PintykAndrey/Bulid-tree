using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp7
{
    internal class Program
    {
        static CustomList<Tree<string>> trees = new CustomList<Tree<string>>();
        static CustomList<string[]> variables = new CustomList<string[]>();
        public class CustomList<T>
        {
            private T[] items;
            private int count;
            public CustomList(int capacity = 0)
            {
                items = new T[capacity];
                count = 0;
            }
            public void Add(T item)
            {
                if (count == items.Length)
                {
                    IncreaseArraySize(ref items);
                }
                items[count] = item;
                count++;
            }
            public T this[int index]
            {
                get
                {
                    return items[index];
                }
                set
                {
                    if (index < 0 || index >= count)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    items[index] = value;
                }
            }
            public int Count
            {
                get
                {
                    return count;
                }
            }
        }
        class Tree<T>
        {
            public T Value { get;  set; }
            public Tree<T> Left { get; set; }
            public Tree<T> Right { get; set; }
            public Tree(T value)
            {
                Value = value;
                Left = null;
                Right = null;
            }
        }
        private static string LogicOparation( Tree<string> node, string[] numbers, int x)
        {
            TraverseBottomUpHelper(node, numbers, x);
            if (node.Value == "&")
            {
                if(node.Left.Value != "0" && node.Left.Value != "1")
                {
                    LogicOparation(node.Left, numbers, x);
                }
                if(node.Right.Value != "0" && node.Right.Value != "1")
                {
                    LogicOparation(node.Right, numbers, x);
                }
                node.Value =(CustomParseInt(node.Left.Value) & CustomParseInt(node.Right.Value)).ToString();
                node.Left = null;
                node.Right = null;
            }
            else if(node.Value == "|")
            {
                if (node.Left.Value != "0" && node.Left.Value != "1")
                {
                    LogicOparation(node.Left, numbers, x);
                }
                if (node.Right.Value != "0" && node.Right.Value != "1")
                {
                    LogicOparation(node.Right, numbers, x);
                }
                node.Value = (CustomParseInt(node.Left.Value) | CustomParseInt(node.Right.Value)).ToString();
                node.Left = null;
                node.Right = null;
            }
            else if (node.Value == "!")
            {
                if(node.Left != null)
                     node.Value = (CustomParseInt(node.Left.Value) ^ 1).ToString();
                else
                    node.Value = (CustomParseInt(node.Right.Value) ^ 1).ToString();
                node.Left = null;
                node.Right = null;
            }
            return node.Value;
        }
        private static void TraverseBottomUpHelper(Tree<string> node, string[] numbers,int x)
        {
            if (node == null)
                return;
            if (node.Value != "&" && node.Value != "|" && node.Value != "!")
            {
                for(int i = 0; i < variables[x].Length; i++)
                {
                    if (variables[x][i] == node.Value)
                    {
                        node.Value = numbers[i];
                        goto end;
                    }
                }
            }
            end:
                TraverseBottomUpHelper(node.Left, numbers, x);
                TraverseBottomUpHelper(node.Right, numbers, x);
        }
        private static Tree<string> Breketstee(CustomList<string> list, int start, int end, Tree<string> lefttemp, out int closebre)
        {
            Tree<string> tree = new Tree<string>(null);
            closebre = 0;
            if (list[start][0] == '(')
            {
                list[start] = CustomSubstring(list[start], 1, list[start].Length-1);
                while (list[end][list[end].Length - 1] != ')')
                {
                    end--;
                }
                closebre = end;
                list[closebre] = CustomSubstring(list[closebre], 0, list[closebre].Length - 1);
                tree=BuildTree(list, start, end, lefttemp);
            }
            return tree;
        }
        private static Tree<string> BuildTree(CustomList<string> list, int start, int end, Tree<string> lefttemp)
        {
            Tree<string> tree = new Tree<string>(null);
            var right = new Tree<string>(list[start + 2]);
            var left = new Tree<string>(list[start]);
            var root = new Tree<string>(list[start + 1]);
            tree = root;
            if (list[start][0] == '!')
            {
                left = new Tree<string>("!")
                {
                    Left = new Tree<string>(CustomSubstring(list[start], 1, list[start].Length - 1))
                };
                tree.Left = left;
            }
            else if (CustomSubstring(list[start], 0, "func".Length) == "func")
            {
                tree.Left = trees[GetNumber(list[start])];
            }
            else
            {
                if (lefttemp.Value == null || lefttemp == null)
                {
                    tree.Left = left;
                }
                else
                {
                    tree.Left = lefttemp;
                }
            }
            if (list[start + 2][0] == '!')
            {
                right = new Tree<string>("!");
                right.Right = new Tree<string>(CustomSubstring(list[start + 2], 1, list[start + 2].Length - 1));
                tree.Right = right;
            }
            else if (CustomSubstring(list[start + 2], 0, "func".Length) == "func")
            {
                tree.Right = trees[GetNumber(list[start + 2])];
            }
            else if (list[start+2][0] == '(')
            {
                tree.Right = Breketstee(list,start+2, end, lefttemp, out int closebre);
                start = closebre-2;
            }
            else
            {
                tree.Right = right;
            }

            start += 2;
            if (start < end)
            {
                tree = BuildTree(list, start, end, tree);
            }
            return tree;
        }
        public static string[] CustomSplit(string input, char symbol)
        {
            string[] words = new string[0];
            int start = 0, j = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (i>0 && symbol == ' ' && input[i] == symbol && input[i-1] == ',') 
                { 
                }
                else if (input[i] == symbol)
                {
                    IncreaseArraySize(ref words);
                    words[j] = (CustomSubstring(input, start, i - start));
                    j++;
                    start = i + 1;
                }
            }
            IncreaseArraySize(ref words);
            words[words.Length - 1] = (CustomSubstring(input, start, input.Length - start));

            return words;
        }
        public static void IncreaseArraySize<T>(ref T[] arr)
        {
            T[] newArr = new T[arr.Length + 1];
            for (int i = 0; i < arr.Length; i++)
                newArr[i] = arr[i];
            arr = newArr;
        }
        public static int CustomParseInt(string s)
        {
            int result = 0;
            foreach (char c in s)
            {
                if (c >= '0' && c <= '9')
                {
                    result = result * 10 + (c - '0');
                }
            }
            return result;
        }
        public static string CustomSubstring(string input, int startIndex, int length)
        {
            int endIndex = startIndex + length;
            if (endIndex > input.Length)
                endIndex = input.Length;
            string result = "";
            for (int i = startIndex; i < endIndex; i++)
                result += input[i];
            return result;
        }
        public static string Breckets(string word)
        {
            int startindex = 0, endindex = 0;
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == '(')
                {
                    startindex = i;
                }
                if (word[i] == ')')
                {
                    endindex = i;
                }
            }
           return CustomSubstring(word, startindex+1, endindex-startindex-1);
        }
        public static bool IsValue(string[] value, string words)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (words == value[i])
                {
                    return true;
                }
            }
            return false;
        }
        public static int GetNumber(string inputString)
        {
            string[] words = CustomSplit(inputString, '(');
            return CustomParseInt(CustomSubstring(words[0], 4, words[0].Length - 4));
        }
        public static void Define(string[] words)
        {
            while (trees.Count <= GetNumber(words[1]))
            {
                trees.Add(new Tree<string>(null));
                variables.Add(null);
            }
            CustomList<string> operations = new CustomList<string>();
            var bre = Breckets(words[1]);
            var variable = CustomSplit(bre, ',');
            variables[GetNumber(words[1])] = variable;
            for(int i = 0; i < variable.Length; i++)
            {
                if (variable[i][0] == ' ')
                {
                    variable[i] = CustomSubstring(variable[i], 1, variable[i].Length - 1);
                }
            }
            for(int i = 2; i < words.Length; i++)
            {
                if (words[i][0] == '"')
                {
                    words[i] = CustomSubstring(words[i], 1, words[i].Length - 1);
                }
                else if (words[i][words[i].Length - 1] == '"')
                {
                    words[i] = CustomSubstring(words[i], 0, words[i].Length - 1);
                }
            } 
            for(int i = 2; i < words.Length; i++)
            {
                operations.Add(words[i]);
                if (CustomSubstring(words[i], 0, "func".Length) == "func")
                {
                    if (trees[GetNumber(words[i])].Value == null)
                    {
                        Console.WriteLine($"{CustomSubstring(words[i], 0, "funcX".Length)} wasn't define");
                        return;
                    }
                    var temp = CustomSplit(Breckets(words[i]), ',');
                    if (temp.Length != variables[GetNumber(words[i])].Length)
                        {
                            Console.WriteLine($"func{GetNumber(words[i])} must contain {variables[GetNumber(words[i])].Length} variables");
                        } 
                }
                else if (words[i][0] == '!')
                {
                    if (!IsValue(variable, CustomSubstring(words[i], 1, words[i].Length - 1)))
                    {
                        Console.WriteLine($"{CustomSubstring(words[i], 1, words[i].Length - 1)} wasn't define");
                        return;
                    }
                }else if (words[i][0] == '(')
                {
                    if (!IsValue(variable, CustomSubstring(words[i], 1, words[i].Length - 1)))
                    {
                        Console.WriteLine($"{CustomSubstring(words[i], 1, words[i].Length - 1)} wasn't define");
                        return;
                    }
                }
                else if (words[i][words[i].Length - 1] == ')')
                {
                    if (!IsValue(variable, CustomSubstring(words[i], words[i].Length-2, 1)))
                    {
                        Console.WriteLine($"{CustomSubstring(words[i], words[i].Length-2, 1)} wasn't define");
                        return;
                    }

                }
                else if (!IsValue(variable, words[i]) && words[i] != "&" && words[i] != "|")
                {
                    Console.WriteLine($"{words[i]} wasn't define");
                    return;
                }
            }
            trees[GetNumber(words[1])] = BuildTree(operations, 0, operations.Count-1, new Tree<string>(null));
        }
        private static Tree<string> CloneTree(Tree<string> node)
        {
            if (node == null)
                return null;
            Tree<string> clone = new Tree<string>(node.Value);
            clone.Left = CloneTree(node.Left);
            clone.Right = CloneTree(node.Right);
            return clone;
        }
        public static void Solve(string func)
        {
            var numbers = CustomSplit(Breckets(func), ',');
            Tree<string> initialTree = trees[GetNumber(func)];
            Tree<string> clonedTree = CloneTree(initialTree);
            Console.WriteLine($"-> Result:{LogicOparation( clonedTree, numbers, GetNumber(func))}");
        }
        public static void All(string text)
        {
            for(int i = 0; i < variables[GetNumber(text)].Length; i++)
            {
                Console.Write($"{variables[GetNumber(text)][i]} ");
            }
            Console.WriteLine($": func{GetNumber(text)}");
            var list = Combination(text);
            var temp = text;
            for (int i = 0; i < list.Count; i++)
            {
                Console.Write("   ");
                temp = text;
                temp += "(";
                for (int j = 0; j < list[i].Count; j++)
                {
                    if (j == list[i].Count - 1)
                    {
                        Console.Write($"{list[i][j]} : ");
                        temp += $"{list[i][j]})";
                        goto End;
                    }
                    Console.Write($"{list[i][j]} , ");
                    temp += $"{list[i][j]}, ";
                }
            End:
                Solve(temp);
            }
        }
        public static CustomList<CustomList<string>> Combination(string text)
        {
            CustomList<CustomList<string>> comb = new CustomList<CustomList<string>>();
            int n = variables[CustomParseInt(text)].Length;
            for (int i = 0; i < (1 << n); i++)
            {
                CustomList<string> combination = new CustomList<string>();
                for (int j = 0; j < n; j++)
                {
                    combination.Add(((i & (1 << j)) > 0 ? 1 : 0).ToString());
                }
                comb.Add(combination);
            }
            return comb;
        }
        static void Main(string[] args)
        {
            while (true)
            { 
                var text = Console.ReadLine();
                var words = CustomSplit(text,' ');
                switch (words[0])
                {
                    case "DEFINE":
                        Define(words);
                        break;
                    case "SOLVE":
                        Solve(words[1]);
                        break;
                    case "ALL":
                        All(words[1]);
                        break;  
                    default:
                        Console.WriteLine("Error");
                        break;
                }
            }
        }
    }
}
