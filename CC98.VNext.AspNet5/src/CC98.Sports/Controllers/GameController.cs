using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Sakura.AspNet;
using Sakura.AspNet.Mvc.Messages;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CC98.Sports.Controllers
{
	using System.Security.Claims;
	using Models;

	/// <summary>
	/// 提供对比赛信息的访问。
	/// </summary>
	public class GameController : Controller
	{

		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private SportDataModel DbContext { get; }

		/// <summary>
		/// 设置服务对象。
		/// </summary>
		private AppSettingService<SystemSetting> SettingService { get; }

		/// <summary>
		/// 消息对象。
		/// </summary>
		private ICollection<OperationMessage> Messages { get; }

		public GameController(SportDataModel dbContext, AppSettingService<SystemSetting> settingService, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			SettingService = settingService;
			Messages = messageAccessor.Messages;
		}

		/// <summary>
		/// 释放该对象占用的所有资源。
		/// </summary>
		~GameController()
		{
			DbContext.Dispose();
		}

		/// <summary>
		/// 比赛信息主页。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Route("~/Game/List/{eventId?}")]
		public async Task<IActionResult> Index(int? eventId = null, int page = 1)
		{

			var items = from i in DbContext.Games
							.Include(p => p.Event)
							.Include(p => p.Team1)
							.Include(p => p.Team2)
						select i;

			if (eventId != null)
			{
				var eventItem = await (from i in DbContext.Events
									   where i.Id == eventId
									   select i).SingleOrDefaultAsync();

				if (eventItem == null)
				{
					throw new ActionResultException(400, "赛事编号无效。");
				}

				ViewBag.Event = eventItem;

				items = from i in items
						where i.EventId == eventId
						select i;

			}

			var orderedItems = from i in items
							   orderby i.State ascending, i.StartTime ascending
							   select i;

			return View(orderedItems.ToPagedList(SettingService.Current.PageSize, page));
		}

		/// <summary>
		/// 查看某场比赛的详细信息。
		/// </summary>
		/// <param name="id">比赛标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public async Task<IActionResult> Detail(int id)
		{
			var item = await (from i in DbContext.Games
									.Include(p => p.Event)
							  where i.Id == id
							  select i).FirstOrDefaultAsync();

			if (item == null)
			{
				throw new ActionResultException(404);
			}

			return View(item);
		}

		/// <summary>
		/// 删除赛程。
		/// </summary>
		/// <param name="id">要删除的赛程的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> Delete(int id)
		{
			var item = await (from i in DbContext.Games
							  where i.Id == id
							  select i).SingleOrDefaultAsync();

			if (item == null)
			{
				throw new ActionResultException(404);
			}

			// 删除所有赛事
			DbContext.Games.Remove(item);

			// 日志
			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.DeleteGame,
				RelatedEventId = item.EventId,
				Remark = item.Name,
				CC98Id = User.GetUserName(),
				Time = DateTime.Now
			});

			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "指定的赛程已经被删除。");
			}
			catch (DbUpdateException ex)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", $"删除赛程时发生错误。详细信息：{ex.Message}");
			}

			// 回到详细信息页面。
			return RedirectToAction("Detail", "Event", new { id = item.EventId });
		}

		/// <summary>
		/// 查找指定比赛关联的赛事信息。
		/// </summary>
		/// <param name="game">比赛对象。</param>
		/// <returns>赛事信息。</returns>
		private async Task<Event> FindEventByGameAsync(Game game)
		{
			var eventId = game.EventId;

			return await (from i in DbContext.Events
								.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
						  where i.Id == eventId
						  select i).SingleOrDefaultAsync();
		}

		/// <summary>
		/// 显示创建赛事页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> Create(int eventId)
		{
			var eventItem = await (from i in DbContext.Events
									.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
								   where i.Id == eventId
								   select i).FirstOrDefaultAsync();

			if (eventItem == null)
			{
				throw new ActionResultException(400, "给定的赛事编号无效。");
			}

			ViewBag.Event = eventItem;
			return View();
		}

		/// <summary>
		/// 执行创建操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> Create(Game model)
		{
			CheckModelState(model);

			if (ModelState.IsValid)
			{
				DbContext.Games.Add(model);
				try
				{
					await DbContext.SaveChangesAsync();
					Messages.Add(OperationMessageLevel.Success, "操作成功。", "已经成功创建了新的比赛。");
					return RedirectToAction("Index", "Game", new { eventId = model.Id });
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			ViewBag.Event = await FindEventByGameAsync(model);
			return View(model);
		}

		/// <summary>
		/// 显示编辑页面。
		/// </summary>
		/// <param name="id">要编辑的比赛的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> Edit(int id)
		{
			var item = await (from i in DbContext.Games
							  where i.Id == id
							  select i).SingleOrDefaultAsync();

			if (item == null)
			{
				return HttpNotFound();
			}

			ViewBag.Event = await FindEventByGameAsync(item);
			return View(item);
		}

		/// <summary>
		/// 执行编辑操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> Edit(Game model)
		{
			CheckModelState(model);

			if (ModelState.IsValid)
			{
				DbContext.Games.Update(model);

				try
				{
					await DbContext.SaveChangesAsync();
					Messages.Add(OperationMessageLevel.Success, "操作成功。", "赛事信息已经成功更新。");
					return RedirectToAction("Index", "Game", new { eventId = model.Id });
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.Message);
				}
			}

			ViewBag.Event = await FindEventByGameAsync(model);
			return View(model);
		}

		/// <summary>
		/// 显示自动生成比赛界面。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> AutoGenerate(int eventId)
		{
			var eventItem = await (from i in DbContext.Events
								   where i.Id == eventId
								   select i).FirstOrDefaultAsync();

			if (eventItem == null)
			{
				return HttpNotFound();
			}

			ViewBag.Event = eventItem;
			return View();
		}

		/// <summary>
		/// 执行自动生成比赛操作。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AutoGenerate(int eventId, AutoGenerateGameViewModel model)
		{
			if (model.GenerateGroupMatches)
			{
				if (string.IsNullOrEmpty(model.GroupMatchTitleFormat))
				{
					ModelState.AddModelError(nameof(model.GroupMatchTitleFormat), "生成小组赛时必须提供小组赛标题格式。");
				}
			}

			if (model.GenerateKnockoutMatches)
			{
				if (string.IsNullOrEmpty(model.KnockoutMatchTitleFormat))
				{
					ModelState.AddModelError(nameof(model.KnockoutMatchTitleFormat), "生成淘汰赛时必须提供淘汰赛标题格式。");
				}

				if (model.KnockoutRoundCount == null)
				{
					ModelState.AddModelError(nameof(model.KnockoutRoundCount), "生成淘汰赛时必须提供淘汰赛轮次。");
				}

				if (model.KnockoutRoundCount < model.LoserKnockoutRountCount)
				{
					ModelState.AddModelError("", "淘汰赛败者组轮次的值不能大于总轮次的值。");
				}
			}

			if (ModelState.IsValid)
			{
				var eventItem = await (from i in DbContext.Events.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
									   where i.Id == eventId
									   select i).FirstOrDefaultAsync();

				if (eventItem == null)
				{
					throw new ActionResultException(404);
				}

				// 淘汰赛开始论数
				var startRound = 0;

				// 生成小组赛
				if (model.GenerateGroupMatches)
				{
					var groupMatches = GenerateGroupGameCore(model, eventItem).ToArray();
					DbContext.Games.AddRange(groupMatches);

					startRound = groupMatches.Max(i => i.Round) + 1;
				}

				// 生成淘汰赛
				if (model.GenerateKnockoutMatches)
				{
					// 淘汰赛开始轮次
					var knockoutMatches = GenerateKnockoutGameCore(model, startRound + 1).ToArray();
					DbContext.Games.AddRange(knockoutMatches);

				}

				await DbContext.SaveChangesAsync();

				// 返回赛事列表页面。
				return RedirectToAction("Detail", "Event", new { id = eventId });
			}

			return View(model);

		}

		#region 自动生成的核心算法部分


		/// <summary>
		/// 表示队伍引用的类型。
		/// </summary>
		private enum TeamRefType
		{
			/// <summary>
			/// 某场赛事的胜者。
			/// </summary>
			Winner,
			/// <summary>
			/// 某场赛事的败者。
			/// </summary>
			Loser
		}

		/// <summary>
		/// 表示淘汰赛中一个参与者的信息。
		/// </summary>
		private class TeamRef
		{
			/// <summary>
			/// 参与者对应的赛事。
			/// </summary>
			public KnockoutGameInfo Game { get; }

			/// <summary>
			/// 指定参与者是 <see cref="Game"/> 所表示的赛事的胜者还是败者。
			/// </summary>
			public TeamRefType Type { get; }

			/// <summary>
			/// 初始化一个对象的新实例。
			/// </summary>
			/// <param name="game"><see cref="Game"/> 属性的值。</param>
			/// <param name="type"><see cref="Type"/> 属性的值。</param>
			public TeamRef(KnockoutGameInfo game, TeamRefType type)
			{
				Game = game;
				Type = type;
			}

			/// <summary>
			/// 从赛事对象中构造队伍引用。
			/// </summary>
			/// <param name="game">参赛队伍对应的赛事。</param>
			/// <param name="type">指定参与者是 <paramref name="game"/> 所表示的赛事的胜者还是败者。</param>
			/// <returns>创建的队伍引用。</returns>
			public static TeamRef FromGame(KnockoutGameInfo game, TeamRefType type)
			{
				return new TeamRef(game, type);
			}
		}

		/// <summary>
		/// 表示一场淘汰赛的预定义信息。该类型仅用于自动生成淘汰赛的过程内部使用。
		/// </summary>
		private class KnockoutGameInfo
		{
			/// <summary>
			/// 赛事编号。
			/// </summary>
			public int Id { get; set; }

			/// <summary>
			/// 赛事轮次。
			/// </summary>
			public int Round { get; set; }

			/// <summary>
			/// 赛事等级。
			/// </summary>
			public int Level { get; set; }

			/// <summary>
			/// 赛事的第一支队伍。
			/// </summary>
			public TeamRef Team1 { get; set; }

			/// <summary>
			/// 赛事的第二支队伍。
			/// </summary>
			public TeamRef Team2 { get; set; }

			/// <summary>
			/// 赛事的类型。
			/// </summary>
			public KnockoutGameType Type { get; set; }
		}

		/// <summary>
		/// 表示淘汰赛相关的生成上下文。
		/// </summary>
		private class KnockOutRoundContext
		{
			/// <summary>
			/// 胜者组队伍。
			/// </summary>
			public IReadOnlyList<TeamRef> WinnerTeams { get; set; }

			/// <summary>
			/// 败者组队伍。
			/// </summary>
			public IReadOnlyList<TeamRef> LoserTeams { get; set; }

			/// <summary>
			/// 比赛轮次。
			/// </summary>
			public int Round { get; set; }

			/// <summary>
			/// 赛事等级。
			/// </summary>
			public int Level { get; set; }

			/// <summary>
			/// 比赛列表。
			/// </summary>
			public IEnumerable<KnockoutGameInfo> Games { get; set; }
		}

		/// <summary>
		/// 根据上一轮比赛信息生成胜者组比赛。
		/// </summary>
		/// <param name="context">上一轮比赛信息。</param>
		/// <return>本轮比赛信息。</return>
		private static KnockOutRoundContext GenerateWinRound(KnockOutRoundContext context)
		{
			if (context.WinnerTeams.Count % 2 != 0)
			{
				throw new ArgumentException("胜者组队伍数目必须是偶数。");
			}

			// 胜者组比赛
			var winnerGames = new List<KnockoutGameInfo>();

			for (var i = 0; i < context.WinnerTeams.Count / 2; i++)
			{
				winnerGames.Add(new KnockoutGameInfo
				{
					Team1 = context.WinnerTeams[2 * i],
					Team2 = context.WinnerTeams[2 * i + 1],
					Round = context.Round,
					Level = context.Level,
					Type = KnockoutGameType.None,
				});
			}

			return new KnockOutRoundContext
			{
				Games = winnerGames,
				WinnerTeams = SelectTeams(winnerGames, TeamRefType.Winner),
				LoserTeams = SelectTeams(winnerGames, TeamRefType.Loser),
				Round = context.Round + 1,
			};
		}

		/// <summary>
		/// 选择一系列比赛的胜者或者败者。
		/// </summary>
		/// <param name="games">要选择的比赛。</param>
		/// <param name="type">控制选择胜者或者败者。</param>
		/// <returns>胜者或者败者队伍的集合。</returns>
		private static IReadOnlyList<TeamRef> SelectTeams(IEnumerable<KnockoutGameInfo> games, TeamRefType type)
		{
			return games.Select(i => new TeamRef(i, type)).ToArray();
		}

		/// <summary>
		/// 根据上一轮比赛信息生成新一轮比赛的相关数据。
		/// </summary>
		/// <param name="context">上一轮产生的信息。</param>
		/// <returns>本轮产生的信息。</returns>
		private static KnockOutRoundContext GenerateWinLoseRound(KnockOutRoundContext context)
		{
			if (context.WinnerTeams.Count != context.LoserTeams.Count)
			{
				throw new ArgumentException("胜者组和败者组队伍数目必须相等");
			}

			if (context.WinnerTeams.Count % 2 != 0)
			{
				throw new ArgumentException("胜者组和败者组队伍数目必须是偶数。");
			}

			// 胜者组比赛
			var winnerGames = new List<KnockoutGameInfo>();

			for (var i = 0; i < context.WinnerTeams.Count / 2; i++)
			{
				winnerGames.Add(new KnockoutGameInfo
				{
					Team1 = context.WinnerTeams[2 * i],
					Team2 = context.WinnerTeams[2 * i + 1],
					Round = context.Round,
					Level = context.Level,
					Type = KnockoutGameType.Winner
				});
			}


			// 败者组比赛
			var loserGames = new List<KnockoutGameInfo>();

			for (var i = 0; i < context.LoserTeams.Count / 2; i++)
			{
				loserGames.Add(new KnockoutGameInfo
				{
					Team1 = context.LoserTeams[2 * i],
					Team2 = context.LoserTeams[2 * i + 1],
					Round = context.Round,
					Level = context.Level,
					Type = KnockoutGameType.Loser
				});

			}

			// 本轮胜者组败者和上一轮败者组胜者对战
			var winnerLoserGames = new List<KnockoutGameInfo>();

			for (var i = 0; i < winnerGames.Count; i++)
			{
				winnerLoserGames.Add(new KnockoutGameInfo
				{
					Team1 = TeamRef.FromGame(winnerGames[i], TeamRefType.Loser),
					Team2 = TeamRef.FromGame(loserGames[i], TeamRefType.Winner),
					Round = context.Round + 1,
					Level = context.Level,
					Type = KnockoutGameType.WinnerLoser
				});
			}

			return new KnockOutRoundContext
			{
				Games = winnerGames.Concat(loserGames).Concat(winnerLoserGames),
				WinnerTeams = SelectTeams(winnerGames, TeamRefType.Winner),
				LoserTeams = SelectTeams(winnerLoserGames, TeamRefType.Winner),
				Round = context.Round + 2
			};

		}

		/// <summary>
		/// 生成第一轮比赛。
		/// </summary>
		/// <param name="roundNumber">淘汰赛的总轮数。</param>
		/// <param name="round">当前轮次。</param>
		/// <returns>本轮比赛信息。</returns>
		private static KnockOutRoundContext GenerateFirstRound(int roundNumber, int round)
		{
			if (roundNumber <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(roundNumber), roundNumber, "淘汰赛场次必须为正整数。");
			}

			var gameCount = Utility.PowN(2, roundNumber - 1);

			var gameList = new List<KnockoutGameInfo>();

			for (var i = 0; i < gameCount; i++)
			{
				gameList.Add(new KnockoutGameInfo
				{
					Team1 = null,
					Team2 = null,
					Round = round,
					Level = roundNumber,
					Type = KnockoutGameType.None,
				});
			}

			return new KnockOutRoundContext
			{
				Games = gameList,
				WinnerTeams = SelectTeams(gameList, TeamRefType.Winner),
				LoserTeams = SelectTeams(gameList, TeamRefType.Loser),
				Round = round + 1
			};
		}

		/// <summary>
		/// 生成总决赛对象。
		/// </summary>
		/// <param name="context">上下文信息。</param>
		/// <returns>总决赛比赛对象。</returns>
		private static KnockoutGameInfo GenerateFinal(KnockOutRoundContext context)
		{
			var finalGame = new KnockoutGameInfo
			{
				Team1 = context.WinnerTeams.Single(),
				Team2 = context.LoserTeams.Single(),
				Level = 0,
				Round = context.Round,
				Type = KnockoutGameType.None
			};

			return finalGame;
		}


		/// <summary>
		/// 生成淘汰赛所有场次和对应的队伍关联。
		/// </summary>
		/// <param name="totalRound">淘汰赛的总轮次。</param>
		/// <param name="loserRound">败者组淘汰赛的轮次。</param>
		/// <returns>所有赛事信息的集合。</returns>
		private static IEnumerable<KnockoutGameInfo> GenerateKnockoutGameAndTeamRef(int totalRound, int loserRound)
		{
			var games = new List<KnockoutGameInfo>();

			// 第一轮比赛
			var context = GenerateFirstRound(totalRound, 0);
			games.AddRange(context.Games);

			// 第二轮开始的比赛
			for (var r = 1; r < totalRound; r++)
			{
				// 设置等级
				context.Level = totalRound - r;

				// 根据败者组轮次，生成胜者组比赛或者胜者及败者组比赛
				context = totalRound - loserRound > r ? GenerateWinRound(context) : GenerateWinLoseRound(context);
				games.AddRange(context.Games);
			}

			// 如果有败者组，则应当生成总决赛
			if (loserRound > 0)
			{
				games.Add(GenerateFinal(context));
			}

			// 生成编号
			for (var i = 0; i < games.Count; i++)
			{
				games[i].Id = i + 1;
			}

			return games;
		}

		/// <summary>
		/// 生成淘汰赛的比赛集合。
		/// </summary>
		/// <param name="model">视图模型。</param>
		/// <param name="startRound">开始轮次</param>
		/// <returns>赛事信息的集合。</returns>
		private IEnumerable<Game> GenerateKnockoutGameCore(AutoGenerateGameViewModel model, int startRound)
		{
			var games = GenerateKnockoutGameAndTeamRef(model.KnockoutRoundCount.Value, model.LoserKnockoutRountCount ?? 0);

			foreach (var i in games)
			{
				var context = new KnockoutGameGenerateContext
				{
					GameType = GameType.Knockout,
					GameId = i.Id,
					KnockoutGameType = i.Type,
					KnockoutLevel = i.Level,
					LocalRound = i.Round + 1,
					Team1 = i.Team1,
					Team2 = i.Team2
				};

				yield return new Game
				{
					EventId = model.EventId,
					Name = GenerateTitle(context, model.KnockoutMatchTitleFormat),
					Team1Id = null,
					Team2Id = null,
					Round = i.Round + startRound,
				};
			}
		}

		/// <summary>
		/// 生成小组赛列表的核心方法。
		/// </summary>
		/// <param name="model">设置信息。</param>
		/// <param name="eventItem">赛事信息。</param>
		/// <returns>小组赛列表的集合。</returns>
		private IEnumerable<Game> GenerateGroupGameCore(AutoGenerateGameViewModel model, Event eventItem)
		{
			var eventId = model.EventId;

			var groupedTeams = from i in eventItem.TeamRegistrations
							   where i.AuditState == AuditState.Accepted
							   where !string.IsNullOrEmpty(i.Group)
							   orderby i.GroupNumber ascending
							   group i by i.Group;

			foreach (var group in groupedTeams)
			{
				// 当前小组的队伍数组
				var teamsInGroup = group.ToArray();

				var teamCount = teamsInGroup.Length;

				// 如果是奇数队伍，则具有轮空。
				var hasBye = teamCount % 2 != 0;

				// 构造对战队列。注意由于分组必须是偶数，轮空时必须进行额外处理
				var pairs = GeneratePairs(hasBye ? teamCount + 1 : teamCount);

				// 轮编号
				var roundNum = 0;

				foreach (var round in pairs)
				{
					roundNum++;

					foreach (var pair in round)
					{
						var realPair = SwitchIfNecessary(pair, teamCount);

						// 轮空标记，如果任意一队占据轮空编号则直接略过。
						if (hasBye && (realPair.Item1 == teamCount || realPair.Item2 == teamCount))
						{
							continue;
						}

						var context = new GroupGameGenerateContext
						{
							GameType = GameType.Group,
							LocalRound = roundNum,
							TotalRound = roundNum,
							Team1 = teamsInGroup[realPair.Item1],
							Team2 = teamsInGroup[realPair.Item2],
							Group = group.Key
						};

						yield return new Game
						{
							Name = GenerateTitle(context, model.GroupMatchTitleFormat),
							Team1Id = teamsInGroup[realPair.Item1].TeamId,
							Team2Id = teamsInGroup[realPair.Item2].TeamId,
							Round = roundNum,
							EventId = eventId
						};
					}
				}
			}
		}

		private static Tuple<int, int> SwitchIfNecessary(Tuple<int, int> teams, int count)
		{
			if (DetermineSwitch(teams.Item1, teams.Item2, count))
			{
				return teams;
			}
			else
			{
				return Tuple.Create(teams.Item2, teams.Item1);
			}
		}

		/// <summary>
		/// 在一个分组中决定队伍的主客顺序。
		/// </summary>
		/// <param name="team1">队伍 1 的编号。从 0 开始。</param>
		/// <param name="team2">队伍 2 的编号。从 0 开始。</param>
		/// <param name="count">队伍的总数。</param>
		/// <returns>如果可以按照原样排序，返回 true；如果需要交换排序，返回 false。</returns>
		private static bool DetermineSwitch(int team1, int team2, int count)
		{
			if (team1 < (count + 1) / 2)
			{
				return team2 > team1 && team2 - team1 <= count / 2;
			}
			else
			{
				return team2 > team1 || team1 - team2 > count / 2;
			}
		}

		/// <summary>
		/// 生成对一个有序集合进行两两对象分组的组合结果。
		/// </summary>
		/// <param name="count">集合的元素个数。</param>
		/// <returns>表示结果的双重集合。每个基本元素是代表两个配对元素索引位置的 <see cref="Tuple{int, int}"/> 类型的对象。</returns>
		private static IReadOnlyList<IReadOnlyList<Tuple<int, int>>> GeneratePairs(int count)
		{
			if (count <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), count, "对象数量必须为正数。");
			}

			if (count % 2 != 0)
			{
				throw new ArgumentException("组队生成功能要求数据总数必须是偶数。", nameof(count));
			}

			var existItems = new HashSet<Tuple<int, int>>();

			var result = new List<List<Tuple<int, int>>>();

			for (var round = 0; round < count; round++)
			{
				// 当前轮的结果集合
				var resultForCurrentRound = new List<Tuple<int, int>>(count / 2);

				// 当前集合的临时排序结果。
				var currentSet = new HashSet<int>(Enumerable.Range(0, count));

				for (var first = 0; first < count; first++)
				{
					// 如果当前集合已经不存在该数，说明这一轮中已经被用掉了。
					if (!currentSet.Contains(first))
					{
						continue;
					}

					// 否则，尝试为当前编号匹配另一个编号，从小开始
					for (var second = 0; second < count; second++)
					{
						// 下一个编号必须也没用过
						if (first == second || !currentSet.Contains(second))
						{
							continue;
						}

						// 新的编号组合
						var newItem = Tuple.Create(first, second);

						// 如果已经存在过对应的编号组，则跳过
						if (existItems.Contains(newItem) || existItems.Contains(Tuple.Create(second, first)))
						{
							continue;
						}

						// 找到第一个项目，并且终止集合
						resultForCurrentRound.Add(newItem);
						existItems.Add(newItem);
						currentSet.Remove(first);
						currentSet.Remove(second);
						break;
					}
				}

				result.Add(resultForCurrentRound);
			}

			return result;
		}

		/// <summary>
		/// 定义比赛的类型。
		/// </summary>
		private enum GameType
		{
			/// <summary>
			/// 小组赛。
			/// </summary>
			Group,
			/// <summary>
			/// 淘汰赛。
			/// </summary>
			Knockout
		}

		/// <summary>
		/// 定义淘汰赛的类型。
		/// </summary>
		public enum KnockoutGameType
		{
			/// <summary>
			/// 无。
			/// </summary>
			None,
			/// <summary>
			/// 胜者组比赛。
			/// </summary>
			Winner,
			/// <summary>
			/// 败者组比赛。
			/// </summary>
			Loser,
			/// <summary>
			/// 胜者组对败者组比赛。
			/// </summary>
			WinnerLoser,
		}

		/// <summary>
		/// 表示淘汰赛的生成上下文。
		/// </summary>
		private class KnockoutGameGenerateContext : GameGenerateContext
		{
			/// <summary>
			/// 比赛队伍 1。
			/// </summary>
			public TeamRef Team1 { get; set; }

			/// <summary>
			/// 比赛队伍 2。
			/// </summary>
			public TeamRef Team2 { get; set; }

			/// <summary>
			/// 淘汰赛的编号。
			/// </summary>
			public int GameId { get; set; }

			/// <summary>
			/// 获取或设置淘汰赛的比赛层次。
			/// </summary>
			public int KnockoutLevel { get; set; }

			/// <summary>
			/// 获取或设置淘汰赛类型。
			/// </summary>
			public KnockoutGameType KnockoutGameType { get; set; }

		}

		/// <summary>
		/// 表示小组赛的生成上下文。
		/// </summary>
		private class GroupGameGenerateContext : GameGenerateContext
		{
			/// <summary>
			/// 获取或设置比赛的队伍 1。
			/// </summary>
			public EventTeamRegistration Team1 { get; set; }
			/// <summary>
			/// 获取或设置比赛的队伍 2。
			/// </summary>
			public EventTeamRegistration Team2 { get; set; }

			/// <summary>
			/// 获取或设置分组名称。
			/// </summary>
			public string Group { get; set; }
		}

		/// <summary>
		/// 表示比赛生成的相关上下文。
		/// </summary>
		private abstract class GameGenerateContext
		{
			/// <summary>
			/// 获取或设置当前轮次。
			/// </summary>
			public int LocalRound { get; set; }

			/// <summary>
			/// 获取或设置总轮次。
			/// </summary>
			public int TotalRound { get; set; }

			/// <summary>
			/// 获取或设置比赛类型。
			/// </summary>
			public GameType GameType { get; set; }
		}

		/// <summary>
		/// 生成淘汰赛的标题字符串。
		/// </summary>
		/// <param name="type">淘汰赛类型。</param>
		/// <param name="level">淘汰赛轮次。</param>
		/// <returns>淘汰赛标题字符串内容。</returns>
		private static string GenerateKnockoutTypeString(KnockoutGameType type, int level)
		{
			string root;
			string prefix;

			// 前缀
			switch (type)
			{
				case KnockoutGameType.Winner:
					prefix = "胜者组";
					break;
				case KnockoutGameType.Loser:
					prefix = "败者组";
					break;
				case KnockoutGameType.WinnerLoser:
					prefix = "胜-败者组";
					break;
				default:
					prefix = "";
					break;
			}

			// 后缀
			switch (level)
			{
				case 0:
					root = "总决赛";
					break;
				case 1:
					root = "决赛";
					break;
				case 2:
					root = "半决赛";
					break;
				default:
					root = string.Format(CultureInfo.CurrentCulture, "1/{0:d}决赛", Utility.PowN(2, level - 1));
					break;
			}

			return string.Format(CultureInfo.CurrentCulture, "{0}{1}", prefix, root);
		}

		/// <summary>
		/// 生成队伍占位符的名称。
		/// </summary>
		/// <param name="team">队伍占位符对象。</param>
		/// <returns>队伍名称。</returns>
		private static string GetName(TeamRef team)
		{
			if (team == null)
			{
				return "待定";
			}

			switch (team.Type)
			{
				case TeamRefType.Winner:
					return string.Format(CultureInfo.CurrentCulture, "#{0} 胜者", team.Game.Id);
				case TeamRefType.Loser:
					return string.Format(CultureInfo.CurrentCulture, "#{0} 败者", team.Game.Id);
				default:
					throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// 根据给定的格式，使用上下文信息生成标题。
		/// </summary>
		/// <param name="context">比赛生成上下文对象。</param>
		/// <param name="format">标题格式字符串。</param>
		/// <returns>生成结果。</returns>
		private string GenerateTitle(KnockoutGameGenerateContext context, string format)
		{
			// 类型字符串
			var gameTypeString = GenerateKnockoutTypeString(context.KnockoutGameType, context.KnockoutLevel);

			var args = new object[]
			{
				context.LocalRound,
				context.TotalRound,
				gameTypeString,
				context.GameId,
				GetName(context.Team1),
				GetName(context.Team2),
			};
			return string.Format(format, args);
		}

		/// <summary>
		/// 根据给定的格式，使用上下文信息生成标题。
		/// </summary>
		/// <param name="context">比赛生成上下文对象。</param>
		/// <param name="format">标题格式字符串。</param>
		/// <returns>生成结果。</returns>
		private string GenerateTitle(GroupGameGenerateContext context, string format)
		{
			var args = new object[]
			{
				context.LocalRound,
				context.TotalRound,
				context.Group,
				context.Team1?.GetTeamGroupAndNumber(),
				context.Team2?.GetTeamGroupAndNumber(),
				context.Team1?.Team.Id.ToString(SettingService.Current.TeamIdFormat),
				context.Team2?.Team.Id.ToString(SettingService.Current.TeamIdFormat),
				context.Team1?.Team.Name,
				context.Team2?.Team.Name,
			};
			return string.Format(format, args);
		}

		#endregion

		/// <summary>
		/// 现实批量编辑界面。
		/// </summary>
		/// <param name="eventId">事件标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(UserUtility.OrganizePolicy)]
		public async Task<IActionResult> BatchEdit(int eventId)
		{
			var eventItem = await (from i in DbContext.Events
									.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
									.Include(p => p.Games)
								   where i.Id == eventId
								   select i).FirstOrDefaultAsync();

			if (eventItem == null)
			{
				throw new ActionResultException(404);
			}

			ViewBag.Event = eventItem;

			return View(eventItem.Games.OrderBy(i => i.Round).ToArray());
		}

		/// <summary>
		/// 执行批量编辑操作。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BatchEdit(int eventId, Game[] model)
		{
			var eventItem = await (from i in DbContext.Events
									.Include(p => p.TeamRegistrations).ThenInclude(p => p.Team)
								   where i.Id == eventId
								   select i).FirstOrDefaultAsync();

			if (eventItem == null)
			{
				throw new ActionResultException(404);
			}

			ViewBag.Event = eventItem;

			// 更新数据
			foreach (var item in model)
			{
				item.EventId = eventId;
				DbContext.Update(item);
			}

			// 日志
			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.EventGameBatchEdit,
				CC98Id = User.GetUserName(),
				RelatedEventId = eventId,
				Time = DateTime.Now
			});

			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "赛事的赛程已经批量更新。");

				return RedirectToAction("Detail", "Event", new { id = eventId });
			}
			catch (DbUpdateException ex)
			{
				ModelState.AddModelError("", ex.Message);
			}


			return View(model);

		}


		/// <summary>
		/// 清空赛程。
		/// </summary>
		/// <param name="eventId">赛事标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(UserUtility.OrganizePolicy)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Clear(int eventId)
		{
			var eventItem = await (from i in DbContext.Events.Include(p => p.Games)
								   where i.Id == eventId
								   select i).FirstOrDefaultAsync();

			if (eventItem == null)
			{
				throw new ActionResultException(404);
			}

			// 删除所有赛事
			DbContext.Games.RemoveRange(eventItem.Games);

			// 日志
			DbContext.Logs.Add(new Log
			{
				ActionType = ActionType.ClearEventGames,
				RelatedEventId = eventId,
				CC98Id = User.GetUserName(),
				Time = DateTime.Now
			});

			try
			{
				await DbContext.SaveChangesAsync();
				Messages.Add(OperationMessageLevel.Success, "操作成功。", "赛事的赛程已经清空。");
			}
			catch (DbUpdateException ex)
			{
				Messages.Add(OperationMessageLevel.Error, "操作失败。", $"清空赛程时发生错误，请稍后再试一次。详细信息：{ex.Message}");
			}

			// 回到详细信息页面。
			return RedirectToAction("Detail", "Event", new { id = eventId });
		}

		/// <summary>
		/// 对数据进行额外检查。
		/// </summary>
		/// <param name="model">要检查的数据模型。</param>
		private void CheckModelState(Game model)
		{
			// 删除强引用检查
			ModelState.Remove(nameof(model.Event));

			if (model.StartTime > model.EndTime)
			{
				ModelState.AddModelError("", "比赛结束时间不能位于开始时间之前。");
			}
		}
	}
}

