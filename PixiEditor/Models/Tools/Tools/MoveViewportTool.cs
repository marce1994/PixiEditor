﻿using PixiEditor.Models.Controllers;
using PixiEditor.Models.Position;
using PixiEditor.ViewModels.SubViewModels.Main;
using System.Collections.Generic;
using System.Windows.Input;

namespace PixiEditor.Models.Tools.Tools
{
    public class MoveViewportTool : ReadonlyTool
    {
        private ToolsViewModel ToolsViewModel { get; }

        public MoveViewportTool(ToolsViewModel toolsViewModel)
        {
            Cursor = Cursors.SizeAll;
            ActionDisplay = "Click and move to pan viewport.";

            ToolsViewModel = toolsViewModel;
        }

        public override bool HideHighlight => true;
        public override string Tooltip => "Move viewport. (H)";

        public override void OnMouseUp(MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                ToolsViewModel.SetActiveTool(ToolsViewModel.LastActionTool);
            }
        }

        public override void Use(List<Coordinates> pixels)
        {
            // Implemented inside Zoombox.xaml.cs
        }
    }
}
