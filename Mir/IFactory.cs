namespace Mir
{
    public interface IFactory
    {
        DCPUEntity CreateDCPUEntity();

        LightEntity CreateLightEntity(LightRoomObject lightRoomObject);

        PlayerEntity CreatePlayerEntity();

        Room CreateRoom();

        RoomEditorEntity CreateRoomEditorEntity(Room room);

        RoomObject CreateRoomObject();

        Ship CreateShip();

        ShipEditorEntity CreateShipEditorEntity(Ship ship);

        ShipEntity CreateShipEntity();
    }
}