﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrialByFire.Tresearch.DAL.Contracts;
using TrialByFire.Tresearch.DAL.Implementations;
using TrialByFire.Tresearch.Managers.Contracts;
using TrialByFire.Tresearch.Managers.Implementations;
using TrialByFire.Tresearch.Models.Contracts;
using TrialByFire.Tresearch.Models.Implementations;
using TrialByFire.Tresearch.Services.Contracts;
using TrialByFire.Tresearch.Services.Implementations;
using Xunit;

namespace TrialByFire.Tresearch.Tests.AuthenticationTests.UnitTests
{
    public class InMemoryAuthenticationManagerShould : InMemoryTestDependencies
    {
        public InMemoryAuthenticationManagerShould() : base()
        {
        }

        [Theory]
        [InlineData("larry@gmail.com", "user", "ABCdef123", 2022, 3, 4, 5, 6, 0, "success")]
        [InlineData("billy@yahoo.com", "admin", "abcdef123", 2022, 3, 4, 5, 6, 0, "Data: Invalid Username or OTP. " +
            "Please try again.")]
        [InlineData("billy@yahoo.com", "admin", "abcdefghi", 2022, 3, 4, 5, 6, 0, "Data: Invalid Username or OTP. " +
            "Please try again.")]
        [InlineData("billyyahoocom", "admin", "ABCdef123", 2022, 3, 4, 5, 6, 0, "Data: Invalid Username or OTP. " +
            "Please try again.")]
        [InlineData("joe@outlook.com", "user", "ABCdef123", 2023, 3, 4, 5, 6, 0, "success")]
        [InlineData("bob@yahoo.com", "user", "ABCdef123", 2022, 3, 4, 5, 6, 0, "Database: The account was not found " +
            "or it has been disabled.")]
        [InlineData("harry@yahoo.com", "user", "ABCdef123", 2022, 3, 4, 5, 6, 0, "Database: Please click on the " +
            "confirmation link that we sent to your email in order to confirm your account.")]
        public void AuthenticateTheUser(string username, string role, string otp, int year, int month, int day, 
            int hour, int minute, int second, string expected)
        {
            // Arrange
            IRoleIdentity roleIdentity = new RoleIdentity(false, username, role);
            IRolePrincipal rolePrincipal = new RolePrincipal(roleIdentity);
            IAuthenticationManager authenticationManager = new AuthenticationManager(sqlDAO, 
                logService, validationService, authenticationService, rolePrincipal);
            DateTime now = new DateTime(year, month, day, hour, minute, second);

            // Act
            List<string> results = authenticationManager.Authenticate(username, otp, now);

            // Assert
            Assert.Equal(expected, results[0]);

            // Not unit test if connecting to outside db
            // Unit test if using in memory/turn into unit with mocking
        }

    }
}
