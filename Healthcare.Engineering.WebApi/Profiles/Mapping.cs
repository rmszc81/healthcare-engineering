using AutoMapper;

namespace Healthcare.Engineering.WebApi.Profiles;

public class Mapping : Profile
{
    public Mapping()
    {
        CreateMap<Healthcare.Engineering.Database.Model.Customer, Healthcare.Engineering.DataObject.Data.CustomerDto>().ReverseMap();
    }
}