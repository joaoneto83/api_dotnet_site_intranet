using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.Transactions;

namespace Elgin.Portal.Services.Implementation
{
    public class PerfilService : AbstractService<PerfilService>
    {
        public PerfilService(string connectionString) : base(connectionString) { }

        public List<Perfil> Listar()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Perfil>(
                @"
                    SELECT  
                        Id,
		                Nome,
                        Ativo
                    FROM Perfil
                        WHERE Ativo = 1
                    ORDER BY Nome
                ").ToList();
            }
        }

        public Perfil ProcurarPorId(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                return conexao.Query<Perfil>(
                @"
                    SELECT [Id]
                          ,[Nome]
                          ,[Ativo]
                    FROM [Perfil]
                    WHERE Id = @id
                    ORDER BY Nome
                ", new { id }).FirstOrDefault();
            }
        }

        public List<Menu> ListarMenusFuncionalidades(Guid idPerfil)
        {
            var retorno = new List<Menu>();

            var sql = @"
                   SELECT 
                        Menu.Id,
                        Menu.Nome,
                        CASE WHEN PerfilFuncionalidade.IdPerfil IS NOT NULL THEN 1 ELSE 0 END AS Selecionado,
                        Funcionalidade.Id,
                        Funcionalidade.Nome,
	                    PerfilFuncionalidade.IdPerfil 
                    FROM Menu
                    INNER JOIN Funcionalidade
                    ON Funcionalidade.IdMenu = Menu.Id
                    LEFT JOIN PerfilFuncionalidade
                    ON Funcionalidade.Id = PerfilFuncionalidade.IdFuncionalidade AND 
					(
                        PerfilFuncionalidade.IdPerfil IS NULL 
                        OR PerfilFuncionalidade.IdPerfil = @idPerfil
					)
                    ORDER BY Ordem, Funcionalidade.Nome

                ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Menu>();

                retorno = conexao.Query<Menu, Funcionalidade, Menu>(sql,
                (menu, funcionalidade) =>
                {
                    Menu menuEntry;

                    if (!dictionary.TryGetValue(menu.Id, out menuEntry))
                    {
                        menuEntry = menu;
                        menuEntry.Funcionalidades = new List<Funcionalidade>();
                        dictionary.Add(menuEntry.Id, menuEntry);
                    }

                    if (funcionalidade != null && !menuEntry.Funcionalidades.Any(x => x.Id == funcionalidade.Id))
                        menuEntry.Funcionalidades.Add(funcionalidade);

                    return menuEntry;
                }, new { idPerfil },
                splitOn: "Selecionado").Distinct().ToList();
            }

            return retorno;
        }

        public List<Menu> RetornaMenu(Guid id)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var menus = conexao.Query<Menu>(
                @"
                    SELECT 
                    DISTINCT 
                        Menu.Id,
                        Menu.Nome,
                        Menu.Codigo,
                        Menu.Rota,
                        Menu.Ordem,
                        Menu.IdMenuPai
                    FROM Menu
                    JOIN Funcionalidade
                    ON Funcionalidade.IdMenu = Menu.Id
                    JOIN PerfilFuncionalidade PF
                    ON PF.IdFuncionalidade = Funcionalidade.Id
                    JOIN Usuario
                    ON Usuario.IdPerfil = PF.IdPerfil
					WHERE
                    Menu.Ativo = 1 AND Usuario.Id = @id
                    
                    UNION ALL

                    SELECT 
                    DISTINCT 
                        MenuPai.Id,
                        MenuPai.Nome,
                        MenuPai.Codigo,
                        MenuPai.Rota,
                        MenuPai.Ordem,
                        NULL as IdMenuPai
                    FROM Menu
                    JOIN Funcionalidade
                    ON Funcionalidade.IdMenu = Menu.Id
                    JOIN PerfilFuncionalidade PF
                    ON PF.IdFuncionalidade = Funcionalidade.Id
                    JOIN Usuario
                    ON Usuario.IdPerfil = PF.IdPerfil
                    JOIN Menu MenuPai
                    ON MenuPai.Id = Menu.IdMenuPai
					WHERE
                    Menu.Ativo = 1 AND Usuario.Id = @id
                ", new { id }).ToList();

                menus.ForEach(x => {
                    x.Filhos.AddRange(menus.Where(y => y.IdMenuPai == x.Id).OrderBy(y => y.Ordem));
                });

                return menus.OrderBy(x => x.Ordem).ToList();
            }
        }

        public bool Salvar(Perfil model)
        {
            try
            {
                using (var transaction = new TransactionScope())
                {
                    if(model.Id == Guid.Empty)
                    {
                        model.Id = Guid.NewGuid();
                        IncluiPerfil(model);
                    }
                    else
                    {
                        EditarPerfil(model);
                    }

                    var funcionalidades = new List<PerfilFuncionalidade>();
                    foreach (var menu in model.Menus)
                    {
                        funcionalidades.AddRange( 
                            menu.Funcionalidades.Where(x => x.Selecionado).Select(x => 
                                    new PerfilFuncionalidade {
                                        IdFuncionalidade = x.Id,
                                        IdPerfil = model.Id
                                    }));
                    }

                    RemoverPerfilFuncionalidades(model.Id);
                    IncluirPerfilFuncionalidades(funcionalidades);

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void EditarPerfil(Perfil model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    UPDATE [dbo].[Perfil]
                        SET 
                            [Nome] = @Nome
                            ,[Ativo] = @Ativo
                    WHERE Id = @Id
                ", model);
            }
        }

        private void IncluiPerfil(Perfil model)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [dbo].[Perfil]
                        ([Id]
                        ,[Nome]
                        ,[Ativo])
                    VALUES
                        (@Id
                        ,@Nome
                        ,@Ativo)
               ", model);
            }
        }

        private void RemoverPerfilFuncionalidades( Guid idPerfil)
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    DELETE
                        FROM [PerfilFuncionalidade]
                    WHERE
                        IdPerfil = @idPerfil 
               ", new { idPerfil });
            }
        }

        private void IncluirPerfilFuncionalidades(List<PerfilFuncionalidade> model)
        {
            
            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                conexao.Execute(
                @"
                    INSERT INTO [dbo].[PerfilFuncionalidade]
                        ([IdPerfil]
                        ,[IdFuncionalidade])
                    VALUES
                        (@IdPerfil
                        ,@IdFuncionalidade)

               ", model);
            }
        }
    }
}
