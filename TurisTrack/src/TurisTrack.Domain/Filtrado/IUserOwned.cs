using System;

namespace TurisTrack
{
    public interface IUserOwned
    {
        Guid UserId { get; set; }
    }
}
