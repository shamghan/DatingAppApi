using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Helpers
{
    public class MessageParam : PagingParams
    {
        public string? MemberId {get;set;}="";
        public string Container {get;set;}="Inbox";
    }
}