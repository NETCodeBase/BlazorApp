using BlazorAzureSignalR.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorAzureSignalR.Services
{
    public class EmployeeService
    {
        AppDbContext dbContext = new AppDbContext();

        public async Task<List<Employee>> GetAllEmployees()
        {
            return await dbContext.Employee.AsNoTracking().ToListAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesRefresh()
        {
            return await dbContext.Employee.AsNoTracking().ToListAsync();
        }
    }
}
