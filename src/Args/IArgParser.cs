
namespace Args
{
    public interface IArgParser
    {
        bool IsValid { get; }
        bool IsMissing(IArgumentInfo arg);
        T GetValue<T>(IArgumentInfo arg);
    }
}
