using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class connModel
    {
        private string host;
        private string user;
        private string password;
        private string db;
        private string port;

        public connModel()
        {
            this.host = "localhost";
            this.user = "password";
            this.password = "password";
            this.port = "1234";
            this.db = "db";
        }

        public connModel(string host, string user, string password,string port, string db)
        {
            this.host = host;
            this.user = user;
            this.password = password;
            this.port = port;
            this.db = db;
        }

        public string getHost()
        {
            return this.host;
        }
        public string getUser() {
            return this.user;
        }

        public string getPassword()
        {
            return this.password;
        }
        
        public string getPort()
        {
            return this.port;
        }
        public string getDb()
        {
            return this.db;
        }

        public void setHost(string host)
        {
            this.host = host;
        }

        public void setUser(string user)
        {
            this.user = user;
        }

        public void setPassword(string password)
        {
            this.password = password;
        }

        public void setPort(string port)
        {
            this.port = port;
        }

        public void setDb(string db)
        {
            this.db = db;
        }
    }
}
