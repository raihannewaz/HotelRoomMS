using Common.Core.CommonModelProperties;
using Common.Core.DateTimeConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Models
{
    public class ChartOfAccount : CommonModelProperty
    {
        public long Id { get; private set; }
        public long GroupId { get; private set; }
        public long ParentId { get; private set; }
        public string Name { get; private set; }
        public bool IsActive { get; private set; }

        public static ChartOfAccount Create(long id, long groupId, long parentId, string name, long userId)
        {
            return new ChartOfAccount
            {
                Id = id,
                GroupId = groupId,
                ParentId = parentId,
                Name = name,
                IsActive = true,
                CreatedBy = userId,
                Created = DateTimeConversion.UTCToBST()
            };
        }

        public void Update(long groupId, long parentId, string name, long userId)
        {
            GroupId = groupId;
            ParentId = parentId;
            Name = name;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }
         public void ActiveStatus(long userId)
        {
            IsActive = !IsActive;
            LastModified = DateTimeConversion.UTCToBST();
            ModifiedBy = userId;
        }

    }
}
