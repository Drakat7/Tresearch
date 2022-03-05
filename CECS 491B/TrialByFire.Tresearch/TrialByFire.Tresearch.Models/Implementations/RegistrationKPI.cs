﻿using System;
using TrialByFire.Tresearch.Models.Contracts;

namespace TrialByFire.Tresearch.Models.Implementations
{
	public class RegistrationKPI : IRegistrationKPI
	{
		public List<int> registrationCount { get; }
		public RegistrationKPI(List<int> registrationCount)
		{
			this.registrationCount = registrationCount;
		}
	}
}

