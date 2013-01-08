using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    class ClientManager
    {
        private static AdminServiceClient clientInstance;

        public static AdminServiceClient GetClient()
        {
            if(clientInstance == null)
            {
                clientInstance = new AdminServiceClient();
            }

            return clientInstance;
        }
    }
}
