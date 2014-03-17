namespace Mir
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework.Input;
    using Protogame;
    using Tomato.Hardware;

    public class KeyboardRoomObject : RoomObject, IConnectable
    {
        private GenericKeyboard m_GenericKeyboard;

        public KeyboardRoomObject(bool temporary)
        {
            this.Connections = new List<IConnectable>();
        }

        public List<IConnectable> Connections { get; set; }

        public Device TomatoDevice
        {
            get
            {
                return this.m_GenericKeyboard;
            }
        }

        public override string Type
        {
            get
            {
                return "keyboard";
            }
        }

        public void ConnectionsUpdated()
        {
        }

        public override void Reinitalize()
        {
        }

        public void PrepareDevice()
        {
            this.m_GenericKeyboard = new GenericKeyboard();
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            this.AboveTextureIndex = 19;
            this.BelowTextureIndex = 18;
            this.LeftTextureIndex = 18;
            this.RightTextureIndex = 18;
            this.FrontTextureIndex = 18;
            this.BackTextureIndex = 18;

            base.Render(gameContext, renderContext);

            if (this.m_GenericKeyboard != null)
            {
                var roomEditorWorld = gameContext.World as RoomEditorWorld;
                if (roomEditorWorld == null || roomEditorWorld.FocusedKeyboard != this)
                {
                    return;
                }

                var keyboard = Keyboard.GetState();
                var values = Enum.GetValues(typeof(Keys));
                var winNames = Enum.GetNames(typeof(System.Windows.Forms.Keys));
                foreach (var id in values)
                {
                    var name = Enum.GetName(typeof(Keys), id);
                    if (!winNames.Contains(name))
                    {
                        if (name == "LeftShift" || name == "RightShift")
                        {
                            name = "Shift";
                        }
                        else if (name == "LeftControl" || name == "RightControl")
                        {
                            name = "Control";
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (keyboard.IsKeyChanged(this, (Keys)id) == KeyState.Down)
                    {
                        this.m_GenericKeyboard.KeyDown(
                            (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), name));
                    }

                    if (keyboard.IsKeyUp((Keys)id))
                    {
                        this.m_GenericKeyboard.KeyUp(
                            (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), name));
                    }
                }
            }
        }
    }
}