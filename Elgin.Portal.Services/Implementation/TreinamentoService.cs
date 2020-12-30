using Elgin.Portal.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

using System.Text;
using Elgin.Portal.Services.Model;
using System.Linq;
using System.IO;
using System.Transactions;

namespace Elgin.Portal.Services.Implementation
{
    public class TreinamentoService : AbstractService<TreinamentoService>
    {
        private ArquivoService arquivoService;

        public TreinamentoService(string connectionString, FileSettings fileSettings) : base(connectionString)
        {
            arquivoService = new ArquivoService(connectionString, fileSettings);
        }

        public List<Linha> getDocumentos()
        {
            var retorno = new List<Linha>();

            var sql = @"
              SELECT 
                Linha.NomeLinha,
                Linha.Id,
                Linha.CorTitulo,
                Linha.Cor1,
                Linha.Cor2,
                Linha.Ordem,
                SubLinha.NomeSubLinha,
                SubLinha.Id,
                Produto.NomeProduto,
                Produto.Id,
                Arquivo.NomeArquivo,
                Arquivo.IdPai,
                Arquivo.Id,
                Arquivo.IdTipoArquivo,
                Arquivo.Caminho,
                Arquivo.Ordem                   
                FROM Arquivo
                JOIN TipoArquivo
                ON TipoArquivo.Id = Arquivo.IdTipoArquivo
                LEFT JOIN Produto
                ON Produto.Id = Arquivo.IdPai
                LEFT JOIN SubLinha
                ON SubLinha.Id = Produto.IdSubLinha
                LEFT JOIN Linha as Linha
                ON Linha.Id = SubLinha.IdLinha
                WHERE 
                TipoArquivo.CodigoTipoArquivo = 'treinamento' AND
                Arquivo.Ativo = 1 AND
                Linha.Ativo = 1 AND
                SubLinha.Ativo = 1 AND
                Produto. Ativo = 1

            UNION ALL

            SELECT 
                Linha.NomeLinha,
                Linha.Id,
                Linha.CorTitulo,
                Linha.Cor1,
                Linha.Cor2,
                Linha.Ordem,
                NULL,
                NULL,
                NULL,
                NULL,
                Arquivo.NomeArquivo,
                Arquivo.IdPai,
                Arquivo.Id,
                Arquivo.IdTipoArquivo,
                Arquivo.Caminho,
                Arquivo.Ordem                   
                FROM Arquivo
                JOIN TipoArquivo
                ON TipoArquivo.Id = Arquivo.IdTipoArquivo
                JOIN Linha as Linha
                ON Linha.Id = Arquivo.IdPai
                WHERE 
                TipoArquivo.CodigoTipoArquivo = 'treinamento' AND
                Arquivo.Ativo = 1 AND
                Linha.Ativo = 1
                        
            ";

            using (SqlConnection conexao = new SqlConnection(ConnectionString))
            {
                var dictionary = new Dictionary<Guid, Linha>();

                retorno = conexao.Query<Linha, SubLinha, Produto, Arquivo, Linha>(sql,
                (linha, sublinha, produto, arquivo) =>
                {
                    Linha _linha;

                    if (!dictionary.TryGetValue(linha.Id, out _linha))
                    {
                        _linha = linha;
                        _linha.SubLinhas = new List<SubLinha>();
                        _linha.Arquivos = new List<Arquivo>();
                        dictionary.Add(_linha.Id, _linha);
                    }

                    if (sublinha == null)
                    {
                        _linha.Arquivos.Add(arquivo);
                        return _linha;
                    }

                    if (sublinha != null && !_linha.SubLinhas.Any(x => x.Id == sublinha.Id))
                    {
                        sublinha.Produtos = new List<Produto>();
                        _linha.SubLinhas.Add(sublinha);
                    }

                    if (produto != null && !_linha.SubLinhas.FirstOrDefault(x => x.Id == sublinha.Id).Produtos.Any(x => x.Id == produto.Id))
                    {
                        produto.Arquivos = new List<Arquivo>();
                        _linha.SubLinhas.FirstOrDefault(x => x.Id == sublinha.Id).Produtos.Add(produto);
                    }

                    if (arquivo != null)
                    {
                        _linha.SubLinhas.FirstOrDefault(x => x.Id == sublinha.Id).Produtos.FirstOrDefault(x => x.Id == produto.Id).Arquivos.Add(arquivo);
                    }

                    return _linha;
                }, new { },
                splitOn: "NomeSubLinha,NomeProduto,NomeArquivo").Distinct().ToList();
            }

            return retorno;
        }

        public bool salvar(DocumentoTreinamento model)
        {
            var idPai = model.IdLinha;
            if (model.IdProduto.HasValue)
            {
                idPai = model.IdProduto.Value;
            }

            model.Arquivos.ForEach(x =>
            {
                x.IdPai = idPai;
                x.Ativo = true;
            });

            var updateList = model.Arquivos.Where(x => x.Id != Guid.Empty).ToList();
            var insertList = model.Arquivos.Where(x => x.Id == Guid.Empty).ToList();
            var idsList = model.Arquivos.Select(x => x.Id).ToList();

            try
            {
                using (var transaction = new TransactionScope())
                {
                    using (SqlConnection conexao = new SqlConnection(ConnectionString))
                    {
                        arquivoService.DelArquivos(idsList, idPai, "treinamento");
                        arquivoService.InsertArquivo(insertList);
                        arquivoService.UpdateArquivo(updateList);
                    }

                    transaction.Complete();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private string GetExtension(string caminho)
        {
            var extension = "";

            try
            {
                var split = caminho.Split(".");
                extension = split[split.Length - 1].ToUpper();
            }
            catch (Exception ex)
            {
                extension = "ARQUIVO";
            }

            return extension;
        }
    }
}
