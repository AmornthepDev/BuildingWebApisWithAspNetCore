using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.DTO;
using MyBGList.Models;
using System.Linq.Dynamic.Core;

namespace MyBGList.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BoardGamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BoardGamesController> _logger;

        public BoardGamesController(ApplicationDbContext context, ILogger<BoardGamesController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet(Name = "GetBoardGames")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        public async Task<RestDTO<BoardGame[]>> Get(int pageIndex = 0,
                                                    int pageSize = 10,
                                                    string? sortColumn = "Name",
                                                    string? sortOrder = "ASC",
                                                    string? filterQuery = null)
        {
            var query = _context.BoardGames.AsQueryable();

            if (!string.IsNullOrEmpty(filterQuery))
                query = query.Where(b => b.Name.StartsWith(filterQuery));

            var recordCount = await query.CountAsync();

            query = query
                .OrderBy($"{sortColumn} {sortOrder}")
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            return new RestDTO<BoardGame[]>()
            {
                Data = await query.ToArrayAsync(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                RecordCount = recordCount,
                Links = new List<LinkDTO>()
                {
                    new LinkDTO(Url.Action(null, "BoardGames", new { pageIndex, pageSize }, Request.Scheme)!, "self", "GET")
                }
            };
        }

        [HttpPost(Name = "UpdateBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<BoardGame?>> Post(BoardGameDTO model)
        {
            var boardgame = await _context.BoardGames
                .Where(b => b.Id == model.Id)
                .FirstOrDefaultAsync();

            if (boardgame != null)
            {
                if (!string.IsNullOrEmpty(model.Name))
                    boardgame.Name = model.Name;

                if (model.Year.HasValue && model.Year.Value > 0)
                    boardgame.Year = model.Year.Value;

                if (model.MinPlayers.HasValue && model.MinPlayers.Value > 0)
                    boardgame.MinPlayers = model.MinPlayers.Value;

                if (model.MaxPlayers.HasValue && model.MaxPlayers.Value > 0)
                    boardgame.MaxPlayers = model.MaxPlayers.Value;

                if (model.PlayTime.HasValue && model.PlayTime.Value > 0)
                    boardgame.PlayTime = model.PlayTime.Value;

                if (model.MinAge.HasValue && model.MinAge.Value > 0)
                    boardgame.MinAge = model.MinAge.Value;

                boardgame.LastModifiedDate = DateTime.Now;
                _context.BoardGames.Update(boardgame);
                await _context.SaveChangesAsync();
            }

            return new RestDTO<BoardGame?>()
            {
                Data = boardgame,
                Links = new List<LinkDTO>()
                { 
                    new LinkDTO(Url.Action(null, "BoardGames", model, Request.Scheme)!, "self", "POST")
                }
            };
        }

        [HttpDelete(Name = "DeleteBoardGame")]
        [ResponseCache(NoStore = true)]
        public async Task<RestDTO<List<BoardGame>>> Delete(string idList)
        {
            var ids = idList.Split(',')
                            .Select(int.Parse);

            var boardgamesToDelete = _context.BoardGames
                .Where(b => ids.Contains(b.Id))
                .ToList();

            if (boardgamesToDelete != null)
            {
                _context.BoardGames.RemoveRange(boardgamesToDelete);            
                await _context.SaveChangesAsync();

            }

            return new RestDTO<List<BoardGame>>()
            {
                Data = boardgamesToDelete != null && boardgamesToDelete.Count > 0 ? boardgamesToDelete.ToList() : null,
                Links = new List<LinkDTO>()
                {
                    new LinkDTO(Url.Action(null, "BoardGames", idList, Request.Scheme)!, "self", "DELETE")
                }
            };
        }
    }
}
