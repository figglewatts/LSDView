using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace LSDView.GUI
{
    public abstract class ImGuiComponent
    {
        protected readonly Dictionary<string, Modal> _modals;
        protected readonly MainWindow _window;
        protected readonly Queue<string> _modalsToCreate;
        protected readonly Queue<string> _modalsToDestroy;

        protected static int ModalCount = 0;

        protected ImGuiComponent(MainWindow window)
        {
            _modals = new Dictionary<string, Modal>();
            _modalsToCreate = new Queue<string>();
            _modalsToDestroy = new Queue<string>();
            _window = window;
        }

        public abstract void Render();

        protected void renderModals()
        {
            while (_modalsToCreate.Count > 0)
            {
                var modalName = _modalsToCreate.Dequeue();
                var modal = _modals[modalName];
                ImGui.SetNextWindowSize(modal.InitialSize);
                ImGui.OpenPopup(modalName);
            }

            while (_modalsToDestroy.Count > 0)
            {
                _modals.Remove(_modalsToDestroy.Dequeue());
            }

            foreach (KeyValuePair<string, Modal> kv in _modals)
            {
                if (ImGui.BeginPopupModal(kv.Key, ref kv.Value.Active))
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

            _modals[actualName] = new Modal(component, size);
            _modalsToCreate.Enqueue(actualName);
        }

        protected class Modal
        {
            public bool Active = true;
            public readonly ImGuiComponent Component;
            public readonly Vector2 InitialSize;

            public Modal(ImGuiComponent component, Vector2 initialSize)
            {
                Component = component;
                InitialSize = initialSize;
            }
        }
    }
}
