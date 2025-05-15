namespace hotelreservation_tests;

using hotelreservation;
using HotelReservation;

/*
Hotel Name Check: It verifies if any existing reservation has the same hotel name as the new reservation. This check is insufficient; it should confirm that the hotel actually exists within the provided list of rooms.

Date Overlap Check: It checks for overlapping dates but has a logical error. The condition newReservation.StartDate >= t.StartDate && newReservation.EndDate <= t.EndDate only detects overlaps where the new reservation's entire duration falls within an existing reservation. It misses cases where the new reservation partially overlaps with an existing one (e.g., starts before and ends during, or starts during and ends after).

Room Availability: The code doesn't check if the requested room is available for the specified dates. It should verify that no existing reservation conflicts with the new reservation's dates for the specific room being booked.

Room Existence: It does not validate if the roomId in the new reservation actually exists in the list of rooms.

Room Type: The Book method does not consider the RoomType when checking for availability. It should ensure that a room of the requested type is available.
*/
[TestClass]
public class HotelReservation_UnitTests
{

    private IEnumerable<Reservation> GetExistingReservations()
    {
        return new List<Reservation>
        {
            new Reservation("hilton", "101", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"), new Customer("john","lewis","test@test.com"), RoomType.Single),
            new Reservation("hilton", "102", Convert.ToDateTime("15/01/2024"), Convert.ToDateTime("17/01/2024"), new Customer("john","lewis","test@test.com"), RoomType.Double),
            new Reservation("hilton", "103", Convert.ToDateTime("10/01/2024"), Convert.ToDateTime("12/01/2024"), new Customer("john","lewis","test@test.com"), RoomType.Single)
        };
    }

    private IEnumerable<Room> GetRooms()
    {
        return new List<Room>
        {
            new Room("hilton",RoomType.Single, 100.5f, "100"),
            new Room("hilton",RoomType.Double, 100.5f, "101"),
            new Room("hilton",RoomType.Single, 100.5f, "102"),
            new Room("hilton",RoomType.Double, 100.5f, "103"),
            new Room("hilton",RoomType.Single, 100.5f, "104")
        };
    }

    private Customer GetCustomer()
    {
        return new Customer("Tom", "Jerry", "tom.jerry@cartoon.com");
    }

    [TestMethod]
    public void HotelReservationService_Cannot_Be_Created_Without_A_List_Of_Rooms()
    {
        var reservations = GetExistingReservations();
        var customer = GetCustomer();
        Assert.ThrowsException<ArgumentException>(() => new ReservationService(null, reservations.ToList()));
    }

    [TestMethod]
    public void HotelReservationService_Cannot_Be_Created_Without_A_List_Of_Rservations()
    {
        var rooms = GetRooms();
        var customer = GetCustomer();
        Assert.ThrowsException<ArgumentNullException>(() => new ReservationService(rooms, null));
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_True_For_Customer_And_Valid_DateRange()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton", "100", Convert.ToDateTime("20/02/2024"), Convert.ToDateTime("22/02/2024"), customer, RoomType.Single);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsTrue(roomBooked);
        Assert.IsTrue(reservationService.CheckReservationExists(newReservation));
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_InvalidHotel()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("rubbish hotel", "101", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"), customer, RoomType.Single);

        Assert.ThrowsException<InvalidOperationException>(() => reservationService.Book(newReservation));

    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_InvalidHotel_Called_White_House()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("White House", "101", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"), customer, RoomType.Single);

        Assert.ThrowsException<InvalidOperationException>(() => reservationService.Book(newReservation));
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_OverLapping_Dates()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton", "101", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"), customer, RoomType.Single);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsFalse(roomBooked);
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_True_For_When_Booking_Date_Do_Not_Overlap()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton", "101", Convert.ToDateTime("20/04/2024"), Convert.ToDateTime("22/04/2024"), customer, RoomType.Single);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsTrue(roomBooked);
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_When_Booking_Date_Starts_Before_But_Overlaps_EndDate()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton", "101", Convert.ToDateTime("10/01/2024"), Convert.ToDateTime("22/01/2024"), customer, RoomType.Single);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsFalse(roomBooked);
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_When_Booking_Date_Ends_After_But_StartDate_Within_Start_EndDate()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton", "101", Convert.ToDateTime("21/01/2024"), Convert.ToDateTime("24/01/2024"), customer, RoomType.Single);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsFalse(roomBooked);
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_Reservation_With_Room_That_Does_Not_Exist()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton", "109", Convert.ToDateTime("21/01/2024"), Convert.ToDateTime("22/01/2024"), customer, RoomType.Single);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsFalse(roomBooked);
    }

}