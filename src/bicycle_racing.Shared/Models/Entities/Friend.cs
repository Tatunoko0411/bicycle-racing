using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bicycle_racing.Shared.Models.Entities
{
    [MessagePackObject]
    public class Friend
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public int Player_id_01 { get; set; }
        [Key(2)]
        public int Player_id_02 { get; set; }
        [Key(4)]
        public DateTime Created_at { get; set; }
        [Key(5)]
        public DateTime Updated_at { get; set; }
    }
}
