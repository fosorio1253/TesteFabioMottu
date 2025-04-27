namespace Vrumm.Application.Plans.Queries.GetPlans;
public class GetPlansQuery
{
    public int? MinDays { get; set; }
    public int? MaxDays { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreationDate";
    public bool SortDescending { get; set; } = true;
}