using hotelreservation;

namespace HotelReservation;


public class Room
{
    private readonly string roomType;
    private readonly float price;
    private readonly string roomNo;
    
    public Room(string roomType, float price, string roomNo)
    {
        if (string.IsNullOrWhiteSpace(roomType))
            throw new ArgumentNullException(nameof(roomType));
            
        if (string.IsNullOrWhiteSpace(roomNo))
            throw new ArgumentNullException(nameof(roomNo));

        this.roomType = roomType;
        this.price = price;
        this.roomNo = roomNo;
    }

    public string RoomType => roomType;

    public float Price => price;
    public string RoomNo => roomNo;
}
