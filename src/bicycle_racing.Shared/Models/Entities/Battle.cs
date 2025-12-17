using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bicycle_racing.Shared.Models.Entities
{
    [MessagePackObject]

    public class Battle
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public int stage_id { get; set; }
        [Key(3)]
        public DateTime Created_at { get; set; }
        [Key(4)]
        public DateTime Updated_at { get; set; }

    }
}
