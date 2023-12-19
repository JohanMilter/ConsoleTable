
using ConsoleTable;


class Program
{
    static void Main()
    {
        Table<string> table = new()
        {
            Border = new("╔╗╚╝╬║═╠╣╩╦ ") { BorderColor = ConsoleColor.Blue, },
            Margin = new(2, 0, 2, 0),
            Allignment = Allignment.Middle_Offset_Right,
        };
        table.SetColor(ConsoleColor.Magenta, ConsoleColor.Green);
        table.AddRow("Frederik", "Mathilde", "Søren", "Emilie", "Anders", "Sofie");
        table.SetColor(ConsoleColor.Black, ConsoleColor.Yellow);
        table.AddRow("Rasmus", "Ida", "Magnus", "Freja", "Nikolaj", "Karoline");
        table.AddRow("Mikkel", "Laura", "Jesper", "Anne", "Mads", "Emma");
        table.AddRow("Henrik", "Clara", "Christian", "Lærke", "Tobias", "Amalie");
        table.AddRow("Kasper", "Astrid", "Simon", "Olivia", "Jonas", "Mia");
        table.Build();
        Console.SetCursorPosition(0, 0);
        Table<string>.ShowFullTable(ref table);
    }
}
