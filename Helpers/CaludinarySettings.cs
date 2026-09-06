using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Helpers
{
    public class CaludinarySettings
    {
        public required string CloudName {get;set;}
        public required string ApiKey {get;set;}
        public required string ApiSecret {get;set;}
    }
}