using System;
using System.Collections.Generic;
using ImGuiNET;
using LSDView.GUI.Components;
using OpenTK;

namespace LSDView.GUI
{
    public abstract class ImGuiComponent
    {
        public bool Show = true;

        protected readonly List<ImGuiComponent> _children;
        protected ContextMenu _contextMenu;
        protected Action _tooltip;

        private static int ModalCount = 0;
        private readonly Dictionary<string, Modal> _modals;
        private readonly Queue<string> _modalsToCreate;
        private readonly Queue<string> _modalsToDestroy;

        protected ImGuiComponent(ContextMenu contextMenu = null, Action tooltip = null)
        {
            _children = new List<ImGuiComponent>();
            _modals = new Dictionary<string, Modal>();
            _modalsToCreate = new Queue<string>();
            _modalsToDestroy = new Queue<string>();
            _contextMenu = contextMenu;
            _tooltip = tooltip;
        }

        public void Render()
        {
            if (Show)
            {
                renderSelf();
            }

            renderModals();
        }

        protected abstract void renderSelf();

        public virtual void AddChild(ImGuiComponent component) { _children.Add(component); }

        public virtual void AddChildren(IEnumerable<ImGuiComponent> components) { _children.AddRange(components); }

        protected void renderChildren()
        {
            foreach (var child in _children)
            {
                if (child.Show) child.Render();
            }
        }

        private void renderModals()
        {
            while (_modalsToCreate.Count > 0)
            {
                var modalName = _modalsToCreate.Dequeue();
                var modal = _modals[modalName];
                ImGui.SetNextWindowSize(new System.Numerics.Vector2(modal.InitialSize.X, modal.InitialSize.Y));
                ImGui.OpenPopup(modal.Id);
            }

            while (_modalsToDestroy.Count > 0)
            {
                _modals.Remove(_modalsToDestroy.Dequeue());
            }

            foreach (KeyValuePair<string, Modal> kv in _modals)
            {
                if (ImGui.BeginPopupModal(kv.Value.Id, ref kv.Value.Active))
                {
                    kv.Value.Component.Render();
                    ImGui.EndPopup();
                }

                if (!kv.Value.Active)
                {
                    _modalsToDestroy.Enqueue(kv.Key);
                }
            }
        }

        protected void createModal(string name, ImGuiComponent component, Vector2 size)
        {
            string actualName = $"{name}##{ModalCount++}";

            _modals[name] = new Modal(actualName, component, size);
            _modalsToCreate.Enqueue(name);
        }

        protected void destroyModal(string name)
        {
            if (!_modals.ContainsKey(name)) return;
            _modalsToDestroy.Enqueue(name);
        }

        internal class Modal
        {
            public readonly string Id;
            public bool Active = true;
            public readonly ImGuiComponent Component;
            public readonly Vector2 InitialSize;

            public Modal(string id, ImGuiComponent component, Vector2 initialSize)
            {
                Id = id;
                Component = component;
                InitialSize = initialSize;
            }
        }
    }
}
