using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.Data
{
    [Table("OnlineUser")]
    public class OnlineUserDto
    {
        [Key]
        public int Id { get; set; }
        public string ConnectionId { get; set; }

        [ForeignKey("Id")]
        public virtual UserDTO Users { get; set; }
    }
}