using System;
using System.Collections.Generic;
using System.Text;

namespace TcpP43.Common.Models
{
    public class CommandType
    {
        //public const string Auth = "Auth";
        //public const string GetFilesList = "GetFilesList";
        //public const string DownloadFile = "DownloadFile";
        //public const string UploadFile = "UploadFile";
        public const string Register = "Register";
        public const string Login = "Login";
        public const string ListUsers = "ListUsers";
        public const string SendMessage = "SendMessage";
        public const string GetMessages = "GetMessages";
    }
}
