using System;
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

        [Test]
        public void SendNewMail_SendNewMailTo_ShouldNotCrash()
        {
            service.SendNewEmail("tom1.wales2@gmail.com", "Almafa1234", "This is a test", "This is a test",
                "mateszathmari@gmail.com");
        }

        [Test]
        public void IsCorrectLoginCredentials_CorrectCredentials_shouldReturnTrue()
        {
            //Act
            bool isCorrectCredentials = service.IsCorrectLoginCredentials("tom1.wales2@gmail.com", "Almafa1234");
            //Assert
            Assert.True(isCorrectCredentials);
        }

        [Test]
        public void IsCorrectLoginCredentials_WrongCredentials_shouldReturnFalse()
        {
            //Act
            bool isCorrectCredentials = service.IsCorrectLoginCredentials("tom1.wales2@gmail.com", "password");
            //Assert
            Assert.False(isCorrectCredentials);
        }

        [Test]
        public void NewBackup_Emails_ShouldReturnEmails()
        {
            //Arrange
            Email email1 = new Email(false, "thisIs@est.mail", "This is a test", new DateTime(1968, 11, 12),
                "this is a test", new UniqueId(5));
            List<Email> emails = new List<Email>();
            emails.Add(email1);
            //Act
            service.NewBackup(emails);
            List<Email> backupEmails = service.LoadBackup();
            //Assert
            Assert.AreEqual(emails, backupEmails);
        }

    }
}