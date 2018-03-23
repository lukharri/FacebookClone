using FacebookClone.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class OnlineUserViewModel
    {
        public OnlineUserViewModel()
        {

        }

        public OnlineUserViewModel(OnlineUserDto dto)
        {
            Id = dto.Id;
            ConnectionId = dto.ConnectionId;
        }

        public int Id { get; set; }
        public string ConnectionId { get; set; }

    }
}