﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TrialByFire.Tresearch.DAL.Contracts;
using TrialByFire.Tresearch.DAL.Implementations;
using TrialByFire.Tresearch.Services.Contracts;
using TrialByFire.Tresearch.Services.Implementations;
using Xunit;

namespace TrialByFire.Tresearch.Tests.AuthorizationTests
{
    public class AuthorizationServiceShould
    {
        public void VerifyThatTheUserIsAuthorized(IPrincipal rolePrincipal, string requiredRole)
        {
            // Arrange
            ISqlDAO sqlDAO = new SqlDAO();
            ILogService logService = new SqlLogService(sqlDAO);
            IAuthorizationService authorizationService = new AuthorizationService(sqlDAO, logService);
            string expected = "success";

            // Act
            string result = authorizationService.VerifyAuthorized(rolePrincipal, requiredRole);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
