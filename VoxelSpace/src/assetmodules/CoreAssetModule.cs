using System;
using VoxelSpace.Resources;
using VoxelSpace.UI;

namespace VoxelSpace.Assets {

    public class CoreAssetModule : AssetModule {
        
        public override string Name => "core";

        protected override void OnLoadAssets() {

             // ui
            var font = Add(
                new TileFontInfo("ui.font2")
                .SpaceWidth(6)
                .Baseline(7)
                .LineSpacing(3)
            );
            var padding = new Padding(6, 6, 6, 6);
            
            Add(
                new UISkinInfo("ui.skin")
                .Button(
                    new UIBoxStyleInfo("ui.skin.button")
                    .Normal(
                        new UIStyleInfo("ui.skin.button.normal")
                        .Background(new NinePatchInfo("ui.skin.button", 6, 6, 6, 6))
                        .Font(font)
                        .Align(HorizontalAlign.Center, VerticalAlign.Middle)
                        .Padding(padding)
                    )
                    .Disabled(
                        new UIStyleInfo("ui.skin.button.disabled")
                        .Background(new NinePatchInfo("ui.skin.button-disabled", 6, 6, 6, 6))
                        .Font(font)
                        .Align(HorizontalAlign.Center, VerticalAlign.Middle)
                        .Padding(padding)
                    )
                    .Hover(
                        new UIStyleInfo("ui.skin.button.hover")
                        .Background(new NinePatchInfo("ui.skin.button-hover", 6, 6, 6, 6))
                        .Font(font)
                        .Align(HorizontalAlign.Center, VerticalAlign.Middle)
                        .Padding(padding)
                    )
                )
                .TextBox(
                    new UITextBoxStyleInfo("ui.skin.textbox")
                    .Normal(
                        new UIStyleInfo("ui.skin.textbox.normal")
                        .Background(new NinePatchInfo("ui.skin.button", 6, 6, 6, 6))
                        .Font(font)
                        .Align(HorizontalAlign.Left, VerticalAlign.Middle)
                        .Padding(padding)
                    )
                    .Disabled(
                        new UIStyleInfo("ui.skin.textbox.disabled")
                        .Background(new NinePatchInfo("ui.skin.button-disabled", 6, 6, 6, 6))
                        .Font(font)
                        .Align(HorizontalAlign.Left, VerticalAlign.Middle)
                        .Padding(padding)
                    )
                    .Hover(
                        new UIStyleInfo("ui.skin.textbox.hover")
                        .Background(new NinePatchInfo("ui.skin.button-hover", 6, 6, 6, 6))
                        .Font(font)
                        .Align(HorizontalAlign.Left, VerticalAlign.Middle)
                        .Padding(padding)
                    )
                    .Active(
                        new UIStyleInfo("ui.skin.textbox.active")
                        .Background(new NinePatchInfo("ui.skin.textbox-active", 6, 6, 6, 6))
                        .Font(font)
                        .Align(HorizontalAlign.Left, VerticalAlign.Middle)
                        .Padding(padding)
                    )
                    .Cursor(new NinePatchInfo("ui.skin.cursor", 1, 1, 1, 1))
                )
            );

            Add(new VoxelTypeInfo("grass").TBSCSkin("grassT", "dirtTB", "grassS", "grassC"));
            Add(new VoxelTypeInfo("dirt").TBSCSkin("dirtTB", "dirtTB", "dirtS", "dirtC"));
            Add(new VoxelTypeInfo("stone").TBSCSkin("stoneTB", "stoneTB", "stoneS", "stoneC"));
        }
    }

}