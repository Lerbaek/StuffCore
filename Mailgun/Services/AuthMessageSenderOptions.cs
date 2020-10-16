namespace Mailgun.Services
{
  public class AuthMessageSenderOptions
  {
    /// <example>
    /// To set: dotnet user-secrets set MailgunDomain &lt;user&gt;
    /// </example>
    public string MailgunDomain { get; set; }

    /// <example>
    /// To set: dotnet user-secrets set MailgunKey &lt;key&gt;
    /// </example>
    public string MailgunKey { get; set; }

    /// <example>
    /// To set: dotnet user-secrets set MailgunRegion &lt;region&gt;
    /// </example>
    public string MailgunRegion { get; set; }
  }
}
