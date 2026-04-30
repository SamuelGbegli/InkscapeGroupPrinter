using InkscapeGroupPrinter;
using System.Text;
using System.Xml.Linq;

namespace InkscapeGroupPrinter
{
    internal class Program
    {
        /// <summary>
        /// Stores the Inkscape namespace used for finding labelled groups.
        /// </summary>
        static XNamespace inkscapeNamespace = "http://www.inkscape.org/namespaces/inkscape";
        static void Main(string[] args)
        {

            Console.WriteLine("Inkscape SVG tree printer\n");

            // Section to get user input for a folder or SVG file
            Console.WriteLine("Enter the path to a folder or SVG file:");
            string path = Console.ReadLine();
            // Section if a file or folder cannot be found, or a non-SVG file is given
            while (string.IsNullOrEmpty(path) || (!Directory.Exists(path) && !File.Exists(path)) || (File.Exists(path) && !path.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Invalid path. Please enter a valid folder or SVG file path:");
                path = Console.ReadLine();
            }

            // Section to get user input for output path
            Console.WriteLine("Choose the path to write output to:");
            string outputPath = Console.ReadLine();
            // Section if a valid folder cannot be foun
            while (string.IsNullOrEmpty(outputPath) || (!Directory.Exists(outputPath)))
            {
                Console.WriteLine("Invalid path. Please enter a valid folder or file path:");
                outputPath = Console.ReadLine();
            }

            // Processes the SVG file or all SVG files in the folder, writing output to a text file in the output path
            if (File.Exists(path))
            {
                ProcessSVG(path, outputPath);
            }
            else if (Directory.Exists(path))
            {
                var svgFiles = Directory.GetFiles(path, "*.svg");
                Console.WriteLine($"Found {svgFiles.Length} SVG files in the directory. Processing...");

                foreach (var svgFile in svgFiles)
                {
                    ProcessSVG(svgFile, outputPath);
                    Task.Delay(500).Wait();
                }
            }
            // End of program
            Console.WriteLine("Processing complete. Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Processes an SVG file and writes its tree structure to a text file.
        /// </summary>
        /// <param name="svgPath">The path the SVG file is in.</param>
        /// <param name="outputPath">The path to write the output text file to.</param>
        static void ProcessSVG(string svgPath, string outputPath)
        {
            try
            {
                Console.WriteLine($"Processing file: {Path.GetFileName(svgPath)}");
                // Loads SVG document
                XDocument doc = XDocument.Load(svgPath);
                // Gets the root element of the SVG document
                XElement root = doc.Root;
                // Looks for all elements in the root that are groups (g elements)
                var groups = root.Elements().Where(e => e.Name.LocalName == "g");

                // Stores text output of the SVG tree, starting with the name of the SVG file
                var output = new StringBuilder($"SVG tree for \"{Path.GetFileName(svgPath)}\"\n\n");

                // Sets the document root as the master element, with the name of the SVG file as its label and the element type as "svg"
                var masterElement = new SVGElement
                {
                    Label = Path.GetFileNameWithoutExtension(svgPath),
                    ElementType = root.Name.LocalName
                };

                // Loops through each group element and recursively adds element to parent element
                for (int i = 0; i < groups.Count(); i++)
                {
                    bool last = i == groups.Count() - 1;
                    XElement group = groups.Reverse().ElementAt(i);
                    masterElement.ChildElements.Add(GetElements(group));
                }

                // Converts the master element and child elements into a .txt file representing the tree structure of the SVG document
                output.Append(PrintSVGElement(masterElement, new List<bool> { false }, true));
                using (StreamWriter writer = new StreamWriter(Path.Combine(outputPath, $"{Path.GetFileNameWithoutExtension(svgPath)} tree.txt")))
                {
                    writer.Write(output.ToString());
                }
                // Prints output to console
                Console.WriteLine(output.ToString());
            }
            catch (Exception e)
            {
                // Prints error message to console if file cannot be processed
                Console.WriteLine($"Could not process file: {e.ToString()}");
            }
        }

        /// <summary>
        /// Converts an SVG element and any children into an SVGElement object, storing the item's label, element type, and child elements.
        /// This is called recursively to build the tree structure of the SVG element.
        /// </summary>
        /// <param name="element">The XElement to convert.</param>
        /// <returns>An SVGElement representing the XElement and its children.</returns>
        static SVGElement GetElements(XElement element)
        {
            // Creates an SVGElement for the given XElement
            var svgElement = new SVGElement
            {
                Label = GetLabel(element),
                ElementType = element.Name.LocalName
            };

            // Adds child elements to the SVGElement
            for (int i = 0; i < element.Elements().Count(); i++)
            {
                bool last = i == element.Elements().Count() - 1;
                XElement child = element.Elements().Reverse().ElementAt(i);

                svgElement.ChildElements.Add(GetElements(child));
            }

            return svgElement;
        }

        /// <summary>
        /// Function to generate a string representation of an SVGElement and any children
        /// </summary>
        /// <param name="element">The element to be parsed.</param>
        /// <param name="remainingChildren">A list of booleans representing whether there are remaining children at each level of the tree.</param>
        /// <param name="isLast">A boolean indicating whether the current element is the last child of its parent.</param>
        /// <returns>A StringBuilder representing the SVG element and any children.</returns>
        static StringBuilder PrintSVGElement(SVGElement element, List<bool> remainingChildren, bool isLast)
        {
            var line = new StringBuilder();
            // Gets label attribute
            var label = element.Label;
            // Generates spacing and branch characters based on the remaining children in the current branch of the tree
            if (remainingChildren.Count > 1)
            {
                //Determines whether to add a vertical line or spaces for each level of the tree, based on whether there are remaining children at that level
                for (int i = 1; i < remainingChildren.Count - 1; i++)
                {
                    line.Append(remainingChildren[i] ? "│  " : "   ");
                }
                // Adds the appropriate branch character for the current element, based on whether it is the last child of its parent
                line.Append(isLast ? "└─ " : "├─ ");
            }
            // Adds the label of the current element to the line
            line.Append(label);

            // Gets the number of child elements the current element has
            var childElements = element.ChildElements.Count;

            // Appends the number of child elements to the line, using "lines" for text elements and "children" for other element types
            if (childElements > 0)
            {
                if (element.ElementType == "text")
                {
                    line.Append($" ({childElements} {(childElements > 1 ? "lines" : "line")})");
                }
                else
                {
                    line.Append($" ({childElements} {(childElements > 1 ? "children" : "child")})");
                }
            }
            line.AppendLine();
            
            // Cycles through child elements and calls the function recursively to add each child
            for (int i = 0; i < childElements; i++)
            {
                SVGElement child = element.ChildElements.ElementAt(i);

                // Skips element if it is a tspan
                if (child.ElementType == "tspan") continue;

                // Checks if the element is the last child in the parent
                bool last = i == childElements - 1;

                // Creates a new list of remaining children for the next level of the tree, appending the current element
                var nextStack = new List<bool>(remainingChildren);
                nextStack.Add(!last);

                // Calls the function recursively to add the child element, passing the new list of remaining children and if the child is the parent's last
                line.Append(PrintSVGElement(child, nextStack, last));
            }
            return line;
        }

        /// <summary>
        /// Function to get a name for an SVG element.
        /// </summary>
        /// <param name="element">The XElement to get the label for.</param>
        /// <returns>A string representing the label of the SVG element.</returns>
        static string GetLabel(XElement element)
        {
            // Returns the label of the element if it has one, alongside the element type
            if (element.Attribute(inkscapeNamespace + "label") != null)
            {
                return $"{element.Attribute(inkscapeNamespace + "label").Value} ({element.Name.LocalName})";

            }
            // If no label attribute is found, returns the id attribute if it exists, alongside the element
            else if (element.Attribute("id") != null)
            {
                return $"{element.Attribute("id").Value} ({element.Name.LocalName})";
            }
            // If no label or id attribute is found, returns the element type (local name)
            else
            {
                return element.Name.LocalName;
            }
        }

    }
}