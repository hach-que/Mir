namespace Mir
{
    public interface IFactory
    {
        LightEntity CreateLightEntity(LightRoomObject lightRoomObject);

        PlayerEntity CreatePlayerEntity();

        Room CreateRoom();

        RoomEditorEntity CreateRoomEditorEntity(Room room);

        RoomEditorWorld CreateRoomEditorWorld(ShipEditorWorld previousWorld, Room room);

        RoomObject CreateRoomObject();

        Ship CreateShip();

        ShipEditorEntity CreateShipEditorEntity(Ship ship);

        ShipEditorWorld CreateShipEditorWorld();

        ShipEntity CreateShipEntity();
    }
}