using FacebookClone.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class FriendRequestViewModel
    {
        public FriendRequestViewModel()
        {

        }

        public FriendRequestViewModel(FriendDto dto)
        {
            User1 = dto.User1;
            User2 = dto.User2;
            Active = dto.Active;
        }

        public int User1 { get; set; }
        public int User2 { get; set; }
        public bool Active { get; set; }
    }
}