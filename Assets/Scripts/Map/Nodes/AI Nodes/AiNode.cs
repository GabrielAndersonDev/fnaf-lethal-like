using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AiNode : MonoBehaviour
{
    public List<AiNode> adjacentNodes = new();
    public bool isEdgeNode;
    public MapNode attMapNode;
}
