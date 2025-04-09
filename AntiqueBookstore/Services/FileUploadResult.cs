namespace AntiqueBookstore.Services
{
    public class FileUploadResult
    {
        // Represents the result of a file upload operation.

        // Indicates whether the file upload was successful
        public bool Success { get; init; }

        // The path to the uploaded file
        public string? RelativePath { get; init; }


        // The error message if the upload failed
        public string? ErrorMessage { get; init; }


        // successful upload
        public static FileUploadResult Succeeded(string relativePath) => new() 
        { 
            Success = true, 
            RelativePath = relativePath 
        };

        // failed upload
        public static FileUploadResult Failed(string errorMessage) =>new() 
        { 
            Success = false, 
            ErrorMessage = errorMessage 
        };
    }
}
