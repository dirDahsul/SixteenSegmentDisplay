using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SixteenSegmentDisplayV2
{
    class SixteenSegmentDisplay
    {
        // Coordinates where the * symbol should be displayed
        private class Coordinates
        {
            // X and Y coordinates
            public int x { get; set; }
            public int y { get; set; }

            // Just the constructor
            public Coordinates(int tx, int ty)
            {
                x = tx;
                y = ty;
            }
        }

        // The sixteen segment display mapping
        // Static cuz every sixteen segment display is using the same mapping ... obviously
        private static readonly int[,] usageMap = new int[,]
        {
            { 0, 1, 1, 1, 0, 2, 2, 2, 0 },
            { 8,11, 0, 0,12, 0, 0,13, 3 },
            { 8, 0,11, 0,12, 0,13, 0, 3 },
            { 8, 0, 0,11,12,13, 0, 0, 3 },
            { 0, 9, 9, 9, 0,10,10,10, 0 },
            { 7, 0, 0,14,15,16, 0, 0, 4 },
            { 7, 0,14, 0,15, 0,16, 0, 4 },
            { 7,14, 0, 0,15, 0, 0,16, 4 },
            { 0, 6, 6, 6, 0, 5, 5, 5, 0 }
        };

        // How the sixteen segment mapping will be accessed .... obviously
        private static readonly Dictionary<char, int[]> segmentMap = new Dictionary<char, int[]>
        {
            // 0 - don't activate segment at this position | 1 - activate segment
            { '"', new int[] { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 } },
            { '#', new int[] { 0, 0, 1, 1, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0 } },
            { '$', new int[] { 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 0, 1, 0 } },
            { '%', new int[] { 1, 0, 0, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1, 1, 0 } },
            { '&', new int[] { 1, 0, 0, 0, 1, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1 } },
            { '\'', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 } },
            { '(', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1 } },
            { ')', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0 } },
            { '*', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1 } },
            { '+', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0 } },
            { ',', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 } },
            { '-', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '/', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 } },
            { '0', new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0 } },
            { '1', new int[] { 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 } },
            { '2', new int[] { 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '3', new int[] { 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '4', new int[] { 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '5', new int[] { 1, 1, 0, 0, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1 } },
            { '6', new int[] { 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '7', new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 } },
            { '8', new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '9', new int[] { 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '|', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 } },
            { ';', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 } },
            { '<', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1 } },
            { '=', new int[] { 0, 0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { '>', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0 } },
            { '?', new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 } },
            { '@', new int[] { 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0 } },
            { 'A', new int[] { 1, 1, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { 'B', new int[] { 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0 } },
            { 'C', new int[] { 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 'D', new int[] { 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 } },
            { 'E', new int[] { 1, 1, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 } },
            { 'F', new int[] { 1, 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 } },
            { 'G', new int[] { 1, 1, 0, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0 } },
            { 'H', new int[] { 0, 0, 1, 1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { 'I', new int[] { 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 } },
            { 'J', new int[] { 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 'K', new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1 } },
            { 'L', new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 'M', new int[] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0 } },
            { 'N', new int[] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 0, 1 } },
            { 'O', new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 'P', new int[] { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { 'Q', new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1 } },
            { 'R', new int[] { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1 } },
            { 'S', new int[] { 1, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { 'T', new int[] { 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 } },
            { 'U', new int[] { 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 'V', new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 1, 0, 0 } },
            { 'W', new int[] { 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1 } },
            { 'X', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 } },
            { 'Y', new int[] { 0, 0, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 } },
            { 'Z', new int[] { 1, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 } },
            { '[', new int[] { 1, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { '\\', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 } },
            { ']', new int[] { 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { '^', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1 } },
            { '_', new int[] { 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { '`', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 } },
            { 'a', new int[] { 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0 } },
            { 'b', new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0 } },
            { 'c', new int[] { 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 } },
            { 'd', new int[] { 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 } },
            { 'e', new int[] { 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1, 0, 0 } },
            { 'f', new int[] { 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 1, 0 } },
            { 'g', new int[] { 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0 } },
            { 'h', new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0 } },
            { 'i', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 } },
            { 'j', new int[] { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 0, 1, 0 } },
            { 'k', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 1 } },
            { 'l', new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 } },
            { 'm', new int[] { 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 0 } },
            { 'n', new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0 } },
            { 'o', new int[] { 0, 0, 0, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0 } },
            { 'p', new int[] { 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0 } },
            { 'q', new int[] { 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 0 } },
            { 'r', new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0 } },
            { 's', new int[] { 1, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 0, 0, 0, 1, 0 } },
            { 't', new int[] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 } },
            { 'u', new int[] { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0 } },
            { 'v', new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0 } },
            { 'w', new int[] { 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1 } },
            { 'x', new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1 } },
            { 'y', new int[] { 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0 } },
            { 'z', new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0 } },
            { '{', new int[] { 0, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 0 } },
            { '}', new int[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0 } }
        };

        private List<Coordinates> pos; // List of all the coordinates and their positions, literally acts as a buffer
        private readonly int MaxColumnsOnScreen; // The maximum number of columns that the screen could display ... obviously
        private readonly int SpacingBetweenEachSegment; // The distance between each segment
        private readonly int StartingYPosition;

        // Constructor ... just a constructor
        public SixteenSegmentDisplay(int mcos = 60, int sbes = 2, int syp = 5)
        {
            pos = new List<Coordinates>();
            MaxColumnsOnScreen = mcos;
            SpacingBetweenEachSegment = sbes;
            StartingYPosition = syp;
        }

        // Try to parse character to sixteen segment display if character is registered, ofcourse
        public int[] EncodeAsSixteenSegmentsVector(char c)
        {
            if (segmentMap.TryGetValue(c, out int[] segments))
            {
                return segments;
            }
            else
            {
                throw new ArgumentException($"Character '{c}' is not supported by the display.");
            }
        }

        // Whether a certain character is allowed
        public bool IsAllowedCharacter(char c)
        {
            return segmentMap.ContainsKey(c);
        }

        // Print the code of the segment ... if you want to
        public void PrintVector(int[] segments)
        {
            Console.Write("SEGMENTS ENCODING: ");
            foreach (int i in segments)
                Console.Write(i);
            Console.WriteLine();
        }

        // Set the cursor position and write there
        private void WriteAt(string txt, int X, int Y)
        {
            Console.SetCursorPosition(X, Y);
            Console.Write(txt);
        }

        // How the segments should be saved to the coordinates
        public void SaveSegmentsLocation(int[] segments)
        {
            // Acess column
            for (int i = 0; i < 9; i++)
            {
                // Access row
                for (int j = 0; j < 9; j++)
                {
                    // If the selected mapp element stores a 0 .. return
                    if (usageMap[j, i] == 0)
                        continue;

                    // If the specific segment of the mapp is enabled .. then create a coordinate on that position
                    if (segments[usageMap[j, i] - 1] == 1)
                    {
                        // Add the coordinate to a list
                        pos.Add(new Coordinates(MaxColumnsOnScreen, StartingYPosition + j));
                    }
                }
                pos.Add(null); // null indicates end of column
            }
            for (int i = 0; i < SpacingBetweenEachSegment; i++) // How much spacing between each segments there should be
                pos.Add(null); // null indicates end of column
        }

        // Print the segments and scroll them
        public void PrintScrollingSegments()
        {
            int num = -1; // How many elements should be 'processed'
            List<Coordinates> ProcessList = new List<Coordinates>(); // The list of elements in process (the 'process cycle')

            // Forever cycle ... cuz ... why not scroll the text forever?
            while (true)
            {
                // Don't add elements to the 'process cycle' if all elements have already been added
                if (num < (pos.Count - 1))
                {
                    num++; // Skip the null(spacing/end of column) element
                    while (pos[num] != null) // Add elements to the process cycle till we reach the end of column element
                        ProcessList.Add(pos[num++]);
                }

                // Process all elements stored in the 'process cycle'
                for (int i = 0; i < ProcessList.Count; i++)
                {
                    // If the '*' has scrolled 'outside of the screen'
                    if (ProcessList[i].x == 0)
                    {
                        ProcessList[i].x = MaxColumnsOnScreen; // Set the X coordinate of the 'out of screen' element to the 'max columns on screen(default)' size, so it's cycle may continue again later
                        WriteAt(" ", 1, ProcessList[i].y); // Delete the printed '*' of the 'out of screen' element
                        ProcessList.Remove(ProcessList[i--]); // Remove the element from the 'process cycle'
                                                              // And since Lists automatically 'rearange' themselves to keep integrity, decrease i by 1
                    }
                    // If the '*' still hasn't 'scrolled outside of screen'
                    else
                    {
                        // Write '*' at element position ... and delete it's old? position
                        WriteAt("*", ProcessList[i].x, ProcessList[i].y);
                        WriteAt(" ", ProcessList[i].x + 1, ProcessList[i].y);
                        ProcessList[i].x--; // Scroll by 1 to the left side (for the next cycle)
                    }
                }

                // All elements have been added to the 'process cycle' and the 'process cycle' is empty =====> All elements have been scrolled 'outside of screen'
                if ((num == pos.Count - 1) && ProcessList.Count == 0)
                    num = -1; // Reset the number of elements that should be 'processed', thereby 'resetting' the scrolling

                Thread.Sleep(100); // Stop for 100 miliseconds
            }
        }
    }
}
