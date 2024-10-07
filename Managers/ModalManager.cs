using Godot;
using System;

public class ModalManager : Node2D
{
  private PackedScene _modalScene;
  private Modal _modal;
  private Control _modalsNode;

  private static float _borderClampMargin = 5;
  private static Vector2 _tooltipSize = new Vector2(50, 50); //TODO: size should be dynamic depending on the size of the screen

  public override void _Ready()
  {
    _modalScene = ResourceLoader.Load<PackedScene>("res://UI/InGame/Modal.tscn");
  }

  public void Init(Control modalsNode)
  {
    _modalsNode = modalsNode;

    _modal = _modalScene.Instance() as Modal;
    _modal.Visible = false;

    _modalsNode.AddChild(_modal);
  }

  public void ShowModal(ModalInfo modalInfo, Vector2 position)
  {
    var viewportSize = GetViewport().Size;
    var displayPosition = new Vector2(
      Mathf.Clamp(position.x, _tooltipSize.x / 2 + _borderClampMargin, viewportSize.x - _tooltipSize.x / 2 - _borderClampMargin),
      Mathf.Clamp(position.y + 10, _borderClampMargin, viewportSize.y - _tooltipSize.y - 10 - _borderClampMargin)
    );
    _modal.SetInfo(modalInfo);
    _modal.RectPosition = displayPosition;
    _modal.Visible = true;
  }

  public void HideModal()
  {
    _modal.Visible = false;
  }
}