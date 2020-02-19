using System;
using System.Collections.Generic;
using System.Text;

using Banking.Core.Models;

namespace Banking.Core.Interfaces
{
    public interface IUserService
    {
        AuthenticateModel Authenticate(AuthenticateModel authenticateModel);
        AuthenticateModel GetByIdAuthenticationModel(Guid id);

        ProfileInformation AddProfileInformation(ProfileInformation profileInformation);
        AuthenticateModel AddAuthenticationModel(AuthenticateModel authenticationModel);

        IEnumerable<AuthenticateModel> GetAllAuthenticationModels();
        IEnumerable<ProfileInformation> GetAllProfileInformation();

        ProfileInformation GetByIDProfileInformation(Guid id);
    }
}
