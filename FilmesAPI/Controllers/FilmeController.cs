using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
/*
 é o mesmo que usar o [Route("Filme")], pois ele já pega o nome que está
 no controler. Caso mude o nome do controler, não precisaria alterar o
 route.
*/
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context; // tenho acesso as tabelas do banco de dados para fazer operações
    private IMapper _mapper; // vai fazer o mapeamento/conversão entre o objeto que eu recebo e o que eu envio, ou vice-versa

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaFilme([FromBody] CreateFilmeDto filmeDto)
    {
        /*
          o _mapper transforma um CreateFilmeDto em um Filme
         
           É o mesmo que fazer dessa forma:

           var filme = new Filme
            {
                Titulo = filmeDto.Titulo,
                Genero = filmeDto.Genero,
                Duracao = filmeDto.Duracao,
            };

        só que com o mapper é menos trabalhoso no de uma tabela com vários campos
         */

        Filme filme = _mapper.Map<Filme>(filmeDto); 

        _context.Filmes.Add(filme);
        _context.SaveChanges(); // qualquer alteração no banco deve-se salvar a mudança

       return CreatedAtAction(nameof(RecuperaFilmePorId), new {id = filme.Id}, filme);  // em requisições POST sempre tem que retornar o objeto criado
    }


    /* 
       IEnumerable é a mesma coisa que List, só que IEnumerable é a
       interface, então fica mais fácil caso seja alterado futuramente
       para outro tipo de coleção.

       [FromQuery] informa que a informação está vindo da query "filme?skip=10&take=5" 
       informada pelo usuário.

       O método Skip() indica quantos elementos da lista pular, enquanto o
       Take() define quantos serão selecionados.
     */

    /// <summary>
    /// Lista todos os filmes
    /// </summary>
    /// <param name="skip">Quantidade de itens que serão pulados</param>
    /// <param name="take">Quantidade de itens que serão selecionados</param>
    /// <response code="200">Caso a listagem seja feita com sucesso</response>
    [ProducesResponseType(typeof(List<ReadFilmeDto>), StatusCodes.Status200OK)]
    [HttpGet]
    public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take).ToList());
    }

    /* seria a rota "Filme/id" */

    /// <summary>
    /// Busca um filme
    /// </summary>
    /// <param name="id">Identificador do filme</param>
    /// <response code="200">Caso a busca seja feita com sucesso</response>
    /// <returns>IActionResult</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);

        if (filme == null)
            return NotFound();

        var filmeDto = _mapper.Map<Filme>(filme);

        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza um filme
    /// </summary>
    /// <param name="id">Identificador do filme</param>
    /// <param name="filmeDto">Objeto com os campos que serão alterados</param>
    /// <response code="204">Sem conteúdo</response>
    /// <returns>IActionResult</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);

        if(filme == null)
            return NotFound();

        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();

        return NoContent();
    }

    /*
      o patch enviado na requisição é deste formato:
     [
        {
            "op": "replace",
            "path": "/titulo",
            "value": "Cinderela"
        }
     ]

    onde "op" é a operação, "path" é o campo a ser alterado e "value" o valor que será atribuído.
    Não é necessário enviar o objeto completo, como no PUT. Apenas os campos que serão atualizados.
     */

    /// <summary>
    /// Atualiza um filme
    /// </summary>
    /// <param name="id">Identificador do filme</param>
    /// <param name="patch">Array de objeto com os campos que serão alterados</param>
    /// <response code="204">Sem conteúdo</response>
    /// <returns>IActionResult</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id, JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);

        if (filme == null)
            return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }

        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// Deleta um filme
    /// </summary>
    /// <param name="id">Identificador do filme</param>
    /// <response code="204">Sem conteúdo</response>
    /// <returns>IActionResult</returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(
            filme => filme.Id == id);

        if (filme == null)
            return NotFound();
        
        _context.Remove(filme);
        _context.SaveChanges();
        
        return NoContent();
    }
}
