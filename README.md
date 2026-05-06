# InkscapeGroupPrinter

## Overview
This is a C# console application that reads through an SVG document and exports the file's group hierarchy as a visual tree to a text document.

## Problem synopsis and goals
When using SVG graphics applications (e.g., Inkscape), document elements are typically labelled with automatically generated IDs, which can make organising and cleaning up elements tricky. The main goal of this application is to provide an automated audit of a document's structure, allowing users to see, for example, if items are properly labeled, there are no empty groups, and if the item layering hierarchy is accurate.

## Features
- Recursive formatting: goes through nested and child elements to create the full hierarchy
- Batch processing: folders with multiple SVG files can be parsed in one go
- Element labelling: each element's type is outputted alongside the element name or ID

## Tech stack used
- C# (language)
- .NET 10.0 (.NET framework)
- System.Xml.Linq (for parsing elements)

## Demos

### File export
https://github.com/user-attachments/assets/e6adfd62-45e7-4cc7-bc88-ec34c55037c7



### Folder export
https://github.com/user-attachments/assets/cc296704-9a42-4c96-8e72-4a288df3056a

### How to run
1. Clone or download the repo
2. Open the project in an IDE, e.g. Visual Studio, or navigate to the folder via command line (VS 2026 /.NET 10.0 is required)
3. Click the Start or Start Without Debugging button (Visual Studio), or type `dotnet run` to start the application
