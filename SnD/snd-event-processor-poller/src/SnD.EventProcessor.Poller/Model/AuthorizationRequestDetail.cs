namespace SnD.EventProcessor.Poller.Model
{
    public class AuthorizationRequestDetail
    {
        public string DomainUrl { get; set; }
        public string GrantType { get; set; }
        public string AuthClientId { get; set; }
        public string AuthClientSecret { get; set; }
    }
}
