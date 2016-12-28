using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ConsoleFormatExample
{
    public class ConsoleFormatExampleUsage
    {
        private readonly int ConsoleWidth = 80;
        private readonly int ConsoleHeight = 18;

        private ConsoleFormat ConsoleFormat;

        // All named regions that will be rendered.
        private enum ConsoleRegionNames
        {
            Title,
            TimeStarted,
            CurrentRuntime,
            BounceAnimation,
            SpinnerLeft,
            SpinnerRight,
        }

        // These regions span the entire row. The int is which row the region is on.
        private Dictionary<string, int> ConsoleRegionRows = new Dictionary<string, int>()
        {
            { Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.Title),            1 },
            { Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.TimeStarted),      3 },
            { Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.CurrentRuntime),   4 },
        };

        // Bounce specific variables
        private bool BounceRight = true;
        private LinkedList<char> BouncingText = new LinkedList<char>(
            ("Bounce Example                                                              ").ToCharArray());

        // Spinner specific example
        private LinkedList<char> LeftSpinnerText = new LinkedList<char>(("\\-/|").ToCharArray());
        private LinkedList<char> RightSpinnerText = new LinkedList<char>(("|/-\\").ToCharArray());

        private DateTime StartTime;

        public ConsoleFormatExampleUsage()
        {
            ConsoleFormat = new ConsoleFormat(ConsoleWidth, ConsoleHeight);

            // Easy/quick way to define all rows.
            foreach (string consoleRegionName in ConsoleRegionRows.Keys)
            {
                ConsoleFormat.DefineRegion(consoleRegionName, 0, ConsoleRegionRows[consoleRegionName], ConsoleWidth, 1);
            }

            // Defines manually placed regions.
            ConsoleFormat.DefineRegion(Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.SpinnerLeft), x: 1, y: 16, width: 1, height: 1);
            ConsoleFormat.DefineRegion(Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.BounceAnimation), x: 2, y: 16, width: 75, height: 1);
            ConsoleFormat.DefineRegion(Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.SpinnerRight), x: 78, y: 15, width: 1, height: 1);

            ConsoleFormat.PrintToRegion(Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.Title),
                "                            Console Format Example Program                      ", ConsoleColor.Cyan);

            StartTime = DateTime.Now;

            // RuntimeTimer will drive the animations and will act as our update loop.
            Timer RuntimeTimer = new Timer(250);
            RuntimeTimer.Elapsed += RuntimeTimer_Elapsed;
            RuntimeTimer.Start();
            ConsoleFormat.PrintToRegion(Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.TimeStarted), " Time Started: " + StartTime.ToString());
        }

        private void RuntimeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Current Runtime
            ConsoleFormat.PrintToRegion((Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.CurrentRuntime)), " Current Runtime: " + DateTime.Now.Subtract(StartTime).ToString(), ConsoleColor.White);

            // Spinners
            ConsoleFormat.PrintToRegion((Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.SpinnerLeft)), LeftSpinnerText.First().ToString(), ConsoleColor.Cyan);
            ConsoleFormat.PrintToRegion((Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.SpinnerRight)), RightSpinnerText.First().ToString(), ConsoleColor.Cyan);

            // Rotate LeftSpinnerText
            char firstLeftSpinnerChar = LeftSpinnerText.First();
            LeftSpinnerText.RemoveFirst();
            LeftSpinnerText.AddLast(firstLeftSpinnerChar);

            // Rotate RightSpinnerText
            char firstRightSpinnerChar = RightSpinnerText.First();
            RightSpinnerText.RemoveFirst();
            RightSpinnerText.AddLast(firstRightSpinnerChar);

            // BouncingText
            if (BounceRight)
            {
                // Shift right (until 'e' is in Last)
                char lastScrollingChar = BouncingText.Last();
                BouncingText.RemoveLast();
                BouncingText.AddFirst(lastScrollingChar);

                if (BouncingText.Last.Value.Equals('e')) // "Bounce Example"
                                                         //               ^
                {
                    BounceRight = false;

                    // Right side hit, so reverse its spin direction.
                    RightSpinnerText = new LinkedList<char>(RightSpinnerText.Reverse());
                }
            }
            else // if (BounceRight == false)
            {
                // Shift left (until 'B' is in First)
                char firstScrollingChar = BouncingText.First();
                BouncingText.RemoveFirst();
                BouncingText.AddLast(firstScrollingChar);

                if (BouncingText.First.Value.Equals('B')) // "Bounce Example"
                                                          //  ^                
                {
                    BounceRight = true;

                    // Left side hit, so reverse its spin direction.
                    LeftSpinnerText = new LinkedList<char>(LeftSpinnerText.Reverse());
                }
            }

            string processingText = "";
            LinkedListNode<char> currentNode = BouncingText.First;
            do
            {
                processingText += currentNode.Value;
                currentNode = currentNode.Next;
            } while (currentNode != null);

            ConsoleFormat.PrintToRegion((Enum.GetName(typeof(ConsoleRegionNames), ConsoleRegionNames.BounceAnimation)), processingText, ConsoleColor.Magenta);
        }
    }
}
