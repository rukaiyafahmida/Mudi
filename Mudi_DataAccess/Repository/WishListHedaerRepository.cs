using Mudi_DataAccess.Repository.IRepository;
using Mudi_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_DataAccess.Repository
{
    public class WishListHedaerRepository : Repository<WishListHeader>, IWishListHedaerRepository
    {
        private readonly ApplicationDbContext _db;
        public WishListHedaerRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WishListHeader obj)
        {
            throw new NotImplementedException();
        }
    }
}