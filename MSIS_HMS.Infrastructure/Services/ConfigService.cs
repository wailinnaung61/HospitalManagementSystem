using System;
using Microsoft.Extensions.Configuration;
using MSIS_HMS.Infrastructure.Interfaces;

namespace MSIS_HMS.Infrastructure.Services
{
    public class ConfigService : IConfigService
    {
        private readonly string _connectionString;

        public ConfigService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
