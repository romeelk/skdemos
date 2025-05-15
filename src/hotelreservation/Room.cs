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

    private readonly string hotelName;
    
    public Room(string hotelName, RoomType roomType, float price, string roomNo)
    {
        if (string.IsNullOrWhiteSpace(roomNo))
            throw new ArgumentNullException(nameof(roomNo));

        this.hotelName = hotelName;
        this.roomType = roomType;
        this.price = price;
        this.roomNo = roomNo;
    }

    public string HotelName => hotelName;
    public RoomType RoomType => roomType;

    public float Price => price;
    public string RoomNo => roomNo;
}
