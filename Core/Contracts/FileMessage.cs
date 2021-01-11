using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Contracts
{
    public class FileMessage
    {
        public Guid FileMessageId   { get; set; }
        public string? Filename     { get; set; }
        public string? Data         { get; set;  }
    }
}
