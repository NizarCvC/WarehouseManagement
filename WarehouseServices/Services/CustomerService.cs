using Microsoft.Extensions.Logging;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;

namespace WarehouseServices.Services;

public class CustomerService(ICustomerRepository customerRepository,
    ILogger<CustomerService> logger) : ICustomerService
{
    public async Task<CustomerDto> GetCustomerByIdAsync(int customerId, CancellationToken ct)
    {
        Customer? customerInfo = await customerRepository.GetCustomerByIdAsync(customerId, ct);

        if (customerInfo is null)
            throw new NotFoundException($"The customer ID: {customerId} not exists.");

        logger.LogInformation("The customer id '{CustomerId}' is retrieved", customerId);
        return CustomerDto.FromEntity(customerInfo);
    }

    public async Task<CustomerDto> GetCustomerByEmailAsync(string email, CancellationToken ct)
    {
        Customer? customerInfo = await customerRepository.GetCustomerByEmailAsync(email, ct);

        if (customerInfo is null)
            throw new NotFoundException($"The customer email: {email} not exists.");

        logger.LogInformation("The customer email '{Email}' is retrieved", email);
        return CustomerDto.FromEntity(customerInfo);
    }

    public async Task<CustomerDto> GetCustomerByPhoneAsync(string phone, CancellationToken ct)
    {
        Customer? customerInfo = await customerRepository.GetCustomerByPhoneAsync(phone, ct);

        if (customerInfo is null)
            throw new NotFoundException($"The customer phone: {phone} not exists.");

        logger.LogInformation("The customer phone '{Phone}' is retrieved", phone);
        return CustomerDto.FromEntity(customerInfo);
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Customer> customers = await customerRepository.GetAllCustomersAsync(ct, page, pageSize);
        logger.LogInformation("The customers retrieved in page {Page} with page size {PageSize}", page, pageSize);
        return CustomerDto.FromEntities(customers);
    }

    public async Task<int> GetCustomersCountAsync(CancellationToken ct)
    {
        int count = await customerRepository.GetCustomersCountAsync(ct);
        logger.LogInformation("The customers count retrieved");
        return count;
    }

    public async Task<int> AddNewCustomerAsync(CreateCustomerDto customerDto, CancellationToken ct)
    {
        if (await customerRepository.IsCustomerExistsByEmailAsync(customerDto.Email, ct))
            throw new ConflictException($"The customer email {customerDto.Email} is already used.");

        if (await customerRepository.IsCustomerExistsByPhoneAsync(customerDto.Phone, ct))
            throw new ConflictException($"The customer phone {customerDto.Phone} is already used.");

        int newId = await customerRepository.AddNewCustomerAsync(customerDto, ct);

        if (newId == -1)
        {
            logger.LogError("Failed to add the customer with email '{Email}' and the phone '{Phone}' in the system.", customerDto.Email, customerDto.Phone);
            throw new InternalServerErrorException("Failed to add a new customer.");
        }

        logger.LogInformation("A new customer with name '{Name}' was added successfully with ID {newId}", customerDto.Name, newId);
        return newId;
    }

    public async Task UpdateCustomerAsync(int customerId, CreateCustomerDto customerDto, CancellationToken ct)
    {
        Customer? customerInfo = await customerRepository.GetCustomerByIdAsync(customerId, ct);

        if (customerInfo is null)
            throw new NotFoundException($"The customer ID: {customerId} not exists.");

        if (customerInfo.Email != customerDto.Email)
        {
            bool isEmailUsed = await customerRepository.IsCustomerExistsByEmailAsync(customerDto.Email, ct);

            if (isEmailUsed)
                throw new ConflictException($"The customer email {customerDto.Email} is already used.");
        }

        if (customerInfo.Phone != customerDto.Phone)
        {
            bool isPhoneUsed = await customerRepository.IsCustomerExistsByPhoneAsync(customerDto.Phone, ct);

            if (isPhoneUsed)
                throw new ConflictException($"The customer phone {customerDto.Phone} is already used.");
        }

        bool isSuccess = await customerRepository.UpdateCustomerAsync(customerId, customerDto, ct);

        if (!isSuccess)
        {
            logger.LogError("Failed to update the customer with id '{CustomerId}' in the system.", customerId);
            throw new InternalServerErrorException("Failed to update the customer.");
        }

        logger.LogInformation("The customer with ID '{CustomerId}' was updated successfully", customerId);
    }

    public async Task DeactivateCustomerAsync(int customerId, CancellationToken ct)
    {
        bool isSuccess = await customerRepository.DeactivateCustomerAsync(customerId, ct);

        if (!isSuccess)
            throw new NotFoundException($"The customer ID: {customerId} not exists.");

        logger.LogInformation("The customer with id '{CustomerId}' is deactivated", customerId);
    }

    public async Task<bool> IsCustomerExistsByIdAsync(int customerId, CancellationToken ct)
    {
        bool isFound = await customerRepository.IsCustomerExistsByIdAsync(customerId, ct);

        if (isFound)
            logger.LogDebug("The customer with id '{CustomerId}' is found.", customerId);
        else
            logger.LogDebug("The customer with id '{CustomerId}' is not found.", customerId);

        return isFound;
    }

    public async Task<bool> IsCustomerExistsByEmailAsync(string email, CancellationToken ct)
    {
        bool isFound = await customerRepository.IsCustomerExistsByEmailAsync(email, ct);

        if (isFound)
            logger.LogDebug("The customer with email '{Email}' is found.", email);
        else
            logger.LogDebug("The customer with email '{Email}' is not found.", email);

        return isFound;
    }

    public async Task<bool> IsCustomerExistsByPhoneAsync(string phone, CancellationToken ct)
    {
        bool isFound = await customerRepository.IsCustomerExistsByPhoneAsync(phone, ct);

        if (isFound)
            logger.LogDebug("The customer with phone '{Phone}' is found.", phone);
        else
            logger.LogDebug("The customer with phone '{Phone}' is not found.", phone);

        return isFound;
    }
}
