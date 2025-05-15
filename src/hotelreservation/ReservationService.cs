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
    /// Book a reservation on behalf oa customer.
    /// Reservations can't be booked if dates overlap
    /// </summary>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public bool Book(Reservation newReservation)
    {
        if (!rooms.Any(t => t.HotelName.Equals(newReservation.HotelName)))
            throw new InvalidOperationException("You cannot book for a hotel that does not exist");
            
        if (!rooms.Any(t => t.RoomNo == newReservation.RoomId))
            return false;

        if (!IsRooomAvailable(newReservation))
            return false;

        reservations.Add(newReservation);
        return true;
    }

    private bool IsRooomAvailable(Reservation newReservation)
    {
        return !reservations.Any(existing => newReservation.RoomType.Equals(existing.RoomType) && (
            (newReservation.StartDate >= existing.StartDate && newReservation.EndDate <= existing.EndDate) ||
            (newReservation.StartDate <= existing.StartDate && newReservation.EndDate <= existing.EndDate) ||
            (newReservation.StartDate >= existing.StartDate && newReservation.StartDate <= existing.EndDate && newReservation.EndDate > existing.EndDate)));
    }

    public bool CheckReservationExists(Reservation reservation)
    {
        return reservations.Any(t => t.RoomId.Equals(reservation.RoomId) && t.RoomType.Equals(reservation.RoomType) && t.StartDate.Equals(reservation.StartDate) && t.EndDate.Equals(reservation.EndDate));
    }
}

