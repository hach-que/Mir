using System;

namespace Mir
{
    public interface IFactory
    {
        ShipEntity CreateShipEntity();
        PlayerEntity CreatePlayerEntity();
    }
}

