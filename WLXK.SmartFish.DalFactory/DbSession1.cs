 

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using WLXK.SmartFish.Dal;
using WLXK.SmartFish.IDal;

namespace WLXK.SmartFish.IDal
{
    public partial class DbSession:IDbSession
    {
	
		public IControlDataDal ControlDataDal
        {
            get
            {
                return new ControlDataDal();
            }
        }
	
		public IReceiveDatasDal ReceiveDatasDal
        {
            get
            {
                return new ReceiveDatasDal();
            }
        }
		public int SaveChanges()
        {
            DbContext db = EFContextFactory.GetCurrentEFContext();
            return db.SaveChanges();
        }
	}
	
}