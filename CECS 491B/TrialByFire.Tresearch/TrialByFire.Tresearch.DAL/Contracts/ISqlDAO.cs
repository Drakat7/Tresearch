﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrialByFire.Tresearch.Models.Contracts;
using TrialByFire.Tresearch.Models.Implementations;

namespace TrialByFire.Tresearch.DAL.Contracts
{
    public interface ISqlDAO
    {
        public List<string> CreateAccount(IAccount account);
        public List<string> CreateConfirmationLink(IConfirmationLink _confirmationlink);

        public List<string> ConfirmAccount(IAccount account);

        public List<string> RemoveConfirmationLink(IConfirmationLink confirmationLink);
        public IConfirmationLink GetConfirmationLink(string url);

        public IAccount GetUnconfirmedAccount(string email);

        // Authentication
        public string VerifyAccount(IAccount account);
        public List<string> Authenticate(IOTPClaim otpClaim);

        // Authorization
        public string VerifyAuthorized(IRolePrincipal rolePrincipal, string requiredAuthLevel);

        // Request OTP
        public string StoreOTP(IOTPClaim otpClaim);

        // Usage Analysis Dashboard
        public List<IKPI> LoadKPI(DateTime now);

        // Delete account
        public string DeleteAccount(IRolePrincipal rolePrincipal);

        /*
            Ian's Methods
         */

        public string CreateNodesCreated(INodesCreated nodesCreated);

        public List<NodesCreated> GetNodesCreated(DateTime nodeCreationDate);

        public string UpdateNodesCreated(INodesCreated nodesCreated);



        public string CreateDailyLogin(IDailyLogin dailyLogin);

        public List<DailyLogin> GetDailyLogin(DateTime nodeCreationDate);

        public string UpdateDailyLogin(IDailyLogin dailyLogin);


        public string CreateTopSearch(ITopSearch topSearch);

        public List<TopSearch> GetTopSearch(DateTime nodeCreationDate);

        public string UpdateTopSearch(ITopSearch topSearch);


        public string CreateDailyRegistration(IDailyRegistration dailyRegistration);

        public List<DailyRegistration> GetDailyRegistration(DateTime nodeCreationDate);

        public string UpdateDailyRegistration(IDailyRegistration dailyRegistration);
    }
}
