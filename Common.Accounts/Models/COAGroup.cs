using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Models
{
    public class COAGroup : CommonModelProperty
    {
        public long Id { get; private set; }
        public long ParentId { get; private set; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        public static COAGroup Create(long id, long parentId, string name, long userId)
        {
            return new COAGroup
            {
                Id = id,
                ParentId = parentId,
                Name = name,
                IsActive = true,
                Created = DateTimeConversion.UTCToBST(),
                CreatedBy = userId
            };
        }

        public void Update(long parentId, string name, long userId)
        {
            ParentId = parentId;
            Name = name;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }
    }
}
