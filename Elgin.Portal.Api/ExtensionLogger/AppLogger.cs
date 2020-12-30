using Elgin.Portal.Services.Implementation;
using Elgin.Portal.Services.Model;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Elgin.Portal.Api.ExtensionLogger
{
    public class AppLogger : ILogger
    {
        private readonly string _nomeCategoria;
        private readonly Func<string, LogLevel, bool> _filtro;
        private readonly InfraService _service;
        private readonly int _messageMaxLength = 4000;

        public AppLogger(string nomeCategoria, Func<string, LogLevel, bool> filtro, string connectionString)
        {
            _nomeCategoria = nomeCategoria;
            _filtro = filtro;
            _service = new InfraService(connectionString);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventoId,
            TState state, Exception exception, Func<TState, Exception, string> formato)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formato == null)
                throw new ArgumentNullException(nameof(formato));

            var mensagem = formato(state, exception);
            if (string.IsNullOrEmpty(mensagem))
            {
                return;
            }

            if (exception != null)
                mensagem += $"\n{exception.ToString()}";

            mensagem = mensagem.Length > _messageMaxLength ? mensagem.Substring(0, _messageMaxLength) : mensagem;

            //try
            //{
            //    using (StreamWriter writer = new StreamWriter("log.txt"))
            //    {
            //        writer.WriteLine($"== {DateTime.Now} ==");
            //        writer.WriteLine(mensagem);
            //        writer.WriteLine($"==========================");
            //    }
            //}
            //catch(Exception ex)
            //{

            //}

            var eventLog = new LogApi()
            {
                Message = mensagem,
                EventId = eventoId.Id,
                LogLevel = logLevel.ToString(),
                CreatedTime = DateTime.UtcNow
            };

            _service.GravaLogApi(eventLog);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filtro == null || _filtro(_nomeCategoria, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}