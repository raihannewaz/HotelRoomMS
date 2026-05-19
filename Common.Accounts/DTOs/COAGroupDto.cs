using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.DTOs
{
    public class COAGroupDto
    {
        public long Id { get;  set; }
        public string Name { get;  set; }
        public long ParentId { get;  set; }
        public string ParentName { get;  set; }
        public bool IsActive { get;  set; }
    }
}
