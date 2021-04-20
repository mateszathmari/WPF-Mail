using System.Collections.Generic;
using MailKit;
using NSubstitute;
using NUnit.Framework;
using SaintSender.Core.Models;
using MailKit.Net.Imap;
using MailKit.Search;
using MailService = SaintSender.Core.Services.MailService;

namespace SaintSender.Core.Tests
{
    [TestFixture]
    public class MailServiceTests
    {
        MailService service = new MailService();
        private ImapClient _client;

        [Test]
        public void GetMails_CorrectCredentialsAdded_ReturnEmails()
        {
            // Arrange
            
            // Act
            List<Email> emails = service.GetMails("tom1.wales2@gmail.com", "Almafa1234");
            // Assert
            Assert.True(emails.Count != 0);
        }

        [Test]
        public void AddEmailsToList_AddNewMail_ReturnEmailsWithNewMail()
        {
            // Arrange
            _client = Substitute.For<ImapClient>();
            List<UniqueId> list = new List<UniqueId>();
            UniqueId ui = new UniqueId(5);
            list.Add(ui);
            _client.Inbox.Search(SearchQuery.All).Returns(list);
            // Act
            List<Email> emails = service.GetMails("tom1.wales2@gmail.com", "Almafa1234");
            // Assert
            Assert.True(emails.Count != 0);
        }
    }
}