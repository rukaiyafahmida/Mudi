using Microsoft.AspNetCore.Mvc.Rendering;
using Mudi_DataAccess.Repository.IRepository;
using Mudi_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mudi_DataAccess.Repository.IRepository
{
    public interface IWebSiteDetailRepository : IRepository<WebSiteDetail>
    {
        void Update(WebSiteDetail obj);

    }
}