using System;
using System.Threading.Tasks;
using Grpc.Core; // Core GRPC functionalities
using Microsoft.Extensions.Logging; // For logging

namespace VerbaGrpc
{
    public class GrpcServer
    {
        private readonly Server _server; // The GRPC server instance
        private readonly ILogger _logger; // Logger instance

        public GrpcServer(int port, string storagePath, ILogger logger)
        {
            _logger = logger; // Initializes the logger
            _server = new Server // Creates a new GRPC server instance
            {
                Services = { FileService.BindService(new FileServiceImpl(storagePath, logger)) }, // Binds the FileServiceImpl to the server
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) } // Configures the server port and credentials
            };
        }

        // Asynchronously starts the GRPC server
        public async Task StartAsync()
        {
            _logger.Information($"Starting GRPC server on port {GetPort()}"); // Logs server start attempt
            await _server.StartAsync(); // Starts the server
            _logger.Information($"GRPC server started on port {GetPort()}"); // Logs successful server start
        }

        // Asynchronously stops the GRPC server
        public async Task StopAsync()
        {
            _logger.Information("Stopping GRPC server"); // Logs server stop attempt
            await _server.ShutdownAsync(); // Shuts down the server
            _logger.Information("GRPC server stopped"); // Logs successful server stop
        }

        // Returns the bound port of the server
        public int GetPort() => _server.Ports[0].BoundPort;

        // Cancels all active GRPC calls on the server
        public void CancelAllCalls()
        {
            _server.CancelAllCalls();
        }
    }
}