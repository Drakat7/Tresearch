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
using TrialByFire.Tresearch.WebApi.Controllers.Contracts;
using TrialByFire.Tresearch.WebApi.Controllers.Implementations;
using Xunit;

namespace TrialByFire.Tresearch.Tests.UnitTests.UAD
{
	public class InMemoryUADControllerShould : InMemoryTestDependencies
	{
		public InMemoryUADControllerShould() : base()
		{
		}

		[Theory]
		[InlineData(2022, 3, 5, "success")]
		[InlineData(2021, 12, 12, "Error")]
		public void LoadKPI(int year, int month, int day, string expected)
		{
			// Arrange
			IMessageBank messageBank = new MessageBank();
			IAuthenticationService authenticationService = new AuthenticationService(SqlDAO, LogService, messageBank);
			IAuthorizationService authorizationService = new AuthorizationService(SqlDAO, LogService);
			IUADService uadService = new UADService(SqlDAO, LogService);
			IUADManager uadManager = new UADManager(SqlDAO, LogService, uadService, authenticationService, authorizationService);
			IUADController uadController = new UADController(SqlDAO, LogService, uadManager);

			// Act
			List<IKPI> results = new List<IKPI>();
			results = uadController.LoadKPI(new DateTime(year, month, day));

			// Assert
			Assert.Equal(expected, results[0].result);
		}
	}
}
