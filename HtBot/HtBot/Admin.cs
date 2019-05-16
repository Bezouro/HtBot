using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtBot.HtBot
{
    class Admin
    {

        int userId;
        bool userIsBot;
        string userName;
        string userLanguage;

        string userStatus;
        bool userCanBeEdited;
        bool userCanChangeInfo;
        bool userCanDeleteMessages;
        bool userCanInviteUsers;
        bool userCanRestrictMembers;
        bool userCanPinMessages;
        bool userCanPromoteMembers;

        public Admin(JObject admin)
        {
            JObject user = JObject.Parse(admin.GetValue("user").ToString());

            userId = (int)user.GetValue("id");
            userIsBot = Convert.ToBoolean((string)user.GetValue("is_bot"));
            userName = (string)user.GetValue("first_name");
            userLanguage = (string)user.GetValue("language_code");

            userStatus = (string)admin.GetValue("status");
            if (!userStatus.Equals("creator"))
            {
                userCanBeEdited = Convert.ToBoolean((string)admin.GetValue("can_be_edited"));
                userCanChangeInfo = Convert.ToBoolean((string)admin.GetValue("can_change_info"));
                userCanDeleteMessages = Convert.ToBoolean((string)admin.GetValue("can_delete_messages"));
                userCanInviteUsers = Convert.ToBoolean((string)admin.GetValue("can_invite_users"));
                userCanRestrictMembers = Convert.ToBoolean((string)admin.GetValue("can_restrict_members"));
                userCanPinMessages = Convert.ToBoolean((string)admin.GetValue("can_pin_messages"));
                userCanPromoteMembers = Convert.ToBoolean((string)admin.GetValue("can_promote_members"));
            }
            else
            {
                userCanBeEdited = false;
                userCanChangeInfo = true;
                userCanDeleteMessages = true;
                userCanInviteUsers = true;
                userCanRestrictMembers = true;
                userCanPinMessages = true;
                userCanPromoteMembers = true;
            }
        }

        public int id() { return userId; }
        public bool isBot() { return userIsBot; }
        public string name() { return userName; }
        public string language() { return userLanguage; }

        public string status() { return userStatus; }
        public bool can_be_edited() { return userCanBeEdited; }
        public bool canChangeInfo() { return userCanChangeInfo; }
        public bool canDeleteMessages() { return userCanDeleteMessages; }
        public bool canInviteUsers() { return userCanInviteUsers; }
        public bool canRestrictMembers() { return userCanRestrictMembers; }
        public bool canPinMessages() { return userCanPinMessages; }
        public bool canPromoteMembers() { return userCanPromoteMembers; }

    }
}
