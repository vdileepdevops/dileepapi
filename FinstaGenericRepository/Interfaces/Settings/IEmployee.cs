using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FinstaInfrastructure.Settings;

namespace FinstaRepository.Interfaces.Settings
{
    public interface IEmployee
    {        
       bool SaveEmployeeRole(EmployeeRole employeeRole, string connectionstring);
       bool SaveEmployeeDetails(EmployeeDTO employeeDetails, string connectionString);
        Task<List<EmployeeDTO>> GetallEmployeeDetails(string connectionString);
       int checkEmployeeCountinMaster(EmployeeDTO employeeObj,string connectionString);
       Task< EmployeeDTO> GetEmployeeDetailsOnId(long employeeId, string connectionString);
        bool UpdateEmployeeData(EmployeeDTO employeeUpdateDTO, string connectionString);
        int checkEmployeeRoleExistsOrNot(string Rolename, string connectionString);

    }
}
