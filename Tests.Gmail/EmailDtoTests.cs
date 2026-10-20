using Apps.Gmail.Dtos;
using Google.Apis.Gmail.v1.Data;
using System.Text;

namespace Tests.Gmail;

[TestClass]
public class EmailDtoTests
{
    [TestMethod]
    public void Constructor_MultipartRelatedWithNestedAlternative_ReturnsPlainTextBody()
    {
        var expected = "Open project: https://cloud.memsource.com/web/project/show/123";
        var message = CreateMessage(new MessagePart
        {
            MimeType = "multipart/related",
            Body = new MessagePartBody(),
            Parts = new List<MessagePart>
            {
                new()
                {
                    MimeType = "multipart/alternative",
                    Body = new MessagePartBody(),
                    Parts = new List<MessagePart>
                    {
                        CreateBodyPart("text/plain", expected),
                        CreateBodyPart("text/html", $"<p>{expected}</p>")
                    }
                },
                new()
                {
                    MimeType = "image/png",
                    Filename = "signature.png",
                    Body = new MessagePartBody { Data = EncodeBase64Url("image") }
                }
            }
        });

        var result = new EmailDto(message);

        Assert.AreEqual(expected, result.Message);
    }

    [TestMethod]
    public void Constructor_NoPlainTextBody_ReturnsHtmlBody()
    {
        const string expected = "<p>HTML-only message</p>";
        var message = CreateMessage(new MessagePart
        {
            MimeType = "multipart/alternative",
            Body = new MessagePartBody(),
            Parts = new List<MessagePart>
            {
                CreateBodyPart("text/html", expected)
            }
        });

        var result = new EmailDto(message);

        Assert.AreEqual(expected, result.Message);
    }

    [TestMethod]
    public void Constructor_PlainTextAttachmentBeforeBody_IgnoresAttachment()
    {
        const string expected = "Actual email body";
        var attachment = CreateBodyPart("text/plain", "Attachment content");
        attachment.Filename = "notes.txt";

        var message = CreateMessage(new MessagePart
        {
            MimeType = "multipart/mixed",
            Body = new MessagePartBody(),
            Parts = new List<MessagePart>
            {
                attachment,
                CreateBodyPart("text/plain", expected)
            }
        });

        var result = new EmailDto(message);

        Assert.AreEqual(expected, result.Message);
    }

    private static Message CreateMessage(MessagePart payload)
    {
        payload.Headers ??= new List<MessagePartHeader>();

        return new Message
        {
            Id = "email-id",
            Snippet = "snippet",
            Payload = payload
        };
    }

    private static MessagePart CreateBodyPart(string mimeType, string body) => new()
    {
        MimeType = mimeType,
        Body = new MessagePartBody { Data = EncodeBase64Url(body) }
    };

    private static string EncodeBase64Url(string value) => Convert
        .ToBase64String(Encoding.UTF8.GetBytes(value))
        .TrimEnd('=')
        .Replace('+', '-')
        .Replace('/', '_');
}
