using System;
using System.Collections.Generic;
using System.Text;

using Banking.Core.Models;

namespace Banking.Core.Interfaces
{
    public interface IProfileService
    {
        ProfileInformation AddProfileInformation(ProfileInformation profileInformation);
        IEnumerable<ProfileInformation> GetAllProfileInformation();
        ProfileInformation GetByIDProfileInformation(Guid id);
        ProfileInformation GetProfileInformationByEmail(string userName);
    }

}

