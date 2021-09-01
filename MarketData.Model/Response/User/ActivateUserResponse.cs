﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MarketData.Model.Response.User
{
    public class ActivateUserResponse : BaseResponse
    {
        public bool isSuccess { get; set; }
        public bool unActive { get; set; }
        public bool urlExpire { get; set; }
        public bool urlNotFound { get; set; }
    }
}
