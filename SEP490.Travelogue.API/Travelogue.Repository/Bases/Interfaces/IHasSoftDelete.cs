namespace Travelogue.Repository.Bases.Interfaces;

public interface IHasSoftDelete
{
    bool IsDeleted { set; get; }
}
