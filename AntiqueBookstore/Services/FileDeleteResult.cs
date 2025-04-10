namespace AntiqueBookstore.Services
{
    public class FileDeleteResult
    {
        public bool Success { get; private set; }

        public string? ErrorMessage { get; private set; }

        private FileDeleteResult() { }

        public static FileDeleteResult Succeeded() =>
            new FileDeleteResult { Success = true };

        public static FileDeleteResult Failed(string errorMessage) =>
            new FileDeleteResult { Success = false, ErrorMessage = errorMessage };
    }
}
