namespace Mir
{
    public interface IFactory
    {
        PlayerEntity CreatePlayerEntity();

        Room CreateRoom();

        RoomObject CreateRoomObject();

        ShipEntity CreateShipEntity();

        RoomEditorEntity CreateRoomEditorEntity(Room room);
    }
}