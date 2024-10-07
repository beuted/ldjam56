using Godot;
using System;

[Tool]
public class Water : Sprite
{
  public override void _Ready()
  {
    Connect("item_rect_changed", this, "OnItemRectChanged");
    GetViewport().TransparentBg = true;
    OnItemRectChanged();
  }

  public void OnItemRectChanged()
  {
    ((ShaderMaterial) Material).SetShaderParam("aspect_ratio", Scale.y / Scale.x);
    ((ShaderMaterial) Material).SetShaderParam("tile_factor", Scale.x * 8);
  }
}
