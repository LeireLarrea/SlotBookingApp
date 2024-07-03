//using AutoMapper;
//using SlotBookingApp.Infrastructure.Dtos;
//using SlotBookingApp.Domain.Entities;

//public class MappingProfile : Profile
//{
//    public MappingProfile()
//    {
//        CreateMap<CalendarEvent, SlotBookingDto>()
//            .ForMember(dest => dest.FacilityId, opt => opt.MapFrom(src => src.FacilityId))
//            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start))
//            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End))
//            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
//            .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => new PatientDto
//            {
//                Name = src.Name,
//                SecondName = src.SecondName,
//                Email = src.Email,
//                Phone = src.Phone
//            }));
//    }
//}
