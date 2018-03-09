using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.Data
{
    [Table("Wall")]
    public class WallDto
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateEdited { get; set; }

        [ForeignKey("Id")]
        public virtual UserDTO Users { get; set; }
    }
}