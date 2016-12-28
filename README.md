# ConsoleFormat
A simple C# console formatter for console projects.

To use, simply drop the ConsoleFormat.cs file into your project and configure the ConsoleFormat object to your liking.

ConsoleFormat is meant as a full replacement for all console output.

An example of how to use ConsoleFormat is provided in the file ConsoleFormatExampleUsage.cs. It contains a static title, a start time display that has a color change registered, an elapsed time display, two 1-character spinners, and some scrolling text that bounces between the spinners.

## How it works
ConsoleFormat works by defining independent "Regions" that paint and update individually. This allows users to create static/non-scrolling screens without having the "flashing effect" from repainting the whole console window. Only the regions that get updated will be repainted.

Regions are named and referred to by a simple string ID called a "tag". Regions are defined with a simple x/y coordnate and a height/width.

Users may also register a region to change its color after some timeout. A useage example for this is painting a region green when the information is updated and registering a color change back to grey a few seconds later. This will highlight regions that were updated in the last few seconds.
