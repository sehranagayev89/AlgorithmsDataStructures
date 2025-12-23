using System;
namespace Algorithms
{
    class Program
    {
        static int SecondMaxElement(int[] arr)
        {
           int max = int.MinValue;
           int secondMax = int.MinValue;
           for (int i = 0; i < arr.Length; i++)
           {
                if (arr[i] > max)
                {
                    secondMax = max;
                    max = arr[i];
                }
                else if (arr[i] > secondMax && arr[i] < max)
                {
                    secondMax = arr[i];
                }
           }
            return secondMax;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Second max element");
            int[] arr = { 11, 2, -3, 4, 5 };
            int result = SecondMaxElement(arr);
            Console.WriteLine("Second max element is: " + result);

        }
    }
}
