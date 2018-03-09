using FacebookClone.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class WallViewModel
    {
        public WallViewModel()
        {

        }

        public WallViewModel(WallDto dto)
        {
            Id = dto.Id;
            Message = dto.Message;
            DateEdited = dto.DateEdited;
        }

        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime DateEdited { get; set; }

    }
}