using Mudi_DataAccess.Repository.IRepository;
using Mudi_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_DataAccess.Repository
{
    public class WishListDetailRepository : Repository<WishListDetail>, IWishListDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public WishListDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WishListDetail obj)
        {
            throw new NotImplementedException();
        }
    }
}