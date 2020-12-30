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
    public class TipoUsuarioService : AbstractService<TipoUsuarioService>
    {

        public TipoUsuarioService(string connectionString) : base(connectionString){}

        public List<TipoUsuario> Listar()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<TipoUsuario>(
                @"
                    SELECT Id
                          ,NomeTipoUsuario
                          ,CodigoTipoUsuario
                          ,Ativo
                      FROM TipoUsuario
                      WHERE Ativo = 1
                ", new { }).ToList();
            }

        }
    }
}
