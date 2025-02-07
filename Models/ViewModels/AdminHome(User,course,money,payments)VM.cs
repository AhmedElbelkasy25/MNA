using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class AdminHome_User_course_money_payments_VM
    {
        public int courses {  get; set; }
        public int users {  get; set; }
        public double revenue {  get; set; }
        public List<Payment> trans {  get; set; }
    }
}
