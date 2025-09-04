using Tests.Gmail.Base;
using Apps.Gmail.Actions;
using Apps.Gmail.Models.Requests;

namespace Tests.Gmail;

[TestClass]
public class EmailTests : TestBase 
{
    [TestMethod]
    public async Task SendEmail_SubjectHasMoreThanOneLine_IsSuccess() 
    {
        // Arrange
        var emailActions = new EmailActions(InvocationContext, FileManager);
        var request = new SendEmailRequest 
        {
            To = "tohafesenkolovep@gmail.com",
            Subject = "123456\n7890",
            Message = "test\ntest2"
        };

        // Act
        var result = await emailActions.SendEmail(request);

        // Assert
        Console.WriteLine(result);
        Assert.IsNotNull(result);
        Assert.AreEqual("123456 7890", result.Subject);
    }

    [TestMethod]
    public async Task SendEmail_SubjectIsEmpty_IsSuccess() 
    {
        // Arrange
        var emailActions = new EmailActions(InvocationContext, FileManager);
        var request = new SendEmailRequest 
        {
            To = "tohafesenkolovep@gmail.com",
            Subject = "",
            Message = "test\ntest2"
        };

        // Act
        var result = await emailActions.SendEmail(request);

        // Assert
        Console.WriteLine(result);
        Assert.IsNotNull(result);
        Assert.AreEqual(string.Empty, result.Subject);
    }
}
