namespace KillBug.Services
{
    public class PasswordResetTemplate : IMailTemplate
    {
        public string CodeUrl { get; set; }
        public PasswordResetTemplate(string codeUrl)
        {
            CodeUrl = codeUrl;
        }
    }
}