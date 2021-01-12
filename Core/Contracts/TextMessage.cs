using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public class TextMessage
    {
        public Guid     TextMessageId { get; set; }
        public string?  Username      { get; set; }
        public string?  Text          { get; set; }
        public DateTime Timestamp     { get; set; }
    }
}
