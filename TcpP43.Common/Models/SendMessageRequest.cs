using System;
using System.Collections.Generic;
using System.Text;

namespace TcpP43.Common.Models
{
    public class SendMessageRequest
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
    }
}
