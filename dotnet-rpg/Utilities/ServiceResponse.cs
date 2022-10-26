namespace dotnet_rpg.Utilities {
    public class ServiceResponse<T> {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        public void SetServiceResponse(bool success, string message) {
            this.Success = success;
            this.Message = message;
        }
    }
}
