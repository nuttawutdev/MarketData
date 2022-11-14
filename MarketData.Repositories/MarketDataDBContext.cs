using MarketData.Model.Entiry;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Repositories
{
    public class MarketDataDBContext : DbContext
    {
        public MarketDataDBContext(DbContextOptions<MarketDataDBContext> options) : base(options)
        {
        }

        public DbSet<TMBrand> TMBrand { get; set; }
        public DbSet<TMBrandGroup> TMBrandGroup { get; set; }
        public DbSet<TMBrandSegment> TMBrandSegment { get; set; }
        public DbSet<TMBrandType> TMBrandType { get; set; }
        public DbSet<TMCounter> TMCounter { get; set; }
        public DbSet<TMDepartmentStore> TMDepartmentStore { get; set; }
        public DbSet<TMDistributionChannel> TMDistributionChannel { get; set; }
        public DbSet<TMRetailerGroup> TMRetailerGroup { get; set; }
        public DbSet<TMRegion> TMRegion { get; set; }
        public DbSet<TMUserCounter> TMUserCounter { get; set; }
        public DbSet<TTBAKeyIn> TTBAKeyIn { get; set; }
        public DbSet<TTBAKeyInDetail> TTBAKeyInDetail { get; set; }
        public DbSet<TMKeyInRemark> TMKeyInRemark { get; set; }
        public DbSet<TTAdminKeyInDetail> TTAdminKeyInDetail { get; set; }
        public DbSet<TMUser> TMUser { get; set; }
        public DbSet<TMKeyInStatus> TMKeyInStatus { get; set; }
        public DbSet<TTApproveKeyIn> TTApproveKeyIn { get; set; }
        public DbSet<TMApproveStatus> TMApproveStatus { get; set; }
        public DbSet<TTApproveKeyInDetail> TTApproveKeyInDetail { get; set; }
        public DbSet<TTAdjustData> TTAdjustData { get; set; }
        public DbSet<TTAdjustDataDetail> TTAdjustDataDetail { get; set; }
        public DbSet<TTAdjustDataBrandDetail> TTAdjustDataBrandDetail { get; set; }
        public DbSet<TMAdjustStatus> TMAdjustStatus { get; set; }
        public DbSet<TMUrl> TMUrl { get; set; }
        public DbSet<TMUserToken> TMUserToken { get; set; }
        public DbSet<TMTopDepartmentStore> TMTopDepartmentStore { get; set; }
        public DbSet<Brand_Ranking> Brand_Ranking { get; set; }
        public DbSet<Loreal_Store> Loreal_Store { get; set; }
        public DbSet<Brand_Frangances> Brand_Frangances { get; set; }
        public DbSet<Data_Exporting> Data_Exporting { get; set; }
        public DbSet<TMBrandSummary> TMBrandSummary { get; set; }
    }
}
