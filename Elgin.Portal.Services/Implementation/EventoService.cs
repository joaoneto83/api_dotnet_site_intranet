using Dapper;
using Elgin.Portal.Services.Implementation.Shared;
using Elgin.Portal.Services.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Elgin.Portal.Services.Implementation
{
    public class EventoService : AbstractService<EventoService>
    {
        public EventoService(string connectionString) : base(connectionString) { }

        public List<Evento> Listar()
        {
            var retorno = new List<Evento>();

            var sql = @"
                    SELECT 
                        Id,
                        Titulo,
                        (SELECT CONVERT(varchar, DataDe, 23)) AS DataDe,
                        DataDe AS dtDataDe,
                        (SELECT CONVERT(varchar, DataAte, 23)) AS DataAte,
                        DataAte AS dtDataAte,
                        Descricao,  
                        Ativo
                    FROM Evento
                    WHERE Ativo = 1
                    ORDER BY
                    DataDe, DataAte
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                retorno = conexao.Query<Evento>(sql).ToList();
            }

            return retorno;
        }

        public bool Salvar(Evento model)
        {
            try
            {
                if (model.Id != Guid.Empty)
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             UPDATE [Evento]
                                SET [Titulo] = @Titulo
                                    ,[Descricao] = @Descricao
                                    ,[DataDe] = @DataDe
                                    ,[DataAte] = @DataAte
                                    ,[Ativo] = @Ativo
                                WHERE Id = @Id
                        ", model);
                    }
                }

                else
                {
                    model.Id = Guid.NewGuid();

                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        conexao.Execute(
                        @"
                             INSERT INTO [Evento]
                                   ([Id]
                                   ,[Titulo]
                                   ,[Descricao]
                                   ,[DataDe]
                                   ,[DataAte]
                                   ,[Ativo])
                             VALUES
                                   (@Id
                                   ,@Titulo
                                   ,@Descricao
                                   ,@DataDe
                                   ,@DataAte
                                   ,@Ativo)
                        ", model);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public List<Feriado> ListarEventosParaCalendario(int ano)
        {
            var retorno = new List<Feriado>();
            var eventos = new List<Evento>();

            var sql = @"
                    SELECT 
                        Titulo,
                        DataDe AS dtDataDe,
                        DataAte AS dtDataAte
                    FROM Evento
                    WHERE Ativo = 1 AND YEAR(DataDe) = @ano
                    ORDER BY
                    DataDe, DataAte
                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                eventos = conexao.Query<Evento>(sql, new { ano }).ToList();
            }

            foreach(var evento in eventos)
            {
                var feriado = new Feriado() { Name = evento.Titulo };
                string dateFeriado;
                if(evento.dtDataDe.Month == evento.dtDataAte.Month)
                {
                    if (evento.dtDataDe.Day == evento.dtDataAte.Day)
                        dateFeriado = evento.dtDataDe.Day.ToString();
                    else
                        dateFeriado = evento.dtDataDe.Day + " a " + evento.dtDataAte.Day;
                    feriado.Dias = dateFeriado;
                    feriado.Mes = evento.dtDataDe.Month;
                    retorno.Add(feriado);
                }
                else
                {
                    for(var mes = evento.dtDataDe.Month; mes <= evento.dtDataAte.Month; mes++)
                    {
                        if(mes == evento.dtDataDe.Month)
                            dateFeriado = evento.dtDataDe.Day + " a " + DateTime.DaysInMonth(ano, mes);
                        else if (mes == evento.dtDataAte.Month)
                            dateFeriado = "1 a " + evento.dtDataAte.Day;
                        else
                            dateFeriado = "1 a " + DateTime.DaysInMonth(ano, mes);

                        feriado.Dias = dateFeriado;
                        feriado.Mes = mes;
                        retorno.Add(feriado);
                        feriado = new Feriado() { Name = evento.Titulo };
                    }
                }
            }

            return retorno;
        }
    }
}
