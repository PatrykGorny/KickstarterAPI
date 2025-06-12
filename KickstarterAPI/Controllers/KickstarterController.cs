using AutoMapper;
using Infractructure.EF;
using KickstarterAPI.Dto.Kickstarter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KickstarterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KickstarterController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public KickstarterController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResponse<KickstarterProjectDto>>> GetProjects([FromQuery] KickstarterFilterDto filter,int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("Page number and size must be greater than 0.");

        var query = _context.Kickstarters.AsQueryable();

        if (filter.ID.HasValue)
            query = query.Where(p => p.ID == filter.ID.Value);

        if (!string.IsNullOrEmpty(filter.Name))
            query = query.Where(p => p.Name.Contains(filter.Name));

        if (!string.IsNullOrEmpty(filter.Category))
            query = query.Where(p => p.Category == filter.Category);

        if (!string.IsNullOrEmpty(filter.Subcategory))
            query = query.Where(p => p.Subcategory == filter.Subcategory);

        if (!string.IsNullOrEmpty(filter.Country))
            query = query.Where(p => p.Country == filter.Country);

        if (filter.LaunchedFrom.HasValue)
            query = query.Where(p => p.Launched >= filter.LaunchedFrom.Value);

        if (filter.LaunchedTo.HasValue)
            query = query.Where(p => p.Launched <= filter.LaunchedTo.Value);

        if (filter.DeadlineFrom.HasValue)
            query = query.Where(p => p.Deadline >= filter.DeadlineFrom.Value);

        if (filter.DeadlineTo.HasValue)
            query = query.Where(p => p.Deadline <= filter.DeadlineTo.Value);

        if (filter.GoalMin.HasValue)
            query = query.Where(p => p.Goal >= filter.GoalMin.Value);

        if (filter.GoalMax.HasValue)
            query = query.Where(p => p.Goal <= filter.GoalMax.Value);

        if (filter.PledgedMin.HasValue)
            query = query.Where(p => p.Pledged >= filter.PledgedMin.Value);

        if (filter.PledgedMax.HasValue)
            query = query.Where(p => p.Pledged <= filter.PledgedMax.Value);

        if (filter.BackersMin.HasValue)
            query = query.Where(p => p.Backers >= filter.BackersMin.Value);

        if (filter.BackersMax.HasValue)
            query = query.Where(p => p.Backers <= filter.BackersMax.Value);

        if (!string.IsNullOrEmpty(filter.State))
            query = query.Where(p => p.State == filter.State);

        var totalCount = await query.CountAsync();
        IQueryable<KickstarterEntity> sortedQuery = query;

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            bool ascending = filter.SortDirection?.ToLower() != "desc";
            sortedQuery = filter.SortBy.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "category" => ascending ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category),
                "goal" => ascending ? query.OrderBy(p => p.Goal) : query.OrderByDescending(p => p.Goal),
                "pledged" => ascending ? query.OrderBy(p => p.Pledged) : query.OrderByDescending(p => p.Pledged),
                "backers" => ascending ? query.OrderBy(p => p.Backers) : query.OrderByDescending(p => p.Backers),
                "launched" => ascending ? query.OrderBy(p => p.Launched) : query.OrderByDescending(p => p.Launched),
                "deadline" => ascending ? query.OrderBy(p => p.Deadline) : query.OrderByDescending(p => p.Deadline),
                "state" => ascending ? query.OrderBy(p => p.State) : query.OrderByDescending(p => p.State),
                _ => query.OrderBy(p => p.ID)
            };
        }
        else
        {
            sortedQuery = query.OrderBy(p => p.ID); 
        }

        var projects = await sortedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        var result = new PaginatedResponse<KickstarterProjectDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = _mapper.Map<List<KickstarterProjectDto>>(projects)
        };

        return Ok(result);
    }
   

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<KickstarterProjectDto>> GetProject(long id)
    {
        var project = await _context.Kickstarters.FindAsync(id);
        if (project == null)
            return NotFound();
        
        var result = _mapper.Map<KickstarterProjectDto>(project);
        
        return result;
    }
    
    [HttpPost]
    [Authorize(Policy = "Bearer")]
    public async Task<ActionResult<KickstarterProjectDto>> CreateProject(KickstarterCreateDto dto)
    {
        var project = _mapper.Map<KickstarterEntity>(dto);
        long maxId = await _context.Kickstarters
            .OrderByDescending(p => p.ID)
            .Select(p => p.ID)
            .FirstOrDefaultAsync();

        project.ID = maxId + 1;
        
        _context.Kickstarters.Add(project);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<KickstarterProjectDto>(project);

        return CreatedAtAction(nameof(GetProject), new { id = project.ID }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Bearer")]
    public async Task<IActionResult> UpdateProject(long id, KickstarterCreateDto dto)
    {
        var existing = await _context.Kickstarters.FindAsync(id);
        if (existing == null)
            return NotFound();

        _mapper.Map(dto, existing);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id}")]
    [Authorize(Policy = "Bearer")]
    public async Task<IActionResult> DeleteProject(long id)
    {
        var project = await _context.Kickstarters.FindAsync(id);
        if (project == null)
            return NotFound();

        _context.Kickstarters.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}