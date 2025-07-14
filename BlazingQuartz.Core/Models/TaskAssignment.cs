
namespace BlazeQuartz.Core.Models
{
    public class TaskAssignment
    {
        public int ID { get; set; }
        public string JOB_NAME { get; set; }
        public string TRIGGER_NAME { get; set; }
        public int USER_ID { get; set; }
        public int GROUP_ID{ get; set; }
    }
}
