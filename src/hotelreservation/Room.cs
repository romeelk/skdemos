using hotelreservation;

namespace HotelReservation;

public enum RoomType
{
    Single,
    Double
}

public class Room
{
    private readonly RoomType roomType;
    private readonly float price;
    private readonly string roomNo;
    
    public Room(RoomType roomType, float price, string roomNo)
    {
        if (string.IsNullOrWhiteSpace(roomNo))
            throw new ArgumentNullException(nameof(roomNo));

        this.roomType = roomType;
        this.price = price;
        this.roomNo = roomNo;
    }

    public RoomType RoomType => roomType;

    public float Price => price;
    public string RoomNo => roomNo;
}
