using System;
using System.Collections.Generic;
using System.Text;

namespace InkscapeGroupPrinter
{
    /// <summary>
    /// Class to represent an SVG element with its label, type, and child elements.
    /// </summary>
    public class SVGElement
    {
        /// <summary>
        /// The label of the element.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The type the element represents (e.g., "group", "path", "rect", etc.).
        /// </summary>
        public string ElementType { get; set; }
        /// <summary>
        /// The child elements of the present SVG element.
        /// </summary>
        public List<SVGElement> ChildElements { get; set; } = new List<SVGElement>();
    }
}
