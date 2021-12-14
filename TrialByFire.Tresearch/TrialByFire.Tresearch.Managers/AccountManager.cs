<<<<<<< HEAD
﻿using System;
using TrialByFire.Tresearch.DAL;
using TrialByFire.Tresearch.DomainModels;
using TrialByFire.Tresearch.Logging;
=======
﻿using TrialByFire.Tresearch.DomainModels;
using TrialByFire.Tresearch.DAL;
using TrialByFire.Tresearch.Logging;
using TrialByFire.Tresearch.Services;
>>>>>>> c7d05deb0957b214fada0d7800c07f0ce22366fe

namespace TrialByFire.Tresearch.UserManagement
{
    public class AccountManager
    {
        private MSSQLDAO mssqlDAO;
        private LogService logService;
        private Account userAccount;

        public AccountManager(MSSQLDAO mssqlDAO, LogService logService)
        {
            this.mssqlDAO = mssqlDAO;
            this.logService = logService;
        }

        public bool CreateAccount(string email, string passphrase, string authorizationLevel)
        {
            throw new NotImplementedException();
        }

        public bool EnableAccount(string username, string email)
        {
            AccountService accountService = new AccountService(mssqlDAO, logService);
        }

        public bool DisableAccount(string username)
        {
            AccountService accountService = new AccountService(mssqlDAO, logService);
            bool isEnabled = accountService.EnableAccount(username);
        }
    }
}