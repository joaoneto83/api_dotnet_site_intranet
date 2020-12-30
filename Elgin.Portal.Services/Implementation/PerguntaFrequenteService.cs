using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Elgin.Portal.Services.Implementation
{
    public class PerguntaFrequenteService : AbstractService<PerguntaFrequenteService>
    {

        public PerguntaFrequenteService(string connectionString) : base(connectionString) { }

        public List<PerguntaFrequente> ListarPorCodigoComponente(string codigo)
        {
            var retorno = new List<PerguntaFrequente>();

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<PerguntaFrequente>(
                @"
                    SELECT [Id]
                          ,[Pergunta]
                          ,[Resposta]
                          ,[CodigoComponente]
                    FROM PerguntaFrequente
                    WHERE CodigoComponente = @codigo
                    ORDER BY Ordem
                ", new { codigo }).ToList();
            }

        }

    }
}
