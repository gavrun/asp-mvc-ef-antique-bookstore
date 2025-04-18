﻿namespace AntiqueBookstore.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        // Service responsible for managing file storage locally

        // inject dependencies
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger<LocalFileStorageService> _logger; // Logger

        // allowed extensions
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        // max file size 
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        // constructor

        //public LocalFileStorageService(IWebHostEnvironment webHostEnvironment, ILogger<LocalFileStorageService> logger)
        //{
        //    _webHostEnvironment = webHostEnvironment;
        //    _logger = logger;
        //}

        public LocalFileStorageService(IWebHostEnvironment webHostEnvironment, ILogger<LocalFileStorageService> logger)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // save file
        public async Task<FileUploadResult> SaveFileAsync(IFormFile file, string subfolder)
        {
            // validate file
            if (file == null || file.Length == 0)
            {
                _logger.LogInformation("[SaveFileAsync] No file or empty file.");

                return FileUploadResult.Failed("No file provided or file is empty.");
            }

            // validate extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(fileExtension))
            {
                _logger.LogInformation($"[SaveFileAsync] Invalid file extension: {fileExtension}");
                return FileUploadResult.Failed("Invalid file type. Allowed types are: " + string.Join(", ", _allowedExtensions));
            }

            // validate file size
            if (file.Length > _maxFileSize)
            {
                _logger.LogInformation($"[SaveFileAsync] File size exceeds the limit: {file.Length} bytes");
                return FileUploadResult.Failed("File size exceeds the limit of 5 MB.");
            }

            // save file
            try
            {
                string targetFolder = Path.Combine(_webHostEnvironment.WebRootPath, subfolder);

                if (!Directory.Exists(targetFolder))
                {
                    _logger.LogInformation("[SaveFileAsync] Creating directory: {DirectoryPath}", targetFolder);
                    Directory.CreateDirectory(targetFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                string absoluteFilePath = Path.Combine(targetFolder, uniqueFileName);

                _logger.LogInformation("[SaveFileAsync] Saving file to: {FilePath}", absoluteFilePath);

                using (var fileStream = new FileStream(absoluteFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                _logger.LogInformation("[SaveFileAsync] Saved file: {FilePath} successfully", absoluteFilePath);

                string relativePath = Path.Combine("/", subfolder, uniqueFileName).Replace("\\", "/");

                return FileUploadResult.Succeeded(relativePath);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SaveFileAsync] Error saving file {FileName} to subfolder {Subfolder}", file.FileName, subfolder);
                //throw;
                return FileUploadResult.Failed("An internal error occurred while saving the file.");
            }
        }

        // delete file
        public async Task<FileDeleteResult> DeleteFileAsync(string relativePath)
        {
            // validate path
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                _logger.LogWarning("[DeleteFileAsync] Attempted to delete with null or empty file path.");
                return FileDeleteResult.Failed("File path cannot be null or empty.");
            }

            // delete file
            try
            {
                string absolutePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/', '\\'));

                if (File.Exists(absolutePath))
                {
                    _logger.LogInformation("[DeleteFileAsync] Deleting file: {FilePath}", absolutePath);
                    await Task.Run(() => File.Delete(absolutePath));
                    _logger.LogInformation("[DeleteFileAsync] Successfully deleted file: {FilePath}", absolutePath);
                    return FileDeleteResult.Succeeded();
                }
                else
                {
                    _logger.LogWarning("[DeleteFileAsync] File not found: {FilePath}", absolutePath);
                    return FileDeleteResult.Succeeded();
                }
            }
            catch (IOException ioEx)
            {
                _logger.LogError(ioEx, "[DeleteFileAsync] Error deleting file {FileName}", relativePath);               
                return FileDeleteResult.Failed($"IO error deleting file: {ioEx.Message}");
            }
            catch (UnauthorizedAccessException authEx)
            {
                _logger.LogError(authEx, "[DeleteFileAsync] Access denied deleting file: {FilePath}", relativePath);
                return FileDeleteResult.Failed($"Access denied deleting file: {authEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[DeleteFileAsync] Unexpected error deleting file: {FilePath}", relativePath);
                return FileDeleteResult.Failed($"An unexpected error occurred: {ex.Message}");
            }

        }
    }
}
