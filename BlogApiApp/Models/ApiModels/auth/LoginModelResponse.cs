namespace Models.ApiModels
{
    public class LoginModelResponse
    {
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

}
