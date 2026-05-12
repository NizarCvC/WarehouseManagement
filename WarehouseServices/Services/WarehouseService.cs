using Microsoft.Extensions.Logging;
using WarehouseCore.DTOs.CreateDTOs;
using WarehouseCore.DTOs.ReadDTOs;
using WarehouseCore.Entities;
using WarehouseDataAccess.Interfaces;
using WarehouseServices.Exceptions;
using WarehouseServices.Interfaces;

namespace WarehouseServices.Services;

public class WarehouseService(IWarehouseRepository warehouseRepository,
    ILogger<WarehouseService> logger) : IWarehouseService
{
    public async Task<WarehouseDto?> GetWarehouseByIdAsync(int warehouseId, CancellationToken ct)
    {
        Warehouse? warehouseInfo = await warehouseRepository.GetWarehouseByIdAsync(warehouseId, ct);

        if (warehouseInfo is null)
            throw new NotFoundException($"The warehouse ID: {warehouseId} not exists.");

        logger.LogInformation("The warehouse id '{WarehouseId}' is retrieved", warehouseId);
        return WarehouseDto.FromEntity(warehouseInfo);
    }

    public async Task<WarehouseDto?> GetWarehouseByNameAsync(string name, CancellationToken ct)
    {
        Warehouse? warehouseInfo = await warehouseRepository.GetWarehouseByNameAsync(name, ct);

        if (warehouseInfo is null)
            throw new NotFoundException($"The warehouse name: {name} not exists.");

        logger.LogInformation("The warehouse name '{Name}' is retrieved", name);
        return WarehouseDto.FromEntity(warehouseInfo);
    }

    public async Task<WarehouseDto?> GetWarehouseByCodeAsync(string code, CancellationToken ct)
    {
        Warehouse? warehouseInfo = await warehouseRepository.GetWarehouseByCodeAsync(code, ct);

        if (warehouseInfo is null)
            throw new NotFoundException($"The warehouse code: {code} not exists.");

        logger.LogInformation("The warehouse code '{Code}' is retrieved", code);
        return WarehouseDto.FromEntity(warehouseInfo);
    }

    public async Task<List<WarehouseDto>> GetAllWarehousesAsync(CancellationToken ct, int page = 1, int pageSize = 10)
    {
        List<Warehouse> warehouses = await warehouseRepository.GetAllWarehousesAsync(ct, page, pageSize);
        logger.LogInformation("The warehouses retrieved in page {Page} with page size {PageSize}", page, pageSize);
        return WarehouseDto.FromEntities(warehouses);
    }

    public async Task<int> GetWarehousesCountAsync(CancellationToken ct)
    {
        int count = await warehouseRepository.GetWarehousesCountAsync(ct);
        logger.LogInformation("The warehouses count retrieved");
        return count;
    }

    public async Task<int> AddNewWarehouseAsync(CreateWarehouseDto warehouseDto, CancellationToken ct)
    {
        if (await warehouseRepository.IsWarehouseExistsByNameAsync(warehouseDto.Name, ct))
            throw new ConflictException($"The warehouse name {warehouseDto.Name} is already used.");

        if (await warehouseRepository.IsWarehouseExistsByCodeAsync(warehouseDto.Code, ct))
            throw new ConflictException($"The warehouse code {warehouseDto.Code} is already used.");

        int newId = await warehouseRepository.AddNewWarehouseAsync(warehouseDto, ct);

        if (newId == -1)
        {
            logger.LogError("Failed to add the warehouse with name '{Name}' in the system.", warehouseDto.Name);
            throw new InternalServerErrorException("Failed to add the warehouse.");
        }

        logger.LogInformation("A new warehouse with name '{Name}' was added successfully with ID {newId}", warehouseDto.Name, newId);
        return newId;
    }

    public async Task UpdateWarehouseAsync(int warehouseId, CreateWarehouseDto warehouseDto, CancellationToken ct)
    {
        Warehouse? warehouseInfo = await warehouseRepository.GetWarehouseByIdAsync(warehouseId, ct);

        if (warehouseInfo is null)
            throw new NotFoundException($"The warehouse ID: {warehouseId} not exists.");

        if (warehouseInfo.Name != warehouseDto.Name)
        {
            bool isNameUsed = await warehouseRepository.IsWarehouseExistsByNameAsync(warehouseDto.Name, ct);

            if (isNameUsed)
                throw new ConflictException($"The warehouse name {warehouseDto.Name} is already used.");
        }

        if (warehouseInfo.Code != warehouseDto.Code)
        {
            bool isCodeUsed = await warehouseRepository.IsWarehouseExistsByCodeAsync(warehouseDto.Code, ct);

            if (isCodeUsed)
                throw new ConflictException($"The warehouse code {warehouseDto.Code} is already used.");
        }

        bool isSuccess = await warehouseRepository.UpdateWarehouseAsync(warehouseId, warehouseDto, ct);

        if (!isSuccess)
        {
            logger.LogError("Failed to update the warehouse with id '{warehouseId}' in the system.", warehouseId);
            throw new InternalServerErrorException("Failed to update the warehouse.");
        }

        logger.LogInformation("The warehouse with ID '{WarehouseId}' was updated successfully", warehouseId);
    }

    public async Task DeactivateWarehouseAsync(int warehouseId, CancellationToken ct)
    {
        bool isSuccess = await warehouseRepository.DeactivateWarehouseAsync(warehouseId, ct);

        if (!isSuccess)
            throw new NotFoundException($"The warehouse with ID: {warehouseId} not exists.");

        logger.LogInformation("The warehouse with ID '{WarehouseId}' was deactivated", warehouseId);
    }

    public async Task<bool> IsWarehouseExistsByIdAsync(int warehouseId, CancellationToken ct)
    {
        bool isFound = await warehouseRepository.IsWarehouseExistsByIdAsync(warehouseId, ct);

        if (isFound)
            logger.LogDebug("The warehouse with id '{WarehouseId}' is found.", warehouseId);
        else
            logger.LogDebug("The warehouse with id '{WarehouseId}' is not found.", warehouseId);

        return isFound;
    }

    public async Task<bool> IsWarehouseExistsByNameAsync(string name, CancellationToken ct)
    {
        bool isFound = await warehouseRepository.IsWarehouseExistsByNameAsync(name, ct);

        if (isFound)
            logger.LogDebug("The warehouse with name '{Name}' is found.", name);
        else
            logger.LogDebug("The warehouse with name '{Name}' is not found.", name);

        return isFound;
    }

    public async Task<bool> IsWarehouseExistsByCodeAsync(string code, CancellationToken ct)
    {
        bool isFound = await warehouseRepository.IsWarehouseExistsByCodeAsync(code, ct);

        if (isFound)
            logger.LogDebug("The warehouse with code '{Code}' is found.", code);
        else
            logger.LogDebug("The warehouse with code '{Code}' is not found.", code);

        return isFound;
    }
}
