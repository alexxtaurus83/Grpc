# My First Try with gRPC

## Overview 📝
The File Service is a gRPC application designed to manage file operations remotely. It provides a set of RPC methods for handling files, including:

* **`UploadFile`**: Uploads a file to the server.
* **`DownloadFile`**: Downloads a file from the server.
* **`GetFileMetadata`**: Retrieves metadata associated with a stored file.
* **`DeleteFile`**: Deletes a file from the server.

The server stores file content and their associated metadata in a specified local directory.

***

## Protocol Buffer Definition 📜
The core of the gRPC service is defined in **`FileService.proto`**. This file specifies the service contract, including the RPC methods and the structure of the request and response messages for various file operations like uploading, downloading, getting metadata, and deleting files.

***

## Service Implementation 🛠️
The **`FileServiceImpl.cs`** file contains the actual business logic for handling the file operations. It implements the base service interface generated from the Protocol Buffer definition.

Key aspects of the implementation:

* **Constructor**: Sets up where files will be saved and a logger for messages.
* **`UploadFile`**: Creates a unique ID for the file, saves the file content, and stores any related metadata.
* **`DownloadFile`**: Retrieves the file content and its metadata using the file's ID.
* **`GetFileMetadata`**: Provides file details like name, metadata, size, and creation time for a given file ID.
* **`DeleteFile`**: Removes both the file and its metadata from storage.
* **Error Handling**: All methods include error handling to catch issues and return appropriate error codes.

***

## gRPC Server Setup 🚀
The **`GrpcServer.cs`** file is responsible for configuring and managing the gRPC server.

Key functionalities:

* **Constructor**: Sets up the gRPC server, connecting it to the file service logic and defining its port.
* **`StartAsync()`**: Starts the gRPC server.
* **`StopAsync()`**: Shuts down the gRPC server.
* **`GetPort()`**: Tells you the port the server is using.
* **`CancelAllCalls()`**: Stops any ongoing gRPC calls on the server.