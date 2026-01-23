using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bicycle_racing.Shared.Models.Entities
{
 

    [MessagePackObject]
    public class Mail
    {


        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public int send_player_id { get; set; }
        [Key(2)]
        public int Receive_player_id { get; set; }
        [Key(3)]
        public int Type { get; set; }
        [Key(4)]
        public string Content { get; set; }
        [Key(5)]
        public int state { get; set; }
        [Key(6)]
        public DateTime Created_at { get; set; }
        [Key(7)]
        public DateTime Updated_at { get; set; }
    }
}
