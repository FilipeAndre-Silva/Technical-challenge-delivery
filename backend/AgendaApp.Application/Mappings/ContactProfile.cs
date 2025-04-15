using AgendaApp.Application.DTOs.Contact;
using AgendaApp.Application.Features.Contact.Commands;
using AgendaApp.Domain.Entities;
using AutoMapper;

namespace AgendaApp.Application.Mappings;

public class ContactProfile : Profile
{
    public ContactProfile()
    {
        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<CreateContactCommand, Contact>()
            .ConstructUsing(src => new Contact(src.Name, src.Email, src.Phone, Guid.NewGuid())).ReverseMap();
    }
}