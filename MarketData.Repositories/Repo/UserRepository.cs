﻿using MarketData.Helper;
using MarketData.Model.Entiry;
using MarketData.Model.Request.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Repositories.Repo
{
    public class UserRepository
    {
        private readonly MarketDataDBContext _dbContext;
        public UserRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TMUser FindUserBy(Expression<Func<TMUser, bool>> expression)
        {
            try
            {
                return _dbContext.TMUser.Where(expression).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TMUser> GetUserBy(Expression<Func<TMUser, bool>> expression)
        {
            try
            {
                return _dbContext.TMUser.Where(expression).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<TMUserCounter> GetUserCounterBy(Expression<Func<TMUserCounter, bool>> expression)
        {
            try
            {
                return _dbContext.TMUserCounter.Where(expression).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TMUser> CreateNewUser(SaveUserDataRequest request,string passsword)
        {
            try
            {
                TMUser newUser = new TMUser
                {
                    ID = Guid.NewGuid(),
                    Email = request.email,
                    Password = passsword,
                    DisplayName = request.displayName,
                    FirstName = request.firstName,
                    LastName = request.lastName,
                    ActiveFlag = request.active,
                    ValidateEmailFlag = request.validateEmail,
                    ViewMasterPermission = request.viewMaster,
                    EditMasterPermission = request.editMaster,
                    EditUserPermission = request.editUser,
                    ViewDataPermission = request.viewData,
                    KeyInDataPermission = request.keyInData,
                    ApprovePermission = request.approveData,
                    ViewReportPermission = request.viewReport,
                    Create_By = request.actionBy,
                    Create_Date = Utility.GetDateNowThai()
                };

                _dbContext.TMUser.Add(newUser);

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return newUser;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateUserData(SaveUserDataRequest request)
        {
            try
            {
                var userData = _dbContext.TMUser.Find(request.userID);
                userData.Email = request.email;
                userData.DisplayName = request.displayName;
                userData.FirstName = request.firstName;
                userData.LastName = request.lastName;
                userData.ActiveFlag = request.active;
                userData.ValidateEmailFlag = request.validateEmail;
                userData.ViewMasterPermission = request.viewMaster;
                userData.EditMasterPermission = request.editMaster;
                userData.EditUserPermission = request.editUser;
                userData.ViewDataPermission = request.viewData;
                userData.KeyInDataPermission = request.keyInData;
                userData.ApprovePermission = request.approveData;
                userData.ViewReportPermission = request.viewReport;
                userData.Update_By = request.actionBy;
                userData.Update_Date = Utility.GetDateNowThai();

                _dbContext.TMUser.Update(userData);

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAllUserCounterByID(Guid userID)
        {
            try
            {
                var userCounterData = _dbContext.TMUserCounter.Where(c => c.User_ID == userID);

                if (userCounterData.Any())
                {
                    _dbContext.TMUserCounter.RemoveRange(userCounterData);
                    return await _dbContext.SaveChangesAsync() > 0;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CreateUserCounter(List<TMUserCounter> listUserCounter)
        {
            try
            {
                _dbContext.TMUserCounter.AddRange(listUserCounter);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
