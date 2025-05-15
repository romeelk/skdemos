using hotelreservation;

namespace HotelReservation;


public class Customer
{
    private readonly string firstName;
    private readonly string lastName;
    private readonly string email;
    
    public Customer(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
            
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));
            
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        this.firstName = firstName;
        this.lastName = lastName;
        this.email = email;
    }

    public string Name => $"{FirstName} {LastName}";

    public string FirstName => firstName;

    public string LastName => lastName;

    public string Email => email;
}
