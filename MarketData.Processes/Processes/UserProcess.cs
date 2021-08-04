﻿using MarketData.Model.Request.User;
using MarketData.Model.Response.User;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Processes.Processes
{
    public class UserProcess
    {
        private readonly Repository repository;

        public UserProcess(Repository repository)
        {
            this.repository = repository;
        }

        public LoginResponse Login(LoginRequest request)
        {
            LoginResponse response = new LoginResponse();

            try
            {
                var userData = repository.user.FindUserBy(c => c.UserName.ToLower() == request.userName.ToLower());

                if (userData != null)
                {
                    response.userID = userData.ID;
                }

            }
            catch (Exception ex)
            {

            }

            return response;
        }
    }
}
