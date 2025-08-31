using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Model
{
    public class EmployeeModel
    {
        public int EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? DateCreated { get; set; }
        public string? Gmail { get; set; }
    }
}
