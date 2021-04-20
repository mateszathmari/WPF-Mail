using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MimeKit;
using SaintSender.Core.Interfaces;
using SaintSender.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;

namespace SaintSender.Core.Services
{
    public class MailService
    {
        private List<Email> _emails = new List<Email>();
        private EncryptService _encryptService = new EncryptService();

        private string _path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Remail";

        public List<Email> GetMails(string username, string password)
        {
            _emails = new List<Email>();
            if (CheckInternet())
            {
                using (var client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(username, password);
                    //The Inbox folder is always available on all IMAP servers...
                    IMailFolder inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    AddEmailsToList(client);

                    client.Disconnect(true);
                    NewBackup(_emails);
                }
            }
            else
            {
                _emails = LoadBackup();
            }

            _emails.Reverse();
            return _emails;
        }

        private bool CheckInternet()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        private Email getEmailById(ImapClient client, UniqueId id)
        {
            var info = client.Inbox.Fetch(new[] {id}, MessageSummaryItems.Flags);
            var seen = info[0].Flags.Value.HasFlag(MessageFlags.Seen);
            var mail = client.Inbox.GetMessage(id);

            return new Email(seen, mail.From.ToString(), mail.Subject, mail.Date.DateTime, mail.TextBody,
                id);
        }

        private void AddEmailsToList(ImapClient client)
        {
            var uniqueIdList = client.Inbox.Search(SearchQuery.All);
            foreach (UniqueId id in uniqueIdList)
            {
                _emails.Add(getEmailById(client, id));
            }
        }

        public void SetEmailSeen(UniqueId uId, string username, string password)
        {
            if (CheckInternet())
            {
                using (var client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(username, password);
                    //The Inbox folder is always available on all IMAP servers...
                    IMailFolder inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadWrite);
                    inbox.AddFlags(uId, MessageFlags.Seen, true);
                    client.Disconnect(true);
                }
            }
        }


        public void SendNewEmail(string username, string password, string text, string subject, string toMail)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(username));
            message.To.Add(new MailboxAddress(toMail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = text
            };

            using (var client = new SmtpClient())
            {
                client.Connect("imap.gmail.com", 465, true);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(username, password);

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public bool IsCorrectLoginCredentials(string username, string password)
        {
            try
            {
                using (var client = new ImapClient())
                {
                    client.Connect("imap.gmail.com", 993, true);
                    client.Authenticate(username, password);
                    client.Disconnect(true);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void NewBackup(List<Email> emails, string path = "EmailBackup.xml")
        {
            string filePath = Path.Combine(_path, path);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            List<Email> encremails = EncryptEmails(emails);
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Email>));
                xs.Serialize(sw, encremails);
            }
        }

        private List<Email> EncryptEmails(List<Email> unencryptedEmails)
        {
            List<Email> encryptedEmails = new List<Email>();
            foreach (Email unencryptedEmail in unencryptedEmails)
            {
                Email encryptedEmail = new Email(unencryptedEmail.Seen, unencryptedEmail.Sender,
                    unencryptedEmail.Subject, unencryptedEmail.Date, unencryptedEmail.Body, unencryptedEmail.UId);
                encryptedEmail.Sender = _encryptService.Encrypt(unencryptedEmail.Sender);
                encryptedEmail.Subject = _encryptService.Encrypt(unencryptedEmail.Subject);
                encryptedEmail.Body = _encryptService.Encrypt(unencryptedEmail.Sender);
                encryptedEmails.Add(encryptedEmail);
            }

            return encryptedEmails;
        }

        public List<Email> LoadBackup(string path = "EmailBackup.xml")
        {
            string filePath = Path.Combine(_path, path);

            if (File.Exists(filePath))
            {
                using (StreamReader sw = new StreamReader(filePath))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(List<Email>));
                    List<Email> encryptedEmails = (List<Email>) xs.Deserialize(sw);
                    List<Email> decryptedEmails = DecryptEmails(encryptedEmails);
                    return decryptedEmails;
                }
            }

            return null;
        }

        private List<Email> DecryptEmails(List<Email> encryptedEmails)
        {
            List<Email> decryptedEmails = new List<Email>();
            foreach (Email encryptedEmail in encryptedEmails)
            {
                Email decryptedEmail = new Email(encryptedEmail.Seen, encryptedEmail.Sender, encryptedEmail.Subject,
                    encryptedEmail.Date, encryptedEmail.Body, encryptedEmail.UId);
                decryptedEmail.Sender = _encryptService.Decrypt(encryptedEmail.Sender);
                decryptedEmail.Subject = _encryptService.Decrypt(encryptedEmail.Subject);
                decryptedEmail.Body = _encryptService.Decrypt(encryptedEmail.Body);
                decryptedEmails.Add(decryptedEmail);
            }

            return decryptedEmails;
        }
    }
}