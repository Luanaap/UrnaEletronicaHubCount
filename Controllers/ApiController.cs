using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UrnaEletronica.Data;
using UrnaEletronica.Models;
using static UrnaEletronica.Models.ApiModels;
using static UrnaEletronica.Models.CandidatoViewModels;
using static UrnaEletronica.Models.VotoViewModels;


namespace UrnaEletronica.Controllers
{
    public class ApiController : Controller
    {
        private readonly MainContext _context;
        public ApiController(MainContext context) { _context = context; }

       // requisitos obrigátorios 
        [HttpPost]
        [Route("api/candidate")]
        public async Task<IActionResult> AdicionarCandidato()
        {
            try
            {
                
                string json = await new StreamReader(Request.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<CandidatoViewModel>(json);

                //verifica se existem registros iguais 
                if (_context.Candidatos.ToList().FindAll(p => p.Legenda == data.Legenda).Count != 0)
                {
                    return Json("Erro: Já existe um candidato com essa legenda.");
                }

                
                if (data.Legenda.ToString() == "0" || data.Legenda.ToString() == "00" || data.Legenda.ToString().Length != 2)
                {
                    return Json("Erro: Formato incorreto para a legenda.");
                }
                else if (data.NomeCompleto == null || data.NomeCompleto.Length < 3)
                {
                    return Json("Erro: Formato incorreto para o nome do candidato.");
                }
                else if (data.Vice == null || data.Vice.Length < 3)
                {
                    return Json("Erro: Formato incorreto para o nome do vice.");
                }

                
                var nCand = new Candidato
                {
                    Inscricao = DateTime.Now,
                    Legenda = data.Legenda,
                    NomeCompleto = data.NomeCompleto,
                    Vice = data.Vice
                };
                _context.Candidatos.Add(nCand);
                _context.SaveChanges();

                return Json("Sucesso: Candidato inserido com sucesso.");
            }
            catch (Exception e)
            {
                return Json("Erro: " + e.Message); ;
            }
        }

        [HttpDelete()]
        [Route("api/candidate")]
        public async Task<IActionResult> RemoverCandidato()
        {
            try
            {
                
                string json = await new StreamReader(Request.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<CandidatoViewModel>(json);

                
                var cand = _context.Candidatos.First(p => p.Legenda == data.Legenda);
                _context.Candidatos.Remove(cand);
                _context.SaveChanges();

                return Json("Sucesso: Candidato removido com sucesso.");
            }
            catch (Exception e)
            {
                return Json("Erro: " + e.Message); ;
            }
        }

        [HttpPost]
        [Route("api/vote")]
        public async Task<IActionResult> EnviarVoto()
        {
            try
            {
                //Converte o retorno do json 
                string json = await new StreamReader(Request.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<VotoViewModel>(json);

                //armazena infos
                var nVoto = new Voto
                {
                    Candidato = data.Candidato,
                    Data = DateTime.Now,
                };
                _context.Votos.Add(nVoto);
                _context.SaveChanges();

                
                if (data.Candidato == 000)
                {
                    var nCandidato = new Candidato
                    {
                        Legenda = 000 
                    };
                    return Json(nCandidato);
                }
                else
                {
                    var obj = _context.Candidatos.First(p => p.Legenda == data.Candidato);
                    return Json(obj);
                }

            }
            catch (Exception e)
            {
                return Json("Erro: " + e.Message); ;
            }
        }

        [HttpGet]
        [Route("api/votes")]
        public IActionResult VerVotos()
        {
            try
            {
                //cria uma lista que armazena todos os dados até o front
                var Resultados = new List<ResultadosViewModel>();
                var Votos = _context.Votos.ToList(); 

                foreach (var item in _context.Candidatos) //um para cada candidato
                {
                    var nResultado = new ResultadosViewModel
                    {
                        Legenda = item.Legenda,
                        Candidato = item.NomeCompleto,
                        Vice = item.Vice,
                        Votos = Votos.FindAll(p => p.Candidato == item.Legenda).Count 

                    };
                    Resultados.Add(nResultado);
                }

                //calcula todos os votos em branco
                var nResultadoBranco = new ResultadosViewModel
                {
                    Legenda = 000,
                    Candidato = "VOTOS EM BRANCO",
                    Vice = "",
                    Votos = Votos.FindAll(p => p.Candidato == 000).Count
                };
                Resultados.Add(nResultadoBranco);

                return Json(Resultados.OrderByDescending(p => p.Votos));
            }
            catch (Exception e)
            {
                return Json("Erro: " + e.Message); ;
            }
        }

        // alguns adicionais
        [HttpGet]
        [Route("api/getcand/{data}")] //busca informaçoes do candidato em questão
        public JsonResult BuscarCandidato(int data)
        {
            try
            {
                
                if (_context.Candidatos.ToList().FindAll(p => p.Legenda == data).Count != 0)
                {
                    var obj = _context.Candidatos.First(p => p.Legenda == data);
                    return Json(obj); 
                }
                else
                {
                    var n = new Candidato
                    {
                        ID = 1,
                        Inscricao = DateTime.Now,
                        Legenda = 000,
                        NomeCompleto = "Fulano de Tal",
                        Vice = "Ciclano de Tal"
                    };
                    return Json(n); 
                }
            }
            catch (Exception e)
            {
                return Json("Erro: " + e.Message); ;
            }
        }

        [HttpGet]
        [Route("api/getcands")] 
        public IActionResult BuscarCandidatos()
        {
            try
            {
                var obj = _context.Candidatos.ToList();
                return Json(obj);
            }
            catch (Exception e)
            {
                return Json("Erro: " + e.Message); ;
            }
        }

        [HttpGet]
        [Route("api/resetwhite")] 
        public IActionResult ResetarBrancos()
        {
            try
            {
                _context.Votos.RemoveRange(_context.Votos.ToList().FindAll(p => p.Candidato == 0));
                _context.SaveChanges();

                return Json("Sucesso: Todos os votos em branco foram removidos.");
            }
            catch (Exception e)
            {
                return Json("Erro: " + e.Message); ;
            }
        }


    }
}
