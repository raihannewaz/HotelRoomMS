using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Core.CommonModelProperties
{
    public class CommonModelProperty
    {
        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
        public long CreatedBy { get; set; }
        public long ModifiedBy { get; set; }
    }
}
