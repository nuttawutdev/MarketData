﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MarketData.Model.Entiry
{
    public class TMUser
    {
        [Key]
        public Guid ID { get; set; }
        public string DisplayName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool ViewMasterPermission { get; set; }
        public bool EditMasterPermission { get; set; }
        public bool EditUserPermission { get; set; }
        public bool KeyInDataPermission { get; set; }
        public bool ApprovePermission { get; set; }
        public bool ViewDataPermission { get; set; }
        public bool ViewReportPermission { get; set; }
        public bool ActiveFlag { get; set; }
        public bool ValidateEmailFlag { get; set; }
        public bool OnlineFlag { get; set; }
        public int? WrongPasswordCount { get; set; }
        public DateTime? Last_Login { get; set; }
        public Guid Create_By { get; set; }
        public DateTime Create_Date { get; set; }
        public Guid? Update_By { get; set; }
        public DateTime? Update_Date { get; set; }
        public bool? Delete_Flag { get; set; }
    }
}
