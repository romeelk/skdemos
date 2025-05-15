using hotelreservation;

namespace HotelReservation;


public class Reservation
{
    private readonly string hotelName;
    private readonly DateTime? startDate;
    private readonly DateTime? endDate;
    private readonly Customer guest;
    private readonly string roomId;
    private readonly RoomType roomType;

    public Reservation(string hotelName, string roomId, DateTime? startDate, DateTime? endDate, Customer guest, RoomType roomType)
    {
        ArgumentNullException.ThrowIfNull(hotelName, nameof(hotelName));

        if (startDate >= endDate)
        {
            throw new ArgumentException("Start date must be before end date.");
        }

        this.hotelName = hotelName;
        this.roomId = roomId;
        this.startDate = startDate;
        this.endDate = endDate;
        this.guest = guest;
        this.roomType = roomType;
    }

    public string HotelName => hotelName;
    public DateTime? StartDate => startDate;
    public DateTime? EndDate => endDate;
    public string GuestName => guest.Name;
    public string RoomId => roomId;
    public RoomType RoomType => roomType;
}
