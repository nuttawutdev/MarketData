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
        public Process(Repository repository)
        {
            masterData = new MasterDataProcess(repository);
        }
    }
}
