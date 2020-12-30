using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.IO;

namespace Elgin.Portal.Services.Implementation
{
    public class NotificacaoService : AbstractService<NotificacaoService>
    {
        public NotificacaoService(string connectionString) : base(connectionString) { }
        
        public List<Notificacao> RetornaNotificacoes(Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Notificacao>(
                @"
                SELECT [Id]
                      ,[IdUsuario]
                      ,[Tipo]
                      ,[Descricao]
                      ,[Link]
                      ,[DataInclusao]
                      ,[DataVisualizacao]
                  FROM [dbo].[Notificacao]
                  WHERE IdUsuario = @idUsuario AND DataExclusao IS NULL
                  ORDER BY DataInclusao DESC
                ", new { idUsuario }).ToList();
            }
        }

        public void VisualizarNotificacoes(Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE Notificacao SET DataVisualizacao = GETDATE()
                    WHERE IdUsuario = @idUsuario 
                ", new { idUsuario });
            }
        }
        public void LimparNotificacoes(Guid idUsuario)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE Notificacao SET DataExclusao = GETDATE()
                    WHERE IdUsuario = @idUsuario 
                ", new { idUsuario });
            }
        }

    }
}
