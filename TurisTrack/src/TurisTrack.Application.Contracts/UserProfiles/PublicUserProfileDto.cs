using System;
using Volo.Abp.Application.Dtos;

namespace TurisTrack.UserProfiles
{
    public class PublicUserProfileDto : EntityDto<Guid>
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Foto { get; set; }
    }
}