using Godot;
using System;
using System.Collections.Generic;

public class ModalInfo {
    public string Name;
}

public class Modal : Control
{
  private Label _name;


  public override void _Ready()
  {
    _name = GetNode<Label>("ColorRect/Label");
  }

  public void SetInfo(ModalInfo modalInfo)
  {
    _name.Text = modalInfo.Name;
  }
}