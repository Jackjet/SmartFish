 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SmartFish.Dal;

namespace WLXK.SmartFish.IDal
{
   public partial interface IDbSession
    {
	
		IControlDataDal  ControlDataDal{get;}
	
		IReceiveDatasDal  ReceiveDatasDal{get;}
		int SaveChanges();
	}
	
}