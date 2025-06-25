var input = Convert.ToInt32(Console.ReadLine());

for (int i = 1; i <= input; i++)
{
    for (int j = 0; j <= i - 1; j++)
    {
        Console.Write("*");
    }
    Console.WriteLine();
}

Console.ReadKey();