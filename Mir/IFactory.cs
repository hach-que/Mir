using System;

namespace Mir
{
    using Protogame;

    public interface IFactory
    {
        ShipEntity CreateShipEntity();
        PlayerEntity CreatePlayerEntity();
        Room CreateRoom();
        RoomObject CreateRoomObject();
    }
}

