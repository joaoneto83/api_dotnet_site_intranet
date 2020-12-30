using Elgin.Portal.Services.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Implementation.Shared
{
    public abstract class AbstractService<T>
    {
        private string _connectionString;
        protected string ConnectionString => _connectionString;


        private EmailSettings _emaiSettings;
        protected EmailSettings EmailSettings => _emaiSettings;

        private FileSettings _fileSettings;
        protected FileSettings FileSettings => _fileSettings;

        public AbstractService(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        public AbstractService(string connectionString, EmailSettings emaiSettings)
        {
            _connectionString = connectionString;
            _emaiSettings = emaiSettings;
        }

        public AbstractService(string connectionString, FileSettings fileSettings)
        {
            _connectionString = connectionString;
            _fileSettings = fileSettings;
        }

        public AbstractService(string connectionString, EmailSettings emaiSettings, FileSettings fileSettings)
        {
            _connectionString = connectionString;
            _emaiSettings = emaiSettings;
            _fileSettings = fileSettings;
        }
    }
}