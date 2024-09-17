using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace FamilyDiagram.Pages.DiagramWidgets
{
    public class InvisibleNode : NodeModel
    {
        public InvisibleNode(Point? position = null) : base(position) { }

        public double FirstNumber { get; set; }
        public double SecondNumber { get; set; }

    }
}
