using MarketData.Model.Entiry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketData.Repositories.Repo
{
    public class ApproveRepository
    {
        private readonly MarketDataDBContext _dbContext;

        public ApproveRepository(MarketDataDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CreateApproveKeyInData(TTBAKeyIn request)
        {
            try
            {
                var submittedStatus = _dbContext.TMApproveStatus.FirstOrDefault(c => c.Status_Name == "Submitted");

                TTApproveKeyIn aaproveKeyInData = new TTApproveKeyIn
                {
                    ID = Guid.NewGuid(),
                    BAKeyIn_ID = request.ID,
                    Status_ID = submittedStatus.ID,
                };

                _dbContext.TTApproveKeyIn.Add(aaproveKeyInData);
                return await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
