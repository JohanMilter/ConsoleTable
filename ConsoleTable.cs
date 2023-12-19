using System;

namespace ConsoleTable
{
    struct Margin
    {
        /// <summary>
        /// Set the Margin for all
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public Margin(int left, int top, int right, int bottom)
        {
            Top = top;
            Bottom = bottom;
            Right = right;
            Left = left;
        }
        /// <summary>
        /// Set the Margin for the Top and Bottom only
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        public Margin(int top, int bottom)
        {
            Top = top;
            Bottom = bottom;
        }
        public int Top { get; private set; }
        public int Bottom { get; private set; }
        public int Right { get; private set; }
        public int Left { get; private set; }
    }
    struct Item
    {
        public string? Content;
        public int X;
        public int Y;
        public ConsoleColor FontColor;
        public ConsoleColor BackColor;
    }
    struct Border
    {
        private readonly char[] _chars;
        public Border(string chars)
        {
            if (chars.Length != 12)
                throw new ArgumentException("Input string must be exactly 12 characters long.", nameof(chars));
            _chars = chars.ToCharArray();
        }
        public ConsoleColor BorderColor = ConsoleColor.Gray;
        public readonly char TopLeftChar => GetCharAtIndex(0);
        public readonly char TopRightChar => GetCharAtIndex(1);
        public readonly char BottomLeftChar => GetCharAtIndex(2);
        public readonly char BottomRightChar => GetCharAtIndex(3);
        public readonly char CrossChar => GetCharAtIndex(4);
        public readonly char VerticalChar => GetCharAtIndex(5);
        public readonly char HorisontalChar => GetCharAtIndex(6);
        public readonly char VerticalRowLeftChar => GetCharAtIndex(7);
        public readonly char VerticalRowRightChar => GetCharAtIndex(8);
        public readonly char HorisontalColumnUpChar => GetCharAtIndex(9);
        public readonly char HorisontalColumnDownChar => GetCharAtIndex(10);
        public readonly char EmptyChar => GetCharAtIndex(11);

        private readonly char GetCharAtIndex(int index)
        {
            return index >= 0 && index < _chars?.Length ? _chars[index] : '\0';
        }
    }
    enum Allignment
    {
        Left,
        Middle_Offset_Left,
        Middle_Offset_Right,
        Right,
    }
    class Table<T_type>
    {
        public Table()
        {
            Margin = new Margin(0, 0, 0, 0);
            Border = new Border("-----|----- ");
            fontColor = ConsoleColor.Gray;
            backColor = ConsoleColor.Black;
            setColor = false;
        }
        #region Public methods
        public static void ShowFullTable(ref Table<T_type> tableClass)
        {
            if (!tableClass.isBuild)
                throw new Exception("Tried 'ShowTableWithValues()' without the Table was 'Build()'");
            (int, int) pos = Console.GetCursorPosition();
            ShowStructure(ref tableClass, pos);
            ShowTable(ref tableClass, pos);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public static void ShowTable(ref Table<T_type> tableClass, (int, int)? startingPos = null)
        {
            if (!tableClass.isBuild)
                throw new Exception("Tried 'ShowTable()' without the Table was 'Build()'");
            startingPos ??= Console.GetCursorPosition();
            int X;
            int Y = startingPos.Value.Item2 + 1;
            int addExtraTop = tableClass.Margin.Top;
            int addExtraBottom = tableClass.Margin.Bottom;
            for (int row = 0; row < tableClass.table.Count; row++)
            {
                X = startingPos.Value.Item1 + 1;
                for (int col = 0; col < tableClass.table[row].Count; col++)
                {
                    Item item = tableClass.table[row][col];
                    int columnLength = 0;
                    if (tableClass.columnLengths != null)
                        columnLength = tableClass.columnLengths[col];
                    if (tableClass.setColor)
                    {
                        Console.BackgroundColor = item.BackColor;
                        Console.ForegroundColor = item.FontColor;
                    }
                    int rM = tableClass.Margin.Right;
                    int lM = tableClass.Margin.Left;
                    int half = (columnLength - (item.Content?.Length + rM + lM) ?? 0) / 2;
                    switch (tableClass.Allignment)
                    {
                        case Allignment.Left:
                            rM += columnLength - (item.Content?.Length + rM + lM) ?? 0;
                            break;
                        case Allignment.Middle_Offset_Left:
                            lM += half;
                            rM += half;
                            if (columnLength - (item.Content?.Length + rM + lM) == 1)
                                rM++;
                            break;
                        case Allignment.Middle_Offset_Right:
                            lM += half;
                            rM += half;
                            if (columnLength - (item.Content?.Length + rM + lM) == 1)
                                lM++;
                            break;
                        case Allignment.Right:
                            lM += columnLength - (item.Content?.Length + rM + lM) ?? 0;
                            break;
                    }

                    string content = item.Content ?? "";
                    content = content.PadRight(content.Length + rM, tableClass.Border.EmptyChar);
                    content = content.PadLeft(content.Length + lM, tableClass.Border.EmptyChar);
                    for (int i = 0; i < addExtraTop; i++)
                    {
                        Console.SetCursorPosition(X, Y + i);
                        Console.Write("".PadLeft(content.Length, tableClass.Border.EmptyChar));
                    }
                    Console.SetCursorPosition(X, Y + addExtraTop);
                    Console.Write($"{content}");
                    for (int i = 0; i < addExtraBottom; i++)
                    {
                        Console.SetCursorPosition(X, Y + i + addExtraTop + 1);
                        Console.Write("".PadLeft(content.Length, tableClass.Border.EmptyChar));
                    }
                    if (tableClass.columnLengths != null)
                        X += columnLength + 1;
                }
                Y += 2 + addExtraTop + addExtraBottom;
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(startingPos.Value.Item1, Y);
        }
        public static void ShowStructure(ref Table<T_type> tableClass, (int, int)? startingPos = null)
        {
            if (!tableClass.isBuild)
                throw new Exception("Tried 'ShowStructure()' without the Table was 'Build()'");
            startingPos ??= Console.GetCursorPosition();
            int X = startingPos.Value.Item1;
            int Y = startingPos.Value.Item2;

            Console.ForegroundColor = tableClass.Border.BorderColor;
            Console.SetCursorPosition(X, Y); Y++;
            Console.WriteLine(tableClass.tableStructure[0]);
            int addExtraHeight = tableClass.Margin.Top + tableClass.Margin.Bottom + 1;
            for (int i = 0; i < addExtraHeight; i++)
            {
                Console.SetCursorPosition(X, Y); Y++;
                Console.WriteLine(tableClass.tableStructure[2]);
            }
            for (int y = 0; y < tableClass.table.Count - 1; y++)
            {
                Console.SetCursorPosition(X, Y); Y++;
                Console.WriteLine(tableClass.tableStructure[1]);
                for (int i = 0; i < addExtraHeight; i++)
                {
                    Console.SetCursorPosition(X, Y); Y++;
                    Console.WriteLine(tableClass.tableStructure[2]);
                }
            }
            if (tableClass.table.Count == 1)
            {
                Console.SetCursorPosition(X, Y); Y++;
                Console.WriteLine(tableClass.tableStructure[2]);
            }
            Console.SetCursorPosition(X, Y);
            Console.WriteLine(tableClass.tableStructure[3]);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(X, Y + 1);
        }
        public void Build()
        {
            if (table.Count == 0)
                throw new Exception("Tried 'Build()' without Table content");
            SortTableByCoordinates(ref table);
            isBuild = true;
            GetColumnLengths();
            #region Top Border
            tableStructure[0] += Border.TopLeftChar;
            tableStructure[3] += Border.BottomLeftChar;
            tableStructure[1] += Border.VerticalRowLeftChar;
            tableStructure[2] += Border.VerticalChar;

            for (int x = 0; x < columnLengths?.Length; x++)
            {
                for (int i = 0; i < columnLengths?[x]; i++)
                    tableStructure[0] += Border.HorisontalChar;
                if (x != columnLengths?.Length - 1)
                    tableStructure[0] += Border.HorisontalColumnDownChar;
                #endregion

                #region Other Borders
                for (int i = 0; i < columnLengths?[x]; i++)
                    tableStructure[1] += Border.HorisontalChar;
                if (x != columnLengths?.Length - 1)
                    tableStructure[1] += Border.CrossChar;
                else
                    tableStructure[1] += Border.VerticalRowRightChar;
                for (int i = 0; i < columnLengths?[x]; i++)
                    tableStructure[2] += Border.EmptyChar;
                tableStructure[2] += Border.VerticalChar;
                #endregion

                #region Bottom Border
                //Bottom Border
                for (int i = 0; i < columnLengths?[x]; i++)
                    tableStructure[3] += Border.HorisontalChar;
                if (x != columnLengths?.Length - 1)
                    tableStructure[3] += Border.HorisontalColumnUpChar;
            }
            tableStructure[0] += Border.TopRightChar;
            tableStructure[3] += Border.BottomRightChar;
            #endregion
        }
        public void AddRow(params T_type[] items)
        {
            table.Add(new());
            for (int x = 0; x < items.Length; x++)
                table[^1].Add(new()
                {
                    Content = items[x]?.ToString(),
                    X = x,
                    Y = table.Count - 1,
                    FontColor = fontColor,
                    BackColor = backColor,
                });
        }
        public void AddRow(params Item[] items)
        {
            for (int x = 0; x < items.Length; x++)
                table[^1].Add(items[x]);
        }
        public void AddItem(bool newRow, params T_type[] items)
        {
            if (newRow) table.Add(new());
            for (int x = table[^1].Count, i = 0; i < items.Length; x++, i++)
                table[^1].Add(new()
                {
                    Content = items[i]?.ToString(),
                    X = x,
                    Y = table.Count - 1,
                    FontColor = fontColor,
                    BackColor = backColor,
                });
        }
        public void AddItem(bool newRow, params Item[] items)
        {
            if (newRow) table.Add(new());
            foreach (Item item in items)
                table[^1].Add(item);
        }
        public void SetColor(ConsoleColor textC, ConsoleColor backC)
        {
            if (!setColor) setColor = true;
            fontColor = textC;
            backColor = backC;
        }
        public List<List<Item>> GetTable()
        {
            return table;
        }
        #endregion
        #region Public objects
        public Margin Margin { get; set; }
        public Border Border { get; set; }
        public Allignment Allignment { get; set; } = Allignment.Left;
        #endregion
        #region Private methods
        private void GetColumnLengths()
        {
            var list = table.OrderByDescending(l => l.Count).FirstOrDefault();
            if (list != null)
            {
                columnLengths = new int[list.Count];
                for (int y = 0; y < table.Count; y++)
                    for (int x = 0; x < table[y].Count; x++)
                    {
                        string content = table[y][x].Content?.ToString() ?? "";
                        columnLengths[x] = columnLengths[x] < content.Length ? content.Length : columnLengths[x];
                    }
                for (int x = 0; x < columnLengths.Length; x++)
                    columnLengths[x] += Margin.Left + Margin.Right;
            }
        }
        private static void SortTableByCoordinates(ref List<List<Item>> table)
        {
            List<List<Item>> sortedTable = new();
            for (int Y = 0; Y < table.Count; Y++)
            {
                sortedTable.Add(new());
                for (int X = 0; X < table[Y].Count; X++)
                {
                    sortedTable[Y].Add(new());
                    bool breakNow = false;
                    foreach (var list in table)
                    {
                        foreach (var item in list)
                            if (X == item.X && Y == item.Y)
                            {
                                sortedTable[Y][X] = item;
                                breakNow = true;
                                break;
                            }
                        if (breakNow)
                            break;
                    }
                }
            }
            table = sortedTable;
        }
        #endregion
        #region Private Objects
        private bool setColor;
        private ConsoleColor fontColor;
        private ConsoleColor backColor;
        private List<List<Item>> table = new();
        private bool isBuild = false;
        private readonly string[] tableStructure = new string[4];
        private int[]? columnLengths;
        #endregion
    }
}
