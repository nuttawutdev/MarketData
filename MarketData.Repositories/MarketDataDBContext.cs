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
    }
}
