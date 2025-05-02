namespace hotelreservation_tests;

using hotelreservation;
using HotelReservation;

[TestClass]
public class HotelReservation_UnitTests
{

    private IEnumerable<Reservation> GetExistingReservations()
    {
        return new List<Reservation>
        {
            new Reservation("hilton", "101", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"), new Customer("john","lewis","test@test.com")),
            new Reservation("hilton", "102", Convert.ToDateTime("15/01/2024"), Convert.ToDateTime("17/01/2024"), new Customer("john","lewis","test@test.com")),
            new Reservation("hilton", "103", Convert.ToDateTime("10/01/2024"), Convert.ToDateTime("12/01/2024"), new Customer("john","lewis","test@test.com"))
        };
    }

    private IEnumerable<Room> GetRooms()
    {
        return new List<Room>
        {
            new Room("Single",100.5f,"100"),
            new Room("Double",100.5f,"101"),
            new Room("Single",100.5f,"102"),
            new Room("Double",100.5f,"103"),
            new Room("Single",100.5f,"104")  
        };
    }

    private Customer GetCustomer(){
        return new Customer("Tom","Jerry","tom.jerry@cartoon.com");
    }

    [TestMethod]
    public void HotelReservationService_Cannot_Be_Created_Without_A_List_Of_Rooms()
    {
        var reservations = GetExistingReservations();
        var customer = GetCustomer();
        Assert.ThrowsException<ArgumentException>(()=> new ReservationService(null,reservations.ToList()));
    }

    [TestMethod]
    public void HotelReservationService_Cannot_Be_Created_Without_A_List_Of_Rservations()
    {
        var rooms = GetRooms();
        var customer = GetCustomer();
        Assert.ThrowsException<ArgumentNullException>(()=> new ReservationService(rooms, null));
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_True_For_Customer_And_Valid_DateRange()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton","100", Convert.ToDateTime("20/02/2024"), Convert.ToDateTime("22/02/2024"),customer);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsTrue(roomBooked);
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_InvalidHotel()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("rubbish hotel","100", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"),customer);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsFalse(roomBooked);
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_InvalidHotel_Called_White_House()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("White House","101", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"),customer);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsFalse(roomBooked);
    }

    [TestMethod]
    public void HotelReservationService_BookRoom_Returns_False_For_OverLapping_Dates()
    {
        var rooms = GetRooms();
        var reservations = GetExistingReservations();
        var customer = GetCustomer();

        var reservationService = new ReservationService(rooms, reservations.ToList());

        var newReservation = new Reservation("hilton","101", Convert.ToDateTime("20/01/2024"), Convert.ToDateTime("22/01/2024"),customer);

        var roomBooked = reservationService.Book(newReservation);

        Assert.IsFalse(roomBooked);
    }
}