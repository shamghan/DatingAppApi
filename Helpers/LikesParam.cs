using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Helpers
{
    public class LikesParam :PagingParams
    {
        public required string MemberId {get; set;}="";
        public required string Predicate {get; set;}="liked";
        
    }
}