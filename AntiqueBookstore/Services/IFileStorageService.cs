namespace AntiqueBookstore.Services
{
    public interface IFileStorageService
    {
        // Service for files storage and managing files locally

        // Saves the uploaded file via the HTTP request to the configured storage
        Task<FileUploadResult> SaveFileAsync(IFormFile file, string subfolder);

        // Retrieves the file path for the specified relative path >> Test Class, Result Class
        //bool FileExists(string relativePath);

        // Deletes the file from the configured storage
        Task<FileDeleteResult> DeleteFileAsync(string relativePath);
    }
}
