using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bicycle_racing.Shared.Models.Entities
{
    [MessagePackObject]
    public class User
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public int State { get; set; }
        [Key(3)]
        public string Room_name { get; set; }
        [Key(4)]
        public int Stage_id { get; set; }
        [Key(5)]
        public int Play_count { get; set; }
        [Key(6)]
        public int Win_count { get; set; }
        [Key(7)]
        public DateTime Created_at { get; set; }
        [Key(8)]
        public DateTime Updated_at { get; set; }

    }
}
