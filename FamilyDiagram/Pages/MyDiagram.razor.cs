using Blazor.Diagrams;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Options;
using FamilyDiagram.Model;
using FamilyDiagram.Pages.DiagramWidgets;

namespace FamilyDiagram.Pages;

public partial class MyDiagram
{

    const int xSize = 200;
    const int ySize = 100;
    private BlazorDiagram Diagram { get; set; } = null!;
    public FamilyTreeModel familyTreeModel { get; set; }

    protected override void OnInitialized()
    {
        familyTreeModel = new FamilyTreeModel
        {
            Target = new Target
            {
                Families = [
                    new Family{
                        Spouse = new Spouse {},
                        Children = [
                            new Child{
                                Layer = 1,
                                Families = [
                                    new Family{
                                        Spouse = new Spouse {},
                                        Children = [
                                            new Child{Layer = 2},
                                            new Child{Layer = 2},
                                            new Child{Layer = 2},
                                            new Child{Layer = 2},
                                        ]
                                    }
                                ]
                                },
                            new Child{
                                Layer = 1,
                                Families = [
                                    new Family{
                                        Spouse = new Spouse {},
                                        Children = [
                                            new Child{
                                                Layer = 2,
                                                Families = [
                                                    new Family{
                                                        Spouse = new Spouse {},
                                                        Children = [
                                                            new Child{Layer = 3},
                                                            new Child{Layer = 3},
                                                            new Child{Layer = 3},
                                                        ]
                                                    }
                                                ]
                                                },
                                            new Child{Layer = 2},
                                            new Child{Layer = 2},
                                        ]
                                    }
                                ]
                                },
                            new Child{Layer = 1}
                        ]
                    }
                ]
            }
        };
        var options = new BlazorDiagramOptions
        {
            AllowMultiSelection = true,
            Zoom =
            {
                Enabled = true,
            },
            Links =
            {
                DefaultRouter = new OrthogonalRouter(),
                DefaultPathGenerator = new StraightPathGenerator()
            },
        };

        Diagram = new BlazorDiagram(options);
        Diagram.RegisterComponent<InvisibleNode, InvisibleNodeWidget>();
        FamilyRender(familyTreeModel.Target, 0, null);
        //     var firstNode = Diagram.Nodes.Add(new NodeModel(position: new Point(50, 50))
        //     {
        //         Title = "Node 1"
        //     });
        //     var secondNode = Diagram.Nodes.Add(new NodeModel(position: new Point(200, 100))
        //     {
        //         Title = "Node 2"
        //     });
        //     var leftPort = secondNode.AddPort(PortAlignment.Left);
        //     var rightPort = secondNode.AddPort(PortAlignment.Right);
        //     // The connection point will be the intersection of
        //     // a line going from the target to the center of the source
        //     var sourceAnchor = new ShapeIntersectionAnchor(firstNode);
        //     // The connection point will be the port's position
        //     var targetAnchor = new SinglePortAnchor(leftPort);
        //     var link = Diagram.Links.Add(new LinkModel(sourceAnchor, targetAnchor));
        //     var thirdNode = Diagram.Nodes.Add(new NodeModel(position: new Point(-200, -100))
        //     {
        //         Title = "Node 3"
        //     });
    }
    //Hien thi Family
    double FamilyRender(Person nodeTarget, double Floor, PortModel? sourcePort)
    {
        if (nodeTarget == null) return 0.0;
        var firstNode = Diagram.Nodes.Add(new NodeModel(position: new Point(nodeTarget.Layer * xSize, Floor * xSize))
        {
            Title = "Target"
        });
        var targetLeftPort = firstNode.AddPort(PortAlignment.Left);
        var targetRightPort = firstNode.AddPort(PortAlignment.Right);
        var targetBottomPort = firstNode.AddPort(PortAlignment.Bottom);
        if (sourcePort != null)
        {
            var sourceLink = Diagram.Links.Add(new LinkModel(targetLeftPort, sourcePort));
            sourceLink.PathGenerator = new StraightPathGenerator();
        }
        if (nodeTarget is HasFamilies nodeTargetHasFamilies)
        {
            if (nodeTargetHasFamilies.Families != null)
            {
                if (nodeTargetHasFamilies.Families.First().Spouse != null)
                {
                    //Render Spouse
                    var secondNode = Diagram.Nodes.Add(new NodeModel(position: new Point(nodeTarget.Layer * xSize, (Floor + 1) * xSize))
                    {
                        Title = "Spouse"
                    });
                    var spousePort = secondNode.AddPort(PortAlignment.Top);
                    //Render Paralell Link
                    var spouseLink = Diagram.Links.Add(new LinkModel(targetBottomPort, spousePort)
                    {
                        Router = new NormalRouter()
                    });
                    //Render Invisible Node
                    var invisibleNode = Diagram.Nodes.Add(new InvisibleNode(new Point(nodeTarget.Layer * xSize + 140, (Floor + 1) * xSize - 84)));//To-do
                    var invisibleRightPort = invisibleNode.AddPort(PortAlignment.Right);
                    var invisibleLeftPort = invisibleNode.AddPort(PortAlignment.Left);
                    var invisibleAnchor = new SinglePortAnchor(invisibleLeftPort)
                    {
                        MiddleIfNoMarker = true,
                    };

                    //Render Invisible Anchor
                    var sourceAnchor = new LinkAnchor(spouseLink, distance: 0.5, offsetX: 0, offsetY: 0);
                    var invisibleLink = Diagram.Links.Add(new LinkModel(sourceAnchor, invisibleAnchor));

                    //Render Children
                    var countFloor = FamilyMemberRender(nodeTargetHasFamilies.Families.First()!.Children!, Floor + 0.5, invisibleRightPort);
                    return nodeTargetHasFamilies.Families.First()!.Children.Count() > 2 ? countFloor : Floor + 1;
                }
                else
                {
                    var countFloor = FamilyMemberRender(nodeTargetHasFamilies.Families.First()!.Children!, Floor, targetRightPort);
                    return countFloor;
                }
            }
        }
        return Floor;
    }
    double FamilyMemberRender(List<Child> children, double Floor, PortModel sourcePort)
    {
        var tempFloor = Floor;
        foreach (var child in children)
        {
            Console.WriteLine(tempFloor);
            var countFloor = 0.0;
            if (child == children.First())
            {
                countFloor = FamilyRender(child, tempFloor, sourcePort);
            }
            else
            {
                countFloor = FamilyRender(child, tempFloor + 1, sourcePort);
            }
            tempFloor = countFloor;
        }
        return tempFloor;
    }


}