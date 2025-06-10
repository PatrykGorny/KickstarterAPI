using AutoMapper;
using Infractructure.EF;
using KickstarterAPI.Dto.Kickstarter;

namespace KickstarterAPI.MapperProfiles;

public class KickstarterProfile : Profile
{
    public KickstarterProfile()
    {
        CreateMap<KickstarterEntity, KickstarterProjectDto>();
        CreateMap<KickstarterCreateDto, KickstarterEntity>();
    }
}