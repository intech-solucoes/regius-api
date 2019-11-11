#region Usings
using Intech.Lib.Email;
using Intech.Lib.Util.Seguranca;
using Intech.Lib.Util.Validacoes;
using Intech.Lib.Web;
using Intech.PrevSystem.Entidades;
using Intech.PrevSystem.Negocio.Proxy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        public IActionResult BuscarFuncionario(string cdEmpresa, string numMatricula, string cpf, DateTime dataNascimento)
        {
            try
            {
                cpf = cpf.LimparMascara();
                numMatricula = numMatricula.PadLeft(9, '0');

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

                var funcionarioNP = new FuncionarioNPProxy().BuscarPorFundacaoEmpresaMatriculaCpfDataNascimento("01", cdEmpresa, numMatricula, cpf, dataNascimento);

                if (funcionarioNP == null)
                    return Ok("null");

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

                    transaction.Complete();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}