using AuthSystem.Identity.Models;
using Common.Accounts.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Accounts.Data;

/// <summary>
/// Called once at startup from Program.cs via:
///   await IdentityModuleSeeder.SeedAsync(app.Services);
/// </summary>
public static class AccountsModuleSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var groups = new List<COAGroupDto>
                     {
                         new() { Id = 1, Name = "Assets", ParentId = 0, IsActive = true },
                         new() { Id = 2, Name = "Current Assets", ParentId = 1, IsActive = true },
                         new() { Id = 3, Name = "Non Current Assets", ParentId = 1, IsActive = true },
                     
                         new() { Id = 4, Name = "Liabilities", ParentId = 0, IsActive = true },
                         new() { Id = 5, Name = "Current Liabilities", ParentId = 4, IsActive = true },
                         new() { Id = 6, Name = "Non Current Liabilities", ParentId = 4, IsActive = true },
                     
                         new() { Id = 7, Name = "Equity", ParentId = 0, IsActive = true },
                     
                         new() { Id = 8, Name = "Income", ParentId = 0, IsActive = true },
                         new() { Id = 9, Name = "Operating Income", ParentId = 8, IsActive = true },
                         new() { Id = 10, Name = "Other Income", ParentId = 8, IsActive = true },
                     
                         new() { Id = 11, Name = "Expenses", ParentId = 0, IsActive = true },
                         new() { Id = 12, Name = "Cost Of Goods Sold", ParentId = 11, IsActive = true },
                         new() { Id = 13, Name = "Operating Expenses", ParentId = 11, IsActive = true },
                         new() { Id = 14, Name = "Other Expenses", ParentId = 11, IsActive = true },
                     };


        var coas = new List<COADto>
                     {
                         new() { Id = 1, Name = "Cash", GroupId = 2, ParentId = 0, IsActive = true },
                         new() { Id = 2, Name = "Cash In Hand", GroupId = 2, ParentId = 1, IsActive = true },
                         new() { Id = 3, Name = "Bank Account", GroupId = 2, ParentId = 1, IsActive = true },
                     
                         new() { Id = 4, Name = "Accounts Receivable", GroupId = 2, ParentId = 0, IsActive = true },
                         new() { Id = 5, Name = "Inventory", GroupId = 2, ParentId = 0, IsActive = true },
                         new() { Id = 6, Name = "Prepaid Expense", GroupId = 2, ParentId = 0, IsActive = true },
                     
                         new() { Id = 7, Name = "Fixed Assets", GroupId = 3, ParentId = 0, IsActive = true },
                         new() { Id = 8, Name = "Furniture", GroupId = 3, ParentId = 7, IsActive = true },
                         new() { Id = 9, Name = "Equipment", GroupId = 3, ParentId = 7, IsActive = true },
                         new() { Id = 10, Name = "Vehicle", GroupId = 3, ParentId = 7, IsActive = true },

                         new() { Id = 11, Name = "Accounts Payable", GroupId = 5, ParentId = 0, IsActive = true },
                         new() { Id = 12, Name = "VAT Payable", GroupId = 5, ParentId = 0, IsActive = true },
                         new() { Id = 13, Name = "Salary Payable", GroupId = 5, ParentId = 0, IsActive = true },
                         new() { Id = 14, Name = "Short Term Loan", GroupId = 5, ParentId = 0, IsActive = true },
                         new() { Id = 15, Name = "Long Term Loan", GroupId = 6, ParentId = 0, IsActive = true },

                         new() { Id = 16, Name = "Owner Capital", GroupId = 7, ParentId = 0, IsActive = true },
                         new() { Id = 17, Name = "Retained Earnings", GroupId = 7, ParentId = 0, IsActive = true },
                         new() { Id = 18, Name = "Drawings", GroupId = 7, ParentId = 0, IsActive = true },

                         new() { Id = 19, Name = "Sales Revenue", GroupId = 9, ParentId = 0, IsActive = true },
                         new() { Id = 20, Name = "Service Revenue", GroupId = 9, ParentId = 0, IsActive = true },
                         new() { Id = 21, Name = "Discount Received", GroupId = 10, ParentId = 0, IsActive = true },

                         new() { Id = 22, Name = "Purchase", GroupId = 12, ParentId = 0, IsActive = true },
                         new() { Id = 23, Name = "Salary Expense", GroupId = 13, ParentId = 0, IsActive = true },
                         new() { Id = 24, Name = "Rent Expense", GroupId = 13, ParentId = 0, IsActive = true },
                         new() { Id = 25, Name = "Utility Expense", GroupId = 13, ParentId = 0, IsActive = true },
                         new() { Id = 26, Name = "Office Expense", GroupId = 13, ParentId = 0, IsActive = true },
                         new() { Id = 27, Name = "Bank Charge", GroupId = 14, ParentId = 0, IsActive = true },
                    };
    }
}
