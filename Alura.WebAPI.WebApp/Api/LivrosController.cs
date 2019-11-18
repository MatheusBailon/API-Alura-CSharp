using System.Linq;
using Alura.ListaLeitura.Persistencia;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alura.ListaLeitura.WebApp.Api
{
    public class LivrosController : Controller
    {
        private readonly IRepository<Livro> _repo;

        public LivrosController(IRepository<Livro> repository)
        {
            _repo = repository;
        }


        //Recupera dados de um determinado livro (a partir do seu id)
        [HttpGet]
        public IActionResult Recupera(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound(); //Code 404
            }
            return Json(model.ToModel());
        }

        [HttpPost]
        //[FromBody] --> Indica onde deverá realizar o Bind
        public IActionResult Incluir([FromBody] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recupera", new { id = livro.Id });
                return Created(uri, livro);
            }
            return BadRequest(); //Code 400
        }

        public IActionResult Alterar([FromBody] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if(model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok(); //Code 200
            }
            return BadRequest(); //Code 404
        }

        [HttpPost]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if(model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return NoContent(); //Code 204
        }
    }
}