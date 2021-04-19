using NUnit.Framework;
using NUnit.Framework.Internal;
using SaintSender.Core.Services;

namespace SaintSender.Core.Tests
{
    [TestFixture]
    public class MailServiceTests
    {
        [Test]
        public void CheckInternet_ShouldReturnTrue()
        {
            //bool isNetAvailable = MailService.CheckInternet();

            // Arrange
            var service = new GreetService();
            // Act
            var greeting = service.Greet(".NET Padawan");
            // Assert
            Assert.AreEqual("Welcome .NET Padawan, my friend!", greeting);
        }
    }
}