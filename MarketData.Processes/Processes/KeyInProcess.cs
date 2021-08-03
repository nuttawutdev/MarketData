using MarketData.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Processes.Processes
{
    public class KeyInProcess
    {
        private readonly Repository repository;

        public KeyInProcess(Repository repository)
        {
            this.repository = repository;
        }
    }
}
