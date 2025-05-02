using System;
using HotelReservation;

namespace hotelreservation;

/*
Write a plugin that meets the following specification.
You are building a travel planner that can make hotel Reservations by booking Rooms for Customers
The Customer should be known by FirstName, LastName, e-mail.
A Hotel Room must allow the Customer to choose it based on RoomType, price, room no.
When a reservation is made the following information must be recorded:
 - room id
 - customer 
 - checkin date
 - checkout date
Additionally a to make a reservation access to the available Hotel Rooms is required
*/
public class ReservationService
{
    private readonly IEnumerable<Room> rooms;
    private readonly List<Reservation> reservations;

    public ReservationService(IEnumerable<Room> rooms, List<Reservation> reservations)
    {
        this.rooms = rooms ?? throw new ArgumentException("Rooms list cannot be null");
        if (!rooms.Any())
            throw new ArgumentException("Rooms list cannot be empty");
            
        this.reservations = reservations ?? throw new ArgumentNullException("Reservations list cannot be null");
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public bool Book(Reservation newReservation)
    {   
        if(!reservations.Any(t=>t.HotelName.Equals(newReservation.HotelName)))
            return false;
        
        if(reservations.Any(t=>newReservation.StartDate >= t.StartDate &&  newReservation.EndDate <= t.EndDate) )
            return false;
        return true;
    }
}

