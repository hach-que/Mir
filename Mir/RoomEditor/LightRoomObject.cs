namespace Mir
{
    using Protogame;

    public class LightRoomObject : RoomObject
    {
        private readonly IFactory m_Factory;

        private readonly bool m_Temporary;

        private LightEntity m_LightEntity;

        private bool m_Spawned;

        public LightRoomObject(IFactory factory, bool temporary)
        {
            this.m_Factory = factory;
            this.m_Spawned = temporary;
        }

        public override string Type
        {
            get
            {
                return "light";
            }
        }

        public override void Reinitalize()
        {
            this.m_Spawned = false;
        }

        public override void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            base.Render(gameContext, renderContext);

            if (this.m_Spawned)
            {
                return;
            }

            var world = (RoomEditorWorld)gameContext.World;

            this.m_LightEntity = this.m_Factory.CreateLightEntity(this);
            this.m_LightEntity.X = this.X;
            this.m_LightEntity.Y = this.Y;
            this.m_LightEntity.Z = this.Z;

            world.Entities.Add(this.m_LightEntity);

            this.m_Spawned = true;
        }
    }
}