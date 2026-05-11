using WarehouseCore.Entities;

namespace WarehouseCore.DTOs.ReadDTOs;

public class CustomerDto
{
    public int CustomerID { get; set; }
    public required string Name { get; set; }
    public required string Phone { get; set; }
    public required string Email { get; set; }
    public required string Address { get; set; }
    public bool IsActive { get; set; }

    public static CustomerDto FromEntity(Customer customer)
    {
        return new CustomerDto()
        {
            CustomerID = customer.CustomerID,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            IsActive = customer.IsActive
        };
    }

    public static List<CustomerDto> FromEntities(List<Customer> customers)
    {
        return customers.Select(FromEntity).ToList();
    }
}
