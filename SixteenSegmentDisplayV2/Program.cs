using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SixteenSegmentDisplayV2
{
    class Program
    {
        static void Main(string[] args)
        {
            string txt;
            int[] segments;

            Console.WriteLine("Enter a text:");
            Console.WriteLine("(Printable characters: []\"#$%&'()*+,-/0123456789|;<=>?@AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz[\\]^_`{}):");
            txt = Console.ReadLine();

            SixteenSegmentDisplay display = new SixteenSegmentDisplay(60, 2, 5);
            foreach(char c in txt.AsEnumerable())
            {
                if (display.IsAllowedCharacter(c))
                {
                    segments = display.EncodeAsSixteenSegmentsVector(c);
                    display.PrintVector(segments);
                    display.SaveSegmentsLocation(segments);
                }
                else
                    Console.WriteLine($"Error: This character ({c}) is not allowed/supported!");
            }

            Thread.Sleep(3500);
            Console.Clear();
            display.PrintScrollingSegments();
            Console.WriteLine();
            Console.Write("Press any key to close the app");
            Console.ReadKey();
        }
    }
}
