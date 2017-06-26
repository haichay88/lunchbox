using Bizkasa.Bizlunch.Business.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizkasa.Bizlunch.Business.Model
{
 
    public class AccountDTO:BaseRequest
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email not available")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }


        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        
        public string MiddleName { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
       
        public string SourceId { get; set; }
        public bool IsActived { get; set; }
        public bool IsSelected { get; set; }

    }
    public class SearchDTO:BaseRequest
    {
        public string Keyword { get; set; }
        public int Id { get; set; }
    }
    public class FriendDTO
    {
        public int Id { get; set; }
      
        public string Email { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [DisplayName("Company Name")]
        public string CompanyName { get; set; }
       
        public string SourceId { get; set; }
        public bool IsActived { get; set; }
        public bool IsSelected { get; set; }

    }

    public class LoginDTO:BaseRequest
    {
       
        public string Email { get; set; }
        
        public string Password { get; set; }
        public bool IsReember { get; set; }

        public int AppCode { get; set; }
    }
    public class SignUpDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string DeviceKey { get; set; }
    }
    public class LoginResultDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }

    }
    public class ContextDTO
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        

    }

    public class BaseRequest
    {
        public string Token { get; set; }
        public string DeviceKey { get; set; }

        public ContextDTO Context
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(Token)) return null;
                    // token= EncryptDecryptUtility.Decrypt(token, true);
                    string encode = EncryptDecryptUtility.Decrypt(Token, true);
                    if (string.IsNullOrEmpty(encode)) return null;
                    return XmlUtility.DeSerialize<ContextDTO>(encode);
                }
                catch (Exception ex)
                {
                    return null;

                }
            }
        }
    }

    public class NotificationDTO
    {
        public string To { get; set; }
        public NotificationItem data { get; set; }
    }
    public class NotificationItem
    {
        public string Title { get; set; }
        public string Message{ get; set; }
        public int Id{ get; set; }
    }
}
