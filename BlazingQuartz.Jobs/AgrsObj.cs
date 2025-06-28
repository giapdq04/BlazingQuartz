using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeQuartz.Jobs
{
    public class AgrsObj
    {
        public AgrsObj() { }
        public string ConnType { get; set; }
        public string ExePath { get; set; }
        public int Cusid { get; set; }
        public string EdiType { get; set; }
        public string CusName { get; set; }
        public string JobType { get; set; }
        public string Agrs { get; set; }
    }
}
