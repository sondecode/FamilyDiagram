using Blazor.Diagrams;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Anchors;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Routers;
using Blazor.Diagrams.Core.PathGenerators;
using Blazor.Diagrams.Options;
using FamilyDiagram.Model;

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
                Enabled = false,
            },
            Links =
            {
                DefaultRouter = new NormalRouter(),
                DefaultPathGenerator = new SmoothPathGenerator()
            },
        };

        Diagram = new BlazorDiagram(options);
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
    int FamilyRender(Person nodeTarget, int Floor, Anchor? sourceAnchor)
    {
        if (nodeTarget == null) return 0;
        var firstNode = Diagram.Nodes.Add(new NodeModel(position: new Point(nodeTarget.Layer * xSize, Floor * xSize))
        {
            Title = "Target"
        });
        var targetAnchor = new ShapeIntersectionAnchor(firstNode);
        if (sourceAnchor != null)
        {
            var sourceLink = Diagram.Links.Add(new LinkModel(targetAnchor, sourceAnchor));
            sourceLink.PathGenerator = new StraightPathGenerator();
        }
        if (nodeTarget is HasFamilies nodeTargetHasFamilies)
        {
            if (nodeTargetHasFamilies.Families != null)
            {
                var countFloor = FamilyMemberRender(nodeTargetHasFamilies.Families.First()!.Children!, Floor, targetAnchor);
                if (nodeTargetHasFamilies.Families.First().Spouse != null)
                {
                    var secondNode = Diagram.Nodes.Add(new NodeModel(position: new Point(nodeTarget.Layer * xSize, (Floor + 1) * xSize))
                    {
                        Title = "Spouse"
                    });
                    var spouseAnchor = new ShapeIntersectionAnchor(secondNode);
                    var spouseLink = Diagram.Links.Add(new LinkModel(targetAnchor, spouseAnchor));
                    return nodeTargetHasFamilies.Families.First()!.Children.Count() > 2 ? countFloor : Floor + 1;
                }
                return countFloor;
            }
        }
        return Floor;
    }
    int FamilyMemberRender(List<Child> children, int Floor, Anchor targetAnchor)
    {
        var tempFloor = Floor;
        foreach (var child in children)
        {
            Console.WriteLine(tempFloor);
            var countFloor = 0;
            if (child == children.First())
            {
                countFloor = FamilyRender(child, tempFloor, targetAnchor);
            }
            else
            {
                countFloor = FamilyRender(child, tempFloor + 1, targetAnchor);
            }
            tempFloor = countFloor;
        }
        return tempFloor;
    }


}