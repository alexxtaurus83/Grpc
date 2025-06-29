using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using Google.Protobuf;

namespace VerbaGrpc
{
    public class FileServiceImpl : FileService.FileServiceBase // Implements the GRPC FileServiceBase
    {
        private readonly string _storagePath; // Path where files will be stored
        private readonly ILogger _logger; // Logger instance for logging messages

        public FileServiceImpl(string storagePath, ILogger logger)
        {
            _storagePath = storagePath; // Initializes the storage path
            _logger = logger; // Initializes the logger
            Directory.CreateDirectory(_storagePath); // Ensures the storage directory exists
        }

        // Handles file upload requests
        public override Task<UploadFileResponse> UploadFile(UploadFileRequest request, ServerCallContext context)
        {
            try
            {
                var fileId = Guid.NewGuid().ToString(); // Generates a unique ID for the file
                var filePath = Path.Combine(_storagePath, fileId); // Constructs the full file path

                // Save the file content
                File.WriteAllBytes(filePath, request.Content.ToByteArray()); // Writes the file content to disk

                // Save metadata
                var metadataPath = filePath + ".metadata"; // Constructs the metadata file path
                File.WriteAllText(metadataPath, request.Metadata); // Writes the metadata to disk

                _logger.Information($"File uploaded: {request.Filename} -> {fileId}"); // Logs successful upload

                return Task.FromResult(new UploadFileResponse
                {
                    Status = "success", // Sets success status
                    Message = "File uploaded successfully", // Sets success message
                    FileId = fileId // Returns the generated file ID
                });
            }
            catch (Exception ex) // Catches any exceptions during upload
            {
                _logger.Error($"Error uploading file: {ex.Message}"); // Logs the error
                throw new RpcException(new Status(StatusCode.Internal, ex.Message)); // Throws an RpcException
            }
        }

        // Handles file download requests
        public override Task<DownloadFileResponse> DownloadFile(DownloadFileRequest request, ServerCallContext context)
        {
            try
            {
                var filePath = Path.Combine(_storagePath, request.FileId); // Constructs the file path
                if (!File.Exists(filePath)) // Checks if the file exists
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "File not found")); // Throws NotFound if file doesn't exist
                }

                var metadataPath = filePath + ".metadata"; // Constructs the metadata path
                var metadata = File.Exists(metadataPath) ? File.ReadAllText(metadataPath) : ""; // Reads metadata if exists, otherwise empty string

                return Task.FromResult(new DownloadFileResponse
                {
                    Filename = Path.GetFileName(filePath), // Gets the filename
                    Content = ByteString.CopyFrom(File.ReadAllBytes(filePath)), // Reads and copies file content
                    Metadata = metadata // Returns the metadata
                });
            }
            catch (Exception ex) // Catches any exceptions during download
            {
                _logger.Error($"Error downloading file: {ex.Message}"); // Logs the error
                throw new RpcException(new Status(StatusCode.Internal, ex.Message)); // Throws an RpcException
            }
        }

        // Handles requests to get file metadata
        public override Task<GetFileMetadataResponse> GetFileMetadata(GetFileMetadataRequest request, ServerCallContext context)
        {
            try
            {
                var filePath = Path.Combine(_storagePath, request.FileId); // Constructs the file path
                if (!File.Exists(filePath)) // Checks if the file exists
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "File not found")); // Throws NotFound if file doesn't exist
                }

                var metadataPath = filePath + ".metadata"; // Constructs the metadata path
                var metadata = File.Exists(metadataPath) ? File.ReadAllText(metadataPath) : ""; // Reads metadata if exists, otherwise empty string

                return Task.FromResult(new GetFileMetadataResponse
                {
                    Filename = Path.GetFileName(filePath), // Gets the filename
                    Metadata = metadata, // Returns the metadata
                    Size = new FileInfo(filePath).Length, // Gets the file size
                    CreatedAt = File.GetCreationTimeUtc(filePath).ToString("o") // Gets creation time in ISO 8601 format
                });
            }
            catch (Exception ex) // Catches any exceptions during metadata retrieval
            {
                _logger.Error($"Error getting file metadata: {ex.Message}"); // Logs the error
                throw new RpcException(new Status(StatusCode.Internal, ex.Message)); // Throws an RpcException
            }
        }

        // Handles file deletion requests
        public override Task<DeleteFileResponse> DeleteFile(DeleteFileRequest request, ServerCallContext context)
        {
            try
            {
                var filePath = Path.Combine(_storagePath, request.FileId); // Constructs the file path
                if (!File.Exists(filePath)) // Checks if the file exists
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "File not found")); // Throws NotFound if file doesn't exist
                }

                // Delete file and metadata
                File.Delete(filePath); // Deletes the file
                var metadataPath = filePath + ".metadata"; // Constructs the metadata path
                if (File.Exists(metadataPath)) // Checks if metadata file exists
                {
                    File.Delete(metadataPath); // Deletes the metadata file
                }

                _logger.Information($"File deleted: {request.FileId}"); // Logs successful deletion

                return Task.FromResult(new DeleteFileResponse
                {
                    Status = "success", // Sets success status
                    Message = "File deleted successfully" // Sets success message
                });
            }
            catch (Exception ex) // Catches any exceptions during deletion
            {
                _logger.Error($"Error deleting file: {ex.Message}"); // Logs the error
                throw new RpcException(new Status(StatusCode.Internal, ex.Message)); // Throws an RpcException
            }
        }
    }
}