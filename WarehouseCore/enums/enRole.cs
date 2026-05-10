namespace WarehouseCore.enums;

public enum enRole
{
    SystemAdministrator = 1,
    WarehouseManager = 2,
    SalesRepresentative = 3,
    PurchasingOfficer = 4,
    Accountant = 5
}

public static class RoleExtensions
{
    public static string ToRoleName(this enRole role)
    {
        return role switch
        {
            enRole.SystemAdministrator => "System Administrator",
            enRole.WarehouseManager => "Warehouse Manager",
            enRole.SalesRepresentative => "Sales Representative",
            enRole.PurchasingOfficer => "Purchasing Officer",
            enRole.Accountant => "Accountant",
            _ => role.ToString()
        };
    }
}