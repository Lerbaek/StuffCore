using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core.Models;
using FluentEmail.Mailgun;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Options = Mailgun.Services.AuthMessageSenderOptions;

namespace Mailgun.Test
{
  public class MailgunEmailSenderTests
  {
    private MailgunEmailSender uut;
    private Mock<IOptions<Options>> options;
    private Options authMessageSenderOptions;
    private IConfigurationRoot configuration;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      configuration = new ConfigurationBuilder()
        .AddUserSecrets<Options>()
        .Build();
    }

    [SetUp]
    public void SetUp()
    {
      string GetConfiguration(string key) => configuration.GetValue<string>(key);

      authMessageSenderOptions = new Options()
      {
        MailgunDomain = GetConfiguration(nameof(Options.MailgunDomain)),
        MailgunKey    = GetConfiguration(nameof(Options.MailgunKey   )),
        MailgunRegion = GetConfiguration(nameof(Options.MailgunRegion))
      };

      options = new Mock<IOptions<Options>>();
      options.SetupGet(o => o.Value).Returns(authMessageSenderOptions);
      uut = new MailgunEmailSender(options.Object);
    }

    private async Task<SendResponse> SendTestMail()
    {
      var response = await (Task<SendResponse>)uut.SendEmailAsync(
        $"{nameof(SendEmailAsync_MailIsSent)}@lortemail.dk",
        "Test mail",
        "If you can read this, the test has passed");
      return response;
    }

    [Test]
    public async Task SendEmailAsync_MailIsSent()
    {
      var response = await SendTestMail();
      Assert.IsTrue(response.Successful, string.Join(Environment.NewLine, response.ErrorMessages));
    }

    private static readonly IEnumerable<string> InvalidConstructorArgs = new[]
    {
      null,
      string.Empty,
      " ",
      string.Concat(Enumerable.Range(0, 100).Select(_ => " "))
    };

    private static IEnumerable<TestCaseData> InvalidConstructorArgsTestCases =>
      InvalidConstructorArgs.SelectMany(arg => new[]
      {
        CreateTestCaseAction(o => o.MailgunDomain = arg, nameof(Options.MailgunDomain), arg),
        CreateTestCaseAction(o => o.MailgunKey    = arg, nameof(Options.MailgunKey),    arg),
        CreateTestCaseAction(o => o.MailgunRegion = arg, nameof(Options.MailgunRegion), arg)
      }).OrderBy(@case => @case.TestName);

    private static TestCaseData CreateTestCaseAction(Action<Options> action, string option, string arg) =>
      new TestCaseData(action).SetName($"{option} = \"{arg ?? "null"}\"");

    [TestCaseSource(nameof(InvalidConstructorArgsTestCases))]
    public void Constructor_MailgunPropertyIsInvalid_Throws(Action<Options> modifier)
    { 
      modifier(authMessageSenderOptions);
      Assert.That(() => new MailgunEmailSender(options.Object), Throws.ArgumentException,
        $"An {nameof(ArgumentException)} was supposed to be thrown.");
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]

    [TestCase("mg.testdomain.com", "TestKey"    , MailGunRegion.EU )]
    [TestCase("anotherdomain.com", "4nother$Key", MailGunRegion.USA)]
    public void Constructor_MailgunPropertiesAreValid_DoesNotThrow(string domain, string key, MailGunRegion region)
    {
      authMessageSenderOptions.MailgunDomain =    domain;
      authMessageSenderOptions.MailgunKey    =    key;
      authMessageSenderOptions.MailgunRegion = $"{region}";
      Assert.DoesNotThrow(() => new MailgunEmailSender(options.Object), $"An exception was not supposed to be thrown.");
    }
  }
}
