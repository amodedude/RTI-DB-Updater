using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTI.DataBase.Objects;

namespace RTI.DataBase.Util
{
    public class EmailAlertList : IEnumerable<Alert>
    {
        public List<Alert> AlertList { get; set; }

        public EmailAlertList()
        {
            AlertList = new List<Alert>();
        }

        public void Add(Alert alert)
        {
            AlertList.Add(alert);
        }

        public int Count()
        {
            return AlertList.Count;
        }

        public IEnumerator<Alert> GetEnumerator()
        {
            return AlertList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AlertList.GetEnumerator();
        }
    }
}
