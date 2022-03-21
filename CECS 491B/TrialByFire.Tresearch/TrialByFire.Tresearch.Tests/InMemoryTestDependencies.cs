﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrialByFire.Tresearch.DAL.Contracts;
using TrialByFire.Tresearch.DAL.Implementations;
using TrialByFire.Tresearch.Models;
using TrialByFire.Tresearch.Models.Contracts;
using TrialByFire.Tresearch.Models.Implementations;
using TrialByFire.Tresearch.Services.Contracts;
using TrialByFire.Tresearch.Services.Implementations;

namespace TrialByFire.Tresearch.Tests
{
    public class InMemoryTestDependencies
    {
        private BuildSettingsOptions _buildSettingsOptions { get; }
        public IOptions<BuildSettingsOptions> BuildSettingsOptions { get; }
        public ISqlDAO SqlDAO { get; }
        public ILogService LogService { get; }
        public IMessageBank MessageBank { get; }
        public IAuthenticationService AuthenticationService { get; }
        public IAuthorizationService AuthorizationService { get; }
        public IValidationService ValidationService { get; }
        public IAccountDeletionService AccountDeletionService { get; }

        public InMemoryTestDependencies()
        {
            _buildSettingsOptions = new BuildSettingsOptions()
            {
                Environment = "Test",
                SqlConnectionString = "Server=MATTS-PC;Initial Catalog=TrialByFire.Tresearch.IntegrationTestDB; Integrated Security=true",
                SendGridAPIKey = ""
            };
            BuildSettingsOptions = Options.Create(_buildSettingsOptions) as IOptions<BuildSettingsOptions>;
            SqlDAO = new InMemorySqlDAO();
            LogService = new LogService(SqlDAO);
            MessageBank = new MessageBank();
            AuthenticationService = new AuthenticationService(SqlDAO, LogService, MessageBank);
            AuthorizationService = new AuthorizationService(SqlDAO, LogService);
            ValidationService = new ValidationService(MessageBank);
            AccountDeletionService = new AccountDeletionService(SqlDAO, LogService);
        }
    }
}
