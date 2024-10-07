using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class GodotHelper
{
  public static void DeleteChildren(this Node node)
  {
    foreach (Node n in node.GetChildren())
    {
      node.RemoveChild(n);
      n.QueueFree();
    }
  }

  public static List<Node> GetChildrenOfType(this Node node, Type type)
  {
    var res = new List<Node>();

    foreach (Node n in node.GetChildren())
    {
      if (type == n.GetType())
      {
        res.Add(n);
      }
    }

    return res;
  }

  public static void DeleteChildrenExceptTypes(this Node node, Type[] types)
  {
    foreach (Node n in node.GetChildren())
    {
      if (!types.Contains(n.GetType()))
      {
        node.RemoveChild(n);
        n.QueueFree();
      }
    }
  }

  public static void DeleteChildrenExceptTypesDeferred(this Node node, Type[] types)
  {
    foreach (Node n in node.GetChildren())
    {
      if (!types.Contains(n.GetType()))
      {
        node.CallDeferred("remove_child", n);
        n.QueueFree();
      }
    }
  }

}
