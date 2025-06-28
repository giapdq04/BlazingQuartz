namespace BlazeQuartz.Jobs.Abstractions
{
    public interface IDataMapValueResolver
    {
        string? Resolve(DataMapValue? dmv);
    }
}