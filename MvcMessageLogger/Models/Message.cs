using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MvcMessageLogger.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get;  set; }
        public DateTime CreatedAt { get;  set; }

        public Message()
        {

            Content = string.Empty;
            CreatedAt = DateTime.UtcNow;
        }

        public Message(string content)
        {
            Content = content;
            CreatedAt = DateTime.Now.ToUniversalTime();
        }
    }
}
