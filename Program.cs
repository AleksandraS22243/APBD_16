using System;

public class MathHelper
{
    public static int FindMax(int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
        {
            throw new ArgumentException("Tablica liczb nie może być pusta.");
        }

        int max = numbers[0];
        foreach (int num in numbers)
        {
            if (num > max)
            {
                max = num;
            }
        }

        return max;
    }
}

class Program
{
    static void Main(string[] args)
    {
        int[] numbers = { 2, 8, 5, 3, 10 };
        int max = MathHelper.FindMax(numbers);
        Console.WriteLine("Maksymalna wartość: " + max);
    }
}
