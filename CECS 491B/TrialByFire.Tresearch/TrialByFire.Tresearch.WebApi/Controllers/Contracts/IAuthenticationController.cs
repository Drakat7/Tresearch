﻿namespace TrialByFire.Tresearch.WebApi.Controllers.Contracts
{
    public interface IAuthenticationController
    {
        public string Authenticate(string username, string otp);
    }
}
