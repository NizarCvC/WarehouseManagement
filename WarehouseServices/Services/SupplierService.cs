using Microsoft.Extensions.Logging;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;

namespace WarehouseServices.Services;

public class SupplierService(ISupplierRepository supplierRepository,
    ILogger<SupplierService> logger) : ISupplierService
{
    public async Task<SupplierDto> GetSupplierByIdAsync(int supplierId, CancellationToken ct)
    {
        Supplier? supplierInfo = await supplierRepository.GetSupplierByIdAsync(supplierId, ct);

        if (supplierInfo is null)
            throw new NotFoundException($"The supplier ID: {supplierId} not exists.");

        logger.LogInformation("The supplier id '{SupplierId}' is retrieved", supplierId);
        return SupplierDto.FromEntity(supplierInfo);
    }

    public async Task<SupplierDto> GetSupplierByEmailAsync(string email, CancellationToken ct)
    {
        Supplier? supplierInfo = await supplierRepository.GetSupplierByEmailAsync(email, ct);

        if (supplierInfo is null)
            throw new NotFoundException($"The supplier email: {email} not exists.");

        logger.LogInformation("The supplier email '{Email}' is retrieved", email);
        return SupplierDto.FromEntity(supplierInfo);
    }

    public async Task<SupplierDto> GetSupplierByPhoneAsync(string phone, CancellationToken ct)
    {
        Supplier? supplierInfo = await supplierRepository.GetSupplierByPhoneAsync(phone, ct);

        if (supplierInfo is null)
            throw new NotFoundException($"The supplier phone: {phone} not exists.");

        logger.LogInformation("The supplier phone '{Phone}' is retrieved", phone);
        return SupplierDto.FromEntity(supplierInfo);
    }

    public async Task<SupplierDto> GetSupplierByTaxNumberAsync(string taxNumber, CancellationToken ct)
    {
        Supplier? supplierInfo = await supplierRepository.GetSupplierByTaxNumberAsync(taxNumber, ct);

        if (supplierInfo is null)
            throw new NotFoundException($"The supplier tax number: {taxNumber} not exists.");

        logger.LogInformation("The supplier tax number '{TaxNumber}' is retrieved", taxNumber);
        return SupplierDto.FromEntity(supplierInfo);
    }

    public async Task<List<SupplierDto>> GetAllSuppliersAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Supplier> suppliers = await supplierRepository.GetAllSuppliersAsync(ct, page, pageSize);
        logger.LogInformation("The suppliers retrieved in page {Page} with page size {PageSize}", page, pageSize);
        return SupplierDto.FromEntities(suppliers);
    }

    public async Task<int> GetSuppliersCountAsync(CancellationToken ct)
    {
        int count = await supplierRepository.GetSuppliersCountAsync(ct);
        logger.LogInformation("The suppliers count retrieved");
        return count;
    }

    public async Task<int> AddNewSupplierAsync(CreateSupplierDto supplierDto, CancellationToken ct)
    {
        if (await supplierRepository.IsSupplierExistsByEmailAsync(supplierDto.Email, ct))
            throw new ConflictException($"The supplier email {supplierDto.Email} is already used.");

        if (await supplierRepository.IsSupplierExistsByPhoneAsync(supplierDto.Phone, ct))
            throw new ConflictException($"The supplier phone {supplierDto.Phone} is already used.");

        if (await supplierRepository.IsSupplierExistsByTaxNumberAsync(supplierDto.TaxNumber, ct))
            throw new ConflictException($"The supplier tax number {supplierDto.TaxNumber} is already used.");

        int newId = await supplierRepository.AddNewSupplierAsync(supplierDto, ct);

        if (newId == -1)
        {
            logger.LogError("Failed to add the supplier with email '{Email}', phone '{Phone}', and tax number '{TaxNumber}' in the system.", supplierDto.Email, supplierDto.Phone, supplierDto.TaxNumber);
            throw new InternalServerErrorException("Failed to add a new supplier.");
        }

        logger.LogInformation("A new supplier with name '{Name}' was added successfully with ID {newId}", supplierDto.Name, newId);
        return newId;
    }

    public async Task UpdateSupplierAsync(int supplierId, CreateSupplierDto supplierDto, CancellationToken ct)
    {
        Supplier? supplierInfo = await supplierRepository.GetSupplierByIdAsync(supplierId, ct);

        if (supplierInfo is null)
            throw new NotFoundException($"The supplier ID: {supplierId} not exists.");

        if (supplierInfo.Email != supplierDto.Email)
        {
            bool isEmailUsed = await supplierRepository.IsSupplierExistsByEmailAsync(supplierDto.Email, ct);
            if (isEmailUsed)
                throw new ConflictException($"The supplier email {supplierDto.Email} is already used.");
        }

        if (supplierInfo.Phone != supplierDto.Phone)
        {
            bool isPhoneUsed = await supplierRepository.IsSupplierExistsByPhoneAsync(supplierDto.Phone, ct);
            if (isPhoneUsed)
                throw new ConflictException($"The supplier phone {supplierDto.Phone} is already used.");
        }

        if (supplierInfo.TaxNumber != supplierDto.TaxNumber)
        {
            bool isTaxNumberUsed = await supplierRepository.IsSupplierExistsByTaxNumberAsync(supplierDto.TaxNumber, ct);
            if (isTaxNumberUsed)
                throw new ConflictException($"The supplier tax number {supplierDto.TaxNumber} is already used.");
        }

        bool isSuccess = await supplierRepository.UpdateSupplierAsync(supplierId, supplierDto, ct);

        if (!isSuccess)
        {
            logger.LogError("Failed to update the supplier with id '{SupplierId}' in the system.", supplierId);
            throw new InternalServerErrorException("Failed to update the supplier.");
        }

        logger.LogInformation("The supplier with ID '{SupplierId}' was updated successfully", supplierId);
    }

    public async Task DeactivateSupplierAsync(int supplierId, CancellationToken ct)
    {
        bool isSuccess = await supplierRepository.DeactivateSupplierAsync(supplierId, ct);

        if (!isSuccess)
            throw new NotFoundException($"The supplier ID: {supplierId} not exists.");

        logger.LogInformation("The supplier with id '{SupplierId}' is deactivated", supplierId);
    }

    public async Task<bool> IsSupplierExistsByIdAsync(int supplierId, CancellationToken ct)
    {
        bool isFound = await supplierRepository.IsSupplierExistsByIdAsync(supplierId, ct);

        if (isFound)
            logger.LogDebug("The supplier with id '{SupplierId}' is found.", supplierId);
        else
            logger.LogDebug("The supplier with id '{SupplierId}' is not found.", supplierId);

        return isFound;
    }

    public async Task<bool> IsSupplierExistsByEmailAsync(string email, CancellationToken ct)
    {
        bool isFound = await supplierRepository.IsSupplierExistsByEmailAsync(email, ct);

        if (isFound)
            logger.LogDebug("The supplier with email '{Email}' is found.", email);
        else
            logger.LogDebug("The supplier with email '{Email}' is not found.", email);

        return isFound;
    }

    public async Task<bool> IsSupplierExistsByPhoneAsync(string phone, CancellationToken ct)
    {
        bool isFound = await supplierRepository.IsSupplierExistsByPhoneAsync(phone, ct);

        if (isFound)
            logger.LogDebug("The supplier with phone '{Phone}' is found.", phone);
        else
            logger.LogDebug("The supplier with phone '{Phone}' is not found.", phone);

        return isFound;
    }

    public async Task<bool> IsSupplierExistsByTaxNumberAsync(string taxNumber, CancellationToken ct)
    {
        bool isFound = await supplierRepository.IsSupplierExistsByTaxNumberAsync(taxNumber, ct);

        if (isFound)
            logger.LogDebug("The supplier with tax number '{TaxNumber}' is found.", taxNumber);
        else
            logger.LogDebug("The supplier with tax number '{TaxNumber}' is not found.", taxNumber);

        return isFound;
    }
}