﻿using Microsoft.AspNetCore.Mvc;
using TrialByFire.Tresearch.DAL.Contracts;
using TrialByFire.Tresearch.Managers.Contracts;
using TrialByFire.Tresearch.WebApi.Controllers.Contracts;

namespace TrialByFire.Tresearch.WebApi.Controllers.Implementations
{
    public class AuthenticationController : Controller, IAuthenticationController
    {
        private ISqlDAO _sqlDAO { get; }
        private ILogService _logService { get; }
        private IAuthenticationManager _authenticationManager { get; }

        public AuthenticationController(ISqlDAO sqlDAO, ILogService logService, IAuthenticationManager authenticationManager)
        {
            _sqlDAO = sqlDAO;
            _logService = logService;
            _authenticationManager = authenticationManager;
        }

        // IEnumerable may be faster than using lists, gives compiler chance to defer work to later, possibly optimizing in the process
        [HttpPost]
        public string Authenticate(string username, string otp)
        {
            List<string> results = _authenticationManager.Authenticate(username, otp, DateTime.Now);
            string result = results[0];
            if(result.Equals("success"))
            {
                result = CreateCookie(results[1]);
                if(result.Equals("success"))
                {
                    _logService.CreateLog(DateTime.Now, "Server", username, "Info", "Authentication Succeeded");
                    return result;
                }
            }

            // {category}: {error message}
            string[] error = result.Split(": ");
            _logService.CreateLog(DateTime.Now, error[0], username, "Error", error[1]);
            return result;
        }

        private string CreateCookie(string jwtToken)
        {
            // unsure of what errors could actually occur here
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.IsEssential = true;
            cookieOptions.Expires = DateTime.Now.AddDays(5);
            cookieOptions.Secure = true;
            Response.Cookies.Append("AuthN", jwtToken, cookieOptions);
            return "success";
        }

    }
}
