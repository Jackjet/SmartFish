 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WLXK.SmartFish.IBll;
using WLXK.SmartFish.Model;

namespace WLXK.SmartFish.Bll
{
   
	
    public partial class ControlDataServices:BaseServices<ControlData>,IControlDataServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.ControlDataDal;
        }
    }
 
	
    public partial class ReceiveDatasServices:BaseServices<ReceiveDatas>,IReceiveDatasServices
    {
        public override void SetCurrentDal()
        {
            CurrentDal = dbSession.ReceiveDatasDal;
        }
    }
 
	
}