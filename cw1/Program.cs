using System;

public class Math
{
    public static double Costam(int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
        {
            throw new ArgumentException("Tablica liczb nie może być pusta.");
        }

        int sum = 0;
        foreach (int num in numbers)
        {
            sum += num;
        }

        return (double)sum / numbers.Length;
    }
}

class Program
{
    static void Main(string[] args)
    {

        int[] numbers = { 2, 4, 6, 8, 10 };
        double average = Math.Costam(numbers);
        Console.WriteLine("Średnia wynosi: " + average);
    }
}
