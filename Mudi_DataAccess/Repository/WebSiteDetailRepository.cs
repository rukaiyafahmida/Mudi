using Mudi_DataAccess.Repository.IRepository;
using Mudi_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_DataAccess.Repository
{
    public class WebSiteDetailRepository : Repository<WebSiteDetail>, IWebSiteDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public WebSiteDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(WebSiteDetail obj)
        {
            _db.WebSiteDetail.Update(obj);
        }
    }
}