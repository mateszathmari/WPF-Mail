using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using SaintSender.Core.Services;

namespace SaintSender.Core.Models
{
    public class Account : ISerializable
    {
        private string _username;
        private string _password;
        private bool _rememberUserCredentials;
        public EncryptService _encryptService = new EncryptService();
        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Remail";

        public Account()
        {
        }

        public void Setup(string username, string password, bool rememberUserCredentials)
        {
            _username = username;
            _password = password;
            _rememberUserCredentials = rememberUserCredentials;
        }


        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public bool RememberUserCredentials
        {
            get => _rememberUserCredentials;
            set => _rememberUserCredentials = value;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Username", Username);
            info.AddValue("Password", Password);
            info.AddValue("RememberUserCredentials", RememberUserCredentials);
        }

        public Account(SerializationInfo info, StreamingContext context)
        {
            Username = (string) info.GetValue("Name", typeof(string));
            Password = (string) info.GetValue("Password", typeof(string));
            RememberUserCredentials = (bool) info.GetValue("RememberUserCredentials", typeof(bool));
        }
    }
}