using MarketData.Model.Response.Approve;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Processes.Processes
{
    public class ApproveProcess
    {
        private readonly Repository repository;

        public ApproveProcess(Repository repository)
        {
            this.repository = repository;
        }

        public GetApproveKeyInListResponse GetApproveKeyInList()
        {
            GetApproveKeyInListResponse response = new GetApproveKeyInListResponse();

            try
            {

            }
            catch(Exception ex)
            {

            }

            return response;
        }
    }
}
