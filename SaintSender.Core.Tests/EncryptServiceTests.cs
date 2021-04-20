using NUnit.Framework;
using SaintSender.Core.Services;

namespace SaintSender.Core.Tests
{
    [TestFixture]
    public class EncryptServiceTests
    {

        [Test]
        public void Encrypt_text_TextShouldBeTheSameAfterEncryptionAndDecryption()
        {
            //Arrange
            EncryptService encryptService = new EncryptService();
            string textToTest = "This is a test text what we should encrypt and decrypt";
            //Act
           string encryptedText = encryptService.Encrypt(textToTest);
           string decryptedText = encryptService.Decrypt(encryptedText);
            //Assert
            Assert.AreEqual(textToTest,decryptedText);
        }

    }
}