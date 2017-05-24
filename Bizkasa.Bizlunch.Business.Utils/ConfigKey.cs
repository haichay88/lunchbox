﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Utils
{
   public class ConfigKey
    {
       public static readonly string APIUploadImage = ConfigurationManager.AppSettings["APIUploadImage"];
       public static readonly string PasswordTech = ConfigurationManager.AppSettings["PasswordTech"];
       public static readonly string FCM_KEY = ConfigurationManager.AppSettings["FCM_KEY"];
       public static readonly string FCM_KEYTEST = ConfigurationManager.AppSettings["FCM_KEYTEST"];
    }
}