using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<FriendDto> Friends { get; set; }
        public DbSet<MessageDto> Messages { get; set; }
        public DbSet<WallDto> Wall { get; set; }
    }
}