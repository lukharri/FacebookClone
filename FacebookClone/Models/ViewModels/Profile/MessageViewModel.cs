using FacebookClone.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FacebookClone.Models.ViewModels.Profile
{
    public class MessageViewModel
    {
        public MessageViewModel()
        {

        }

        public MessageViewModel(MessageDto message)
        {
            Id = message.Id;
            From = message.From;
            To = message.To;
            Message = message.Message;
            DateSent = message.DateSent;
            Read = message.Read;
            FromId = message.FromUsers.Id;
            FromUsername = message.FromUsers.Username;
            FromFirstName = message.FromUsers.FirstName;
            FromLastName = message.FromUsers.LastName;
        }

        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Message { get; set; }
        public DateTime DateSent { get; set; }
        public bool Read { get; set; }

        public int FromId { get; set; }
        public string FromUsername { get; set; }
        public string FromFirstName { get; set; }
        public string FromLastName { get; set; }
    }
}