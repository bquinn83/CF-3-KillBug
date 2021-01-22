namespace KillBug.Services
{
    public class ConfirmEmailTemplate : IMailTemplate
    {
        public string CodeUrl { get; set; }
        public ConfirmEmailTemplate(string codeUrl)
        {
            CodeUrl = codeUrl;
        }
    }
}