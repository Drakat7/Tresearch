﻿using TrialByFire.Tresearch.DAL;
using TrialByFire.Tresearch.DomainModels;
using TrialByFire.Tresearch.Logging;

namespace TrialByFire.Tresearch.UserManagement
{
    public class AccountService
    {
        MSSQLDAO mssqlDAO;
        LogService logService;

        public AccountService(MSSQLDAO mssqlDAO, LogService logService)
        {
            this.mssqlDAO = mssqlDAO;
            this.logService = logService;
        }

        public bool CreateAccount(string email, string passphrase, string authorizationLevel)
        {
            Account account = new Account(email, passphrase, authorizationLevel);
            return true;
        }

        public bool EnableAccount(string username, string email)
        {
            bool isEnabled = false;
            try
            {
                isEnabled = mssqlDAO.EnableAccount(username, email);

            }
            catch (Exception e)
            {
                isEnabled = false;
            }
            return isEnabled;
        }

        public bool DisableAccount(string username)
        {
            bool isDisabled = false;
            try
            {
                isDisabled = mssqlDAO.DisableAccount(username);

            }
            catch (Exception e)
            {
                isDisabled = false;
            }
            return isDisabled;
        }
    }
}
