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
    public class TipoDependenteService : AbstractService<TipoDependenteService>
    {

        public TipoDependenteService(string connectionString) : base(connectionString){}

        public List<TipoDependente> Listar()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<TipoDependente>(
                @"
                    SELECT Id
                          ,NomeTipoDependente
                          ,CodigoTipoDependente
                          ,Ativo
                      FROM TipoDependente
                      WHERE Ativo = 1
                ", new { }).ToList();
            }

        }
    }
}
