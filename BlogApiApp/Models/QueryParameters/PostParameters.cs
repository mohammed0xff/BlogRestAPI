using Models.Entities;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Query
{
    public class PostParameters : QueryStringParameters
    {
        public string? UsreId { get; set; } 
        public string? Tag { get; set; }
        public bool MostLiked { get; set; } = false;
    }
}
