using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Data;
using FilmesAPI.Models;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class SessaoController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public SessaoController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona uma nova sessão.
    /// </summary>
    /// <param name="dto">Objeto contendo as informações da sessão a ser criada.</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso a inserção seja feita com sucesso.</response>
    [HttpPost]
    public IActionResult AdicionaSessao(CreateSessaoDto dto)
    {
        Sessao sessao = _mapper.Map<Sessao>(dto);
        
        _context.Sessoes.Add(sessao);
        _context.SaveChanges();
       
        return CreatedAtAction(nameof(RecuperaSessoesPorId), new { filmeId = sessao.FilmeId, cinemaId = sessao.CinemaId }, sessao);
    }

    /// <summary>
    /// Lista todas as sessões
    /// </summary>
    /// <response code="200">Caso a listagem seja feita com sucesso.</response>
    [HttpGet]
    public IEnumerable<ReadSessaoDto> RecuperaSessoes()
    {
        return _mapper.Map<List<ReadSessaoDto>>(_context.Sessoes.ToList());
    }

    /// <summary>
    /// Busca uma sessão específica com base no ID do filme e no ID do cinema.
    /// </summary>
    /// <param name="filmeId">Identificador do filme.</param>
    /// <param name="cinemaId">Identificador do cinema.</param>
    /// <response code="200">Caso a busca seja feita com sucesso.</response>
    /// <response code="404">Caso não seja encontrado.</response>
    [HttpGet("{filmeId}/{cinemaId}")]
    public IActionResult RecuperaSessoesPorId(int filmeId, int cinemaId)
    {
        Sessao sessao = _context.Sessoes.FirstOrDefault(sessao => sessao.FilmeId == filmeId && sessao.CinemaId == cinemaId);
      
        if (sessao != null)
        {
            ReadSessaoDto sessaoDto = _mapper.Map<ReadSessaoDto>(sessao);

            return Ok(sessaoDto);
        }
        
        return NotFound();
    }
}