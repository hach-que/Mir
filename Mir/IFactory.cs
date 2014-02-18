using System;

namespace Mir
{
    public interface IFactory
    {
        ShipEntity CreateShipEntity();
        PlayerEntity CreatePlayerEntity();
        ShipLayout CreateShipLayout(int size);
        LayoutCell CreateLayoutCell();
        StandardCell CreateStandardCell();
    }
}

