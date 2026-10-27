using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppApi.Entities
{
    public class Group(string name)
    {
        [Key]
        public  string Name {get; set;} = name;
        //nav properties
        public ICollection<Connection> Connections {get;set;} =[];
    }
}