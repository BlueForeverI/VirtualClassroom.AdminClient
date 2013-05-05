using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Manages connections to the service
    /// </summary>
    class ClientManager
    {
        private static AdminServiceClient clientInstance;

        /// <summary>
        /// Creates a new connection to the service or uses an existing one
        /// </summary>
        /// <returns>A working connection to the service</returns>
        public static AdminServiceClient GetClient()
        {
            if(clientInstance == null)
            {
                clientInstance = new AdminServiceClient();
            }

            return clientInstance;
        }

        /// <summary>
        /// Closes the existing connection to the service
        /// </summary>
        public static void CloseClient()
        {
            if (clientInstance != null)
            {
                clientInstance.Close();
            }
        }
    }
}
