using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Plans.Dtos;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Plans.Queries.GetPlans;
public class GetPlansQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPlansQueryHandler> _logger;

    public GetPlansQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetPlansQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaginatedList<PlanDto>> Handle(GetPlansQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recuperando planos com filtros: MinDays={MinDays}, MaxDays={MaxDays}",
            query.MinDays, query.MaxDays);

        var plansQuery = _unitOfWork.Plans.GetAll();

        plansQuery = ApplyFilters(plansQuery, query.MinDays, query.MaxDays);

        plansQuery = ApplySorting(plansQuery, query.SortBy, query.SortDescending);

        var plans = await plansQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await plansQuery.CountAsync(cancellationToken);

        var dtos = plans.Select(MapToDto).ToList();

        _logger.LogInformation("Recuperados {Count} planos", dtos.Count);

        return new PaginatedList<PlanDto>(dtos, totalCount, query.PageNumber, query.PageSize);
    }

    private IQueryable<Plan> ApplyFilters(IQueryable<Plan> query, int? minDays, int? maxDays)
    {
        if (minDays.HasValue)
            query = query.Where(p => p.DayCount >= minDays.Value);

        if (maxDays.HasValue)
            query = query.Where(p => p.DayCount <= maxDays.Value);

        return query;
    }

    private IQueryable<Plan> ApplySorting(IQueryable<Plan> query, string sortBy, bool descending)
    {
        Expression<Func<Plan, object>> keySelector = sortBy?.ToLower() switch
        {
            "daycount" => p => p.DayCount,
            "dailyrate" => p => p.DailyRate,
            "penaltypercentage" => p => p.PenaltyPercentage,
            "updatedate" => p => p.UpdateDate,
            _ => p => p.CreationDate
        };

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }

    private PlanDto MapToDto(Plan plan)
    {
        return new PlanDto
        {
            Id = plan.Id,
            DayCount = plan.DayCount,
            DailyRate = plan.DailyRate,
            PenaltyPercentage = plan.PenaltyPercentage,
            CreationDate = plan.CreationDate,
            UpdateDate = plan.UpdateDate
        };
    }
}