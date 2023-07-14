namespace Application.Helpers.Runtime;

public static class MathHelpers
{
    public static int GetPaginatedOffset(int pageNumber, int pageSize)
    {
        return (pageNumber - 1) * pageSize;
    }
}