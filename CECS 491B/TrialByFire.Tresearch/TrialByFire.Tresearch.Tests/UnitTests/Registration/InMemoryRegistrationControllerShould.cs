﻿using Xunit;
using TrialByFire.Tresearch.DAL.Contracts;
using TrialByFire.Tresearch.Services.Contracts;
using TrialByFire.Tresearch.Managers.Contracts;
using TrialByFire.Tresearch.Models.Contracts;
using TrialByFire.Tresearch.DAL.Implementations;
using TrialByFire.Tresearch.Managers.Implementations;
using TrialByFire.Tresearch.Services.Implementations;
using TrialByFire.Tresearch.Models.Implementations;
using TrialByFire.Tresearch.WebApi.Controllers.Contracts;
using Microsoft.AspNetCore.Mvc;
using TrialByFire.Tresearch.WebApi.Controllers.Implementations;

namespace TrialByFire.Tresearch.Tests.UnitTests.Registration
{

    public class InMemoryRegistrationControllerShould
    {
        ISqlDAO _sqlDAO { get; set; }
        ILogService _logService { get; set; }

        IRegistrationService _registrationService { get; set; }

        IMailService _mailService { get; set; }

        IValidationService _validationService { get; set; }

        IMessageBank _messageBank { get; set; }

        IRegistrationManager _registrationManager { get; set; }

        IRegistrationController _registrationController { get; set; }

        public InMemoryRegistrationControllerShould()
        {
            _sqlDAO = new InMemorySqlDAO();
            _messageBank = new MessageBank();
            _logService = new LogService(_sqlDAO, _messageBank);
            _registrationService = new RegistrationService(_sqlDAO, _logService);
            _mailService = new MailService(_messageBank);
            _validationService = new ValidationService(_messageBank);
            _registrationManager = new RegistrationManager(_sqlDAO, _logService, _registrationService, _mailService, _validationService, _messageBank);
            _registrationController = new RegistrationController(_sqlDAO, _logService, _registrationService, _mailService, _messageBank, _validationService, _registrationManager);
        }

        [Theory]
        [InlineData("pammypoor@gmail.com", "myValidPassphrase")]
        [InlineData("pammypoor@gmail.com", "ApplePie!")]
        public void RegisterTheUser(string email, string passphrase)
        {
            //Arrange
            IAccount account = new Account(email, email, passphrase, "User", true, false);
            //Act
            IActionResult results = _registrationController.RegisterAccount(email, passphrase);
            var objectResult = results as ObjectResult;

            //Assert
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Theory]
        [InlineData("pammypoor+INcontrollerSendConfirmation1@gmail.com")]
        [InlineData("pammypoor+INcontrollerSendConfirmation2@gmail.com")]
        public void SendConfirmation(string email)
        {
            //Arrange
            IAccount account = new Account(email, "temporaryPassword");

            //Act
            IActionResult results = _registrationController.SendConfirmation(email);
            var objectResult = results as ObjectResult;

            //Assert
            Assert.Equal(200, objectResult.StatusCode);
        }


        [Theory]
        [InlineData("pammypoor+INcontrollerConfirm1@gmail.com", "myControllerPass", "www.tresearch.systems/Registration/verify?=", "2022-03-06 21:32:59.910")]
        [InlineData("pammypoor+INcontrollerConfirm2@gmail.com", "myControllerPassword", "www.tresearch.systems/Registration/verify?=", "2022-03-06 21:32:59.910")]
        public void confirmAccount(string email, string passphrase, string url, string date)
        {
            //Arrange
            IAccount _account = new Account(email, email, passphrase, "User", true, false);
            IConfirmationLink _confirmationLink = new ConfirmationLink(email, Guid.NewGuid(), DateTime.Parse(date));

            _sqlDAO.CreateAccount(_account);
            _sqlDAO.CreateConfirmationLink(_confirmationLink);

            //Act
            IActionResult results = _registrationController.ConfirmAccount(url + _confirmationLink.UniqueIdentifier);
            var objectResult = results as ObjectResult;

            //Assert
            Assert.Equal(200, objectResult.StatusCode);
        }
    }
}
