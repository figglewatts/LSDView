using System;
using System.Collections.Generic;
using ImGuiNET;

namespace LSDView.GUI.Components
{
    public class Columns : ImGuiComponent
    {
        public int Count { get; private set; }
        protected readonly List<ImGuiComponent> _content;
        protected readonly float[] _widths;

        private readonly bool[] _setWidths;

        public Columns(int count, List<ImGuiComponent> content, float[] widths = null)
        {
            Count = count;

            if (Count != content.Count)
            {
                throw new ArgumentException("Column count needs to be equal to content count!");
            }

            if (widths == null) widths = new float[Count];
            _widths = widths;
            _setWidths = new bool[Count];

            _content = content;
        }

        protected override void renderSelf()
        {
            ImGui.Columns(Count);
            for (int i = 0; i < Count; i++)
            {
                // if we haven't already set the width and the width is valid, then set it
                if (!_setWidths[i] && _widths[i] > 0)
                {
                    ImGui.SetColumnWidth(i, _widths[i]);
                    _setWidths[i] = true;
                }

                _content[i].Render();
                ImGui.NextColumn();
            }
        }
    }
}
