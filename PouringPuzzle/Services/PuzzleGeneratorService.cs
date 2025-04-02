using PouringPuzzle.Models;
using PouringPuzzle.Utilities;
using System.Diagnostics;
using System.Security.Cryptography;

namespace PouringPuzzle.Services;

public class PuzzleGeneratorService  : IPuzzleGeneratorService
{
    public PuzzleGame GeneratePuzzle(List<Vessel> vessels, int goal) {
        var startingNode = new PuzzleNode();
        startingNode.Vessels = vessels;

        var game = new PuzzleGame();
        game.CurrentNode = startingNode;
        game.NodeList = GenerateNodeList(startingNode);
        PuzzleUtils.DebugPrintNodeList(game.NodeList, "After A/D setting");
        game.Goal = goal;
        PuzzleUtils.DebugPrintNodeList(game.NodeList, "After distance setting");

        return game;
    }

    private List<PuzzleNode> GenerateNodeList(PuzzleNode startNode) {
        var nodeList = new List<PuzzleNode>();
        nodeList.Add(startNode);

        var nodeSearchStack = new Stack<PuzzleNode>();
        nodeSearchStack.Push(startNode);

        while (nodeSearchStack.Count > 0) {
            var currentNode = nodeSearchStack.Pop();

            foreach (var descendant in GenerateDescendantStates(currentNode)) {
                if (!PuzzleUtils.IsDuplicateState(descendant, nodeList)) {
                    var descendantNode = new PuzzleNode();
                    
                    for (var i = 0; i < currentNode.Vessels.Count; i++) {
                        descendantNode.Vessels.Add(new Vessel
                        {
                            Value = descendant[i],
                            Max = currentNode.Vessels[i].Max
                        });
                    }

                    nodeList.Add(descendantNode);
                    nodeSearchStack.Push(descendantNode);
                }
            }
        }

        PuzzleUtils.DebugPrintNodeList(nodeList, "Before A/D setting");

        foreach (var node in nodeList) { 
            foreach (var descendant in GenerateDescendantStates(node)) {
                node.DescendantNodes.Add(PuzzleUtils.GetNodeFromListByState(descendant, nodeList));
            }
            foreach (var ancestor in GenerateAncestorStates(node, nodeList)) {
                node.AncestorNodes.Add(PuzzleUtils.GetNodeFromListByState(ancestor, nodeList));
            }
        }

        return nodeList;
    }

    private List<List<int>> GenerateDescendantStates(PuzzleNode node) {
        var descendantStates = new List<List<int>>();
        for (int i = 0; i < node.Vessels.Count; i++) {     // i = sending vessel index
            for (int j = 0; j < node.Vessels.Count; j++) { // j = receiving vessel index

                // skip if sending and receiving vessel are same index 
                if (i == j) {
                    continue;
                }

                int pourAmount = Math.Min(node.Vessels[i].Value, node.Vessels[j].SpaceAvailable);

                if (pourAmount == 0) {
                    continue;
                }

                List<int> descendant = new List<int>(node.VesselValues);
                descendant[i] -= pourAmount;
                descendant[j] += pourAmount;
                descendantStates.Add(descendant);
            }
        }
        return descendantStates;
    }

    private List<List<int>> GenerateAncestorStates(PuzzleNode node, List<PuzzleNode> viableNodeList) {
        var ancestorStates = new List<List<int>>();
        for (int i = 0; i < node.Vessels.Count; i++) {     // i = sending vessel index
            for (int j = 0; j < node.Vessels.Count; j++) { // j = receiving vessel index

                // skip if sending and receiving vessel are same index 
                if (i == j || (node.Vessels[i].SpaceAvailable != 0 && node.Vessels[j].Value != 0)) { 
                    continue;
                }

                int reversePourAmount = Math.Min(node.Vessels[i].SpaceAvailable, node.VesselValues[j]);

                var ancestorState = new List<int>(node.VesselValues);
                ancestorState[i] += reversePourAmount;
                ancestorState[j] -= reversePourAmount;
                ancestorStates.Add(ancestorState); 
            }
        }
        return ancestorStates;
    }
}
