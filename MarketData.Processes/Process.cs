using MarketData.Processes.Processes;
using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Processes
{
    public class Process
    {
        public MasterDataProcess masterData;
        public KeyInProcess keyIn;
        public UserProcess user;
        public ApproveProcess approve;

        public Process(Repository repository)
        {
            masterData = new MasterDataProcess(repository);
            keyIn = new KeyInProcess(repository);
            user = new UserProcess(repository);
            approve = new ApproveProcess(repository);
        }
    }
}
