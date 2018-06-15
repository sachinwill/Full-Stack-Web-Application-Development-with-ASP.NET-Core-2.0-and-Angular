﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Macaria.Core.Entities
{
    public class User: BaseEntity
    {
        public User()
        {
            Salt = new byte[128 / 8];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(Salt);
            }
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }
    }
}
