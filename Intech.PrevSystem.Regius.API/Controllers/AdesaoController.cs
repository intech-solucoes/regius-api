#region Usings
using Intech.Lib.Email;
using Intech.Lib.Util.Seguranca;
using Intech.Lib.Util.Validacoes;
using Intech.Lib.Web;
using Intech.PrevSystem.Entidades;
using Intech.PrevSystem.Negocio;
using Intech.PrevSystem.Negocio.Proxy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Transactions;
#endregion

namespace Intech.PrevSystem.Regius.API.Controllers
{
    /// <service nome="Adesao" />
    [Route("[controller]")]
    [ApiController]
    public class AdesaoController : ControllerBase
    {
        /// <rota caminho="[action]/{email}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="email" tipo="string" />
        /// </parametros>
        /// <retorno tipo="string" />
        [HttpGet("[action]/{email}")]
        public IActionResult EnviarEmail(string email)
        {
            try
            {
                var token = new Random().Next(999999);

                var emailConfig = AppSettings.Get().Email;
                var corpoEmail =
                    $"Olá,<br/>" +
                    $"Este é seu número de confirmação de e-mail: {token}";
                EnvioEmail.Enviar(emailConfig, email, $"Regius - Confirmação de E-mail", corpoEmail);

                return Ok(Criptografia.Encriptar(token.ToString()));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]/{tokenDigitado}/{tokenEnviado}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="tokenDigitado" tipo="string" />
        ///     <parametro nome="tokenEnviado" tipo="string" />
        /// </parametros>
        /// <retorno tipo="string" />
        [HttpGet("[action]/{tokenDigitado}/{tokenEnviado}")]
        public IActionResult ConfirmarToken(string tokenDigitado, string tokenEnviado)
        {
            try
            {
                tokenDigitado = Criptografia.Encriptar(tokenDigitado);

                if (tokenDigitado == tokenEnviado)
                    return Ok();

                throw new Exception("Código inválido!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]" tipo="GET" />
        /// <retorno tipo="EmpresaEntidade" lista="true" />
        [HttpGet("[action]")]
        public IActionResult BuscarEmpresas()
        {
            try
            {
                return Ok(new EmpresaProxy().BuscarTodas());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]" tipo="GET" />
        /// <retorno tipo="any" lista="true" />
        [HttpGet("[action]")]
        public IActionResult BuscarListaSexo()
        {
            try
            {
                var listaSexos = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("F", "FEMININO"),
                    new KeyValuePair<string, string>("M", "MASCULINO")
                };

                return Ok(listaSexos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]" tipo="GET" />
        /// <retorno tipo="GrauParentescoEntidade" lista="true" />
        [HttpGet("[action]")]
        public IActionResult BuscarListaGrauParentesco()
        {
            try
            {
                var listaGrauParentesco = new GrauParentescoProxy().Listar();
                return Ok(listaGrauParentesco);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]" tipo="GET" />
        /// <retorno tipo="EstadoCivilEntidade" lista="true" />
        [HttpGet("[action]")]
        public IActionResult BuscarListaEstadoCivil()
        {
            try
            {
                var listaEstadoCivil = new EstadoCivilProxy().Listar().OrderBy(x => x.DS_ESTADO_CIVIL).ToList();
                return Ok(listaEstadoCivil);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]" tipo="GET" />
        /// <retorno tipo="NacionalidadeEntidade" lista="true" />
        [HttpGet("[action]")]
        public IActionResult BuscarListaNacionalidade()
        {
            try
            {
                var listaNacionalidade = new NacionalidadeProxy().Listar();
                return Ok(listaNacionalidade);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]" tipo="GET" />
        /// <retorno tipo="UFEntidade" lista="true" />
        [HttpGet("[action]")]
        public IActionResult BuscarListaUF()
        {
            try
            {
                var uf = new UFProxy().Listar().OrderBy(x => x.DS_UNID_FED).ToList();

                return Ok(uf);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]" tipo="GET" />
        /// <retorno tipo="BancoAgEntidade" lista="true" />
        [HttpGet("[action]")]
        public IActionResult BuscarListaBancos()
        {
            try
            {
                var bancos = new BancoAgProxy().BuscarBancos();

                return Ok(bancos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]/{cdPlano}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="cdPlano" tipo="string" />
        /// </parametros>
        /// <retorno tipo="any" lista="true" />
        [HttpGet("[action]/{cdPlano}")]
        public IActionResult BuscarPercentuais(string cdPlano)
        {
            try
            {
                var listaPercentuais = new List<KeyValuePair<decimal, string>>();

                var limite = new LimiteContribuicaoProxy()
                            .Listar()
                            .Single(x => x.CD_PLANO == cdPlano);

                for (decimal i = limite.VAL_PERC_MINIMO_PART; i <= limite.VAL_PERC_MAXIMO_PART; i++)
                    listaPercentuais.Add(new KeyValuePair<decimal, string>(i, $"{i}%"));

                return Ok(listaPercentuais);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]/{cdPlano}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="cdPlano" tipo="string" />
        /// </parametros>
        /// <retorno tipo="LimiteContribuicaoEntidade" />
        [HttpGet("[action]/{cdPlano}")]
        public IActionResult BuscarLimitePatrocinadora(string cdPlano)
        {
            try
            {
                var listaPercentuais = new List<KeyValuePair<decimal, string>>();

                var limite = new LimiteContribuicaoProxy()
                            .Listar()
                            .Single(x => x.CD_PLANO == cdPlano);

                return Ok(limite);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]/{cdEmpresa}/{numMatricula}/{cpf}/{dataNascimento}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="cdEmpresa" tipo="string" />
        ///     <parametro nome="numMatricula" tipo="string" />
        ///     <parametro nome="cpf" tipo="string" />
        ///     <parametro nome="dataNascimento" tipo="string" />
        /// </parametros>
        /// <retorno tipo="any" />
        [HttpGet("[action]/{cdEmpresa}/{numMatricula}/{cpf}/{dataNascimento}")]
        public IActionResult BuscarFuncionario(string cdEmpresa, string numMatricula, string cpf, string dataNascimento)
        {
            try
            {
                cpf = cpf.LimparMascara();
                numMatricula = numMatricula.PadLeft(9, '0');
                var dtNascimento = DateTime.ParseExact(dataNascimento, "dd.MM.yyyy", new CultureInfo("pt-BR"));

                var planosVinculados = new PlanoVinculadoProxy().BuscarPorFundacaoMatricula("01", numMatricula).ToList();

                if (planosVinculados.Count > 0 && planosVinculados[0].CD_PLANO != "0001")
                {
                    var dados = new FuncionarioProxy().BuscarPorMatricula(numMatricula);
                    return Ok(new
                    {
                        tipo = "plano",
                        plano = planosVinculados[0],
                        dados
                    });
                }

                var funcionarioNP = new FuncionarioNPProxy().BuscarPorFundacaoEmpresaMatriculaCpfDataNascimento("01", cdEmpresa, numMatricula, cpf, dtNascimento);

                if (funcionarioNP == null)
                    return Ok("funcionarioNP");

                if (funcionarioNP.E_MAIL == null || funcionarioNP.E_MAIL.Trim() == "")
                    return Ok("email");

                var adesao = new AdesaoProxy().BuscarPorCpf(cpf);

                if (adesao != null)
                    return Ok("adesao");

                return Ok(funcionarioNP);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]/{cdEmpresa}/{numMatricula}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="cdEmpresa" tipo="string" />
        ///     <parametro nome="numMatricula" tipo="string" />
        /// </parametros>
        /// <retorno tipo="any" lista="true" />
        [HttpGet("[action]/{cdEmpresa}/{numMatricula}")]
        public IActionResult BuscarPlanos(string cdEmpresa, string numMatricula)
        {
            try
            {
                var planos = new AdesaoEmpresaPlanoProxy().BuscarPorEmpresa(cdEmpresa).ToList();

                var planoCD = new
                {
                    CD_PLANO = "0002",
                    DS_PLANO = "Plano CD",
                    CPNB = "2012.0017-18",
                    Texto = "<p>Com o objetivo de possibilitar aos participantes ativos do Plano BD-01 a formação de uma reserva financeira para um benefício adicional futuro foi criado o Plano CD-02, " +
                    "na modalidade de Contribuição Definida, que tem como base de cálculo o montante constituído pelas contribuições vertidas para o seu custeio e o correspondente retorno líquido dos investimentos, " +
                    "apurado nos termos do Regulamento do Plano</p>" +
                    "<p>O Plano CD - 02 teve início em 27 / 09 / 2012 e está em plena fase de acumulação de recursos.</p>" +
                    "<p>Consulte a <a href=\"http://www.regius.org.br/images/arquivos/Cartilhas/Cartilha_PlanoCD02_2017.pdf\" target=\"_blank\">Cartilha do Plano CD-02</a> e conheça as principais regras previstas " +
                    "no regulamento deste o Plano.</p>"
                };

                var planoCV = new
                {
                    CD_PLANO = "0003",
                    DS_PLANO = "Plano CV",
                    CPNB = "2000.0025-51",
                    Texto = "<p>O Plano CV-03 teve início em 01/03/2000 e foi elaborado na modalidade de Contribuição Variável, com regras de benefício definido para os casos de risco (invalidez e morte) e com regras de contribuição definida para os benefícios programáveis (aposentadoria por tempo de contribuição e idade)</p>" +
                            "<p>O participante do Plano CV-03 pode planejar seu benefício futuro, estabelecendo seu percentual de contribuição, que pode ser alterado a qualquer tempo, de acordo com seu planejamento pessoal.</p>" +
                            "<p>Consulte a <a href=\"http://www.regius.org.br/images/arquivos/Cartilhas/Cartilha_PlanoCV03_2017.pdf\" target=\"_blank\">Cartilha do Plano CV-03</a> e conheça as principais regras " +
                            "previstas no regulamento deste Plano.</p>"
                };

                var planoCDMetro = new
                {
                    CD_PLANO = "0004",
                    DS_PLANO = "Plano CD-Metrô",
                    CPNB = "2014.0021-18",
                    Texto = "<p>O Plano CD-Metrô-DF teve início em 16/12/2014 e foi elaborado na modalidade de Contribuição Definida, especialmente para os funcionários da Companhia do Metropolitano do Distrito Federal - Metrô-DF. Tem como base de cálculo o montante constituído pelas contribuições vertidas para o seu custeio e o correspondente retorno líquido dos investimentos, apurado nos termos do Regulamento do Plano.<p>" +
                            "<p>O participante do Plano CD-Metrô - DF pode planejar seu benefício futuro, estabelecendo seu percentual de contribuição, que pode ser alterado anualmente, de acordo com seu planejamento pessoal.</p>" +
                            "<p>Consulte a <a href=\"http://www.regius.org.br/images/arquivos/Cartilhas/Cartilha_PlanoCD-MetroDF_2017.pdf\" target=\"_blank\">Cartilha do Plano CD-Metrô-DF</a> e conheça as principais regras " +
                            "previstas no regulamento deste Plano.</p>"
                };

                var planoCD05 = new
                {
                    CD_PLANO = "0005",
                    DS_PLANO = "Plano CD-05",
                    CPNB = "2017.0001-83",
                    Texto = "<p>O Plano CD-05 teve início em 13/02/2017 e foi elaborado na modalidade de Contribuição Definida, tem como base de cálculo o montante constituído pelas contribuições vertidas para o seu custeio e o correspondente retorno líquido dos investimentos, apurado nos termos do Regulamento do Plano.</p>" +
                            "<p>O participante do Plano CD-05 pode planejar seu benefício futuro, estabelecendo seu percentual de contribuição, que pode ser alterado a qualquer tempo, de acordo com seu planejamento pessoal.</p>" +
                            "<p>Consulte a <a href=\"http://www.regius.org.br/images/arquivos/Cartilhas/Cartilha_PlanoCD-05_2017.pdf\" target=\"_blank\">Cartilha do Plano CD-05</a> e conheça as principais regras " +
                            "previstas no regulamento deste Plano.</p>"
                };

                var planosVinculados = new PlanoVinculadoProxy().BuscarPorFundacaoEmpresaMatricula("01", cdEmpresa, numMatricula);
                var planoBD = planosVinculados.SingleOrDefault(x => x.CD_PLANO == "0001" && x.CD_CATEGORIA != "2");

                var retorno = new List<dynamic>();

                if (planos.Any(x => x.CD_PLANO == planoCD.CD_PLANO) && planoBD != null)
                    retorno.Add(planoCD);

                if (planos.Any(x => x.CD_PLANO == planoCV.CD_PLANO))
                    retorno.Add(planoCV);

                if (planos.Any(x => x.CD_PLANO == planoCDMetro.CD_PLANO))
                    retorno.Add(planoCDMetro);

                if (planos.Any(x => x.CD_PLANO == planoCD05.CD_PLANO))
                    retorno.Add(planoCD05);

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]"), DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        public IActionResult Upload([FromForm] FileUploadViewModel model)
        {
            var file = model.File;

            var guid = $"{Guid.NewGuid()}.{file.FileName.Split('.').Last()}";

            var documento = new AdesaoDocumentoEntidade
            {
                TXT_NOME_FISICO = guid,
                TXT_TITULO = model.Nome
            };

            var diretorioUpload = Path.Combine(Environment.CurrentDirectory, "Upload");

            if (file.Length > 0)
            {
                if (!Directory.Exists(diretorioUpload))
                    Directory.CreateDirectory(diretorioUpload);

                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                string fullPath = Path.Combine(diretorioUpload, guid);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                var oidArquivo = new AdesaoDocumentoProxy().Inserir(documento);
                documento.OID_ADESAO_DOCUMENTO = oidArquivo;
            }

            return Ok(documento);
        }

        /// <rota caminho="[action]/{oid}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="oid" tipo="number" />
        /// </parametros>
        /// <retorno tipo="any" />
        [HttpGet("[action]/{oid}")]
        public IActionResult ExcluirArquivo(decimal oid)
        {
            var arquivo = new AdesaoDocumentoProxy().BuscarPorChave(oid);
            if (arquivo != null)
            {
                var caminhoArquivo = Path.Combine(Environment.CurrentDirectory, "Upload", arquivo.TXT_NOME_FISICO);
                System.IO.File.Delete(caminhoArquivo);

                new AdesaoDocumentoProxy().Deletar(arquivo);
            }

            return Ok();
        }

        /// <rota caminho="[action]" tipo="POST" />
        /// <parametros>
        ///     <parametro nome="adesao" tipo="AdesaoEntidade" />
        /// </parametros>
        /// <retorno tipo="string" />
        [HttpPost("[action]")]
        public IActionResult Inserir(AdesaoEntidade adesao)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    adesao.DTA_CRIACAO = DateTime.Now;
                    adesao.IND_SIT_ADESAO = "";
                    adesao.COD_CPF = adesao.COD_CPF.LimparMascara();
                    adesao.COD_CEP = adesao.COD_CEP.LimparMascara();

                    var oidAdesao = new AdesaoProxy().Inserir(adesao);

                    adesao.Plano.OID_ADESAO = oidAdesao;
                    var oidAdesaoPlano = new AdesaoPlanoProxy().Inserir(adesao.Plano);

                    adesao.Contrib.OID_ADESAO_PLANO = oidAdesaoPlano;
                    var oidAdesaoContrib = new AdesaoContribProxy().Inserir(adesao.Contrib);

                    foreach (var dep in adesao.Dependentes)
                    {
                        dep.OID_ADESAO = oidAdesao;
                        dep.COD_CPF = dep.COD_CPF.LimparMascara();

                        new AdesaoDependenteProxy().Inserir(dep);
                    }

                    foreach (var doc in adesao.Documentos)
                    {
                        var documento = new AdesaoDocumentoProxy().BuscarPorChave(doc.OID_ADESAO_DOCUMENTO);
                        documento.OID_ADESAO = oidAdesao;
                        new AdesaoDocumentoProxy().Atualizar(documento);
                    }

                    var listaConteudo = new List<ItemTransacao>
                    {
                        new ItemTransacao("Nome", adesao.NOM_PESSOA),
                        new ItemTransacao("Data de Nascimento", adesao.DTA_NASCIMENTO.ToString("dd/MM/yyyy")),
                        new ItemTransacao("CPF", adesao.COD_CPF),
                        new ItemTransacao("E-mail", adesao.COD_EMAIL),
                        new ItemTransacao("Patrocinadora", adesao.DES_EMPRESA),
                        new ItemTransacao("Matrícula", adesao.COD_MATRICULA),

                        new ItemTransacao("Data de Admissão", adesao.DTA_ADMISSAO.ToString("dd/MM/yyyy")),
                        new ItemTransacao("Sexo", adesao.DES_SEXO),
                        new ItemTransacao("Nacionalidade", adesao.DES_NACIONALIDADE),
                        new ItemTransacao("Naturalidade", adesao.DES_NATURALIDADE),
                        new ItemTransacao("UF", adesao.DES_END_UF),
                        new ItemTransacao("RG", adesao.COD_RG),
                        new ItemTransacao("Órgão Expeditor", adesao.DES_ORGAO_EXPEDIDOR),
                        new ItemTransacao("Emissão", adesao.DTA_EXPEDICAO_RG.Value.ToString("dd/MM/yyyy")),
                        new ItemTransacao("Estado Civil", adesao.DES_ESTADO_CIVIL),
                        new ItemTransacao("Nome da Mãe", adesao.NOM_MAE),
                        new ItemTransacao("Nome do Pai", adesao.NOM_PAI),
                        new ItemTransacao("CEP", adesao.COD_CEP),
                        new ItemTransacao("Endereço", adesao.DES_END_LOGRADOURO),
                        new ItemTransacao("Numero", adesao.DES_END_NUMERO),
                        new ItemTransacao("Complemento", adesao.DES_END_COMPLEMENTO),
                        new ItemTransacao("Bairro", adesao.DES_END_BAIRRO),
                        new ItemTransacao("Cidade", adesao.DES_END_CIDADE),
                        new ItemTransacao("UF", adesao.DES_END_UF),
                        new ItemTransacao("Telefone Fixo", adesao.COD_TELEFONE_FIXO),
                        new ItemTransacao("Telefone Celular", adesao.COD_TELEFONE_CELULAR),
                        new ItemTransacao("Pessoa Politicamente Exposta", adesao.IND_PPE),
                        new ItemTransacao("Familiar Politicamente Exposto", adesao.IND_PPE_FAMILIAR),
                        new ItemTransacao("US Person", adesao.IND_FATCA),
                        new ItemTransacao("Regime Tributação", adesao.Plano.IND_REGIME_TRIBUTACAO),
                        new ItemTransacao("Percentual Contribuição", adesao.Contrib.VAL_CONTRIBUICAO.ToString()),
                    };

                    foreach (var dep in adesao.Dependentes)
                    {
                        listaConteudo.Add(new ItemTransacao("Nome", dep.NOM_DEPENDENTE));
                        listaConteudo.Add(new ItemTransacao("CPF", dep.COD_CPF));
                        listaConteudo.Add(new ItemTransacao("Grau Parentesco", dep.DES_GRAU_PARENTESCO));
                        listaConteudo.Add(new ItemTransacao("Percentual Rateio", dep.COD_PERC_RATEIO.ToString()));
                        listaConteudo.Add(new ItemTransacao("Sexo", dep.DES_SEXO));
                        listaConteudo.Add(new ItemTransacao("Data de Nascimento", dep.DTA_NASCIMENTO.ToString()));
                        listaConteudo.Add(new ItemTransacao("Pensão", dep.IND_PENSAO));
                    }

                    var conteudo = ProtocoloHelper.MontarConteudo(listaConteudo);

                    var funcionalidade = new FuncionalidadeProxy().BuscarPorChave(18);
                    var protocolo = ProtocoloHelper.Criar(funcionalidade.OID_FUNCIONALIDADE, "01", adesao.COD_EMPRESA, adesao.Plano.COD_PLANO, adesao.COD_MATRICULA, null, conteudo,
                                                          adesao.NOM_PESSOA, null, adesao.IPV4, adesao.IPV4, adesao.IPV6, "", "PORTAL");

                    transaction.Complete();

                    var emailConfig = AppSettings.Get().Email;
                    var texto = $"Prezado {adesao.NOM_PESSOA},<br/>" +
                        $"Obrigado por solicitar a adesão ao plano {adesao.Plano.DES_PLANO}!<br/>" +
                        $"Sua solicitação está em análise e em breve será processada.<br/>" +
                        $"Atenciosamente," +
                        $"Equipe Regius";
                    EnvioEmail.Enviar(emailConfig, adesao.COD_EMAIL, "REGIUS - Adesão On Line", texto);

                    return Ok(protocolo);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        /// <rota caminho="[action]" tipo="POST" />
        /// <parametros>
        ///     <parametro nome="adesao" tipo="AdesaoEntidade" />
        /// </parametros>
        /// <retorno tipo="string" />
        [HttpPost("[action]")]
        public IActionResult Efetivar()
        {
            try
            {


                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Rotas de Validação

        /// <rota caminho="[action]/{cpf}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="cpf" tipo="string" />
        /// </parametros>
        /// <retorno tipo="string" />
        [HttpGet("[action]/{cpf}")]
        public IActionResult ValidarCPF(string cpf)
        {
            try
            {
                if (!Validador.ValidarCPF(cpf))
                    return BadRequest("CPF Inválido.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]/{nascimento}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="nascimento" tipo="string" />
        /// </parametros>
        /// <retorno tipo="string" />
        [HttpGet("[action]/{nascimento}")]
        public IActionResult ValidarDataNascimento(string nascimento)
        {
            try
            {
                var dtNascimento = DateTime.ParseExact(nascimento, "dd.MM.yyyy", new CultureInfo("pt-BR"));

                if (dtNascimento.Date > DateTime.Now)
                    return BadRequest("Data de nascimento não pode ser maior que a data atual.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <rota caminho="[action]/{email}" tipo="GET" />
        /// <parametros>
        ///     <parametro nome="email" tipo="string" />
        /// </parametros>
        /// <retorno tipo="string" />
        [HttpGet("[action]/{email}")]
        public IActionResult ValidarEmail(string email)
        {
            try
            {
                if (!Validador.ValidarEmail(email))
                    return BadRequest("Email Inválido.");

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }

    public class FileUploadViewModel
    {
        public IFormFile File { get; set; }
        public string source { get; set; }
        public string Nome { get; set; }
        public long Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Extension { get; set; }
    }
}