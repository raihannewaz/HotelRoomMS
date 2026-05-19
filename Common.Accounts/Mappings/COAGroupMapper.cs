using AutoMapper;
using Common.Accounts.DTOs;
using Common.Accounts.Features.COAGroups.CreateCOAGroups;
using Common.Accounts.Features.COAGroups.GetCOAGroupsGrid;
using Common.Accounts.Features.COAGroups.UpdateCOAGroups;
using Common.Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Accounts.Mappings
{
    internal class COAGroupMapper : Profile
    {
        public COAGroupMapper()
        {
            CreateMap<COAGroup, COAGroupDto>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
                .ForMember(x => x.ParentId, opt => opt.MapFrom(x => x.ParentId))
                .ForMember(x => x.IsActive, opt => opt.MapFrom(x => x.IsActive));

            CreateMap<CreateCOAGroup, COAGroup>();
            CreateMap<CreateCOAGroupRequest, CreateCOAGroup>();
            //.ConstructUsing(req => new CreateCOAGroup(req));

            CreateMap<UpdateCOAGroup, COAGroup>();
            CreateMap<UpdateCOAGroupRequest, UpdateCOAGroup>();
            //.ConstructUsing(req => new UpdateCOAGroup(req));

            CreateMap<GetCOAGroupGridResquest, GetCOAGroupGrid>();
        }
    }
}
