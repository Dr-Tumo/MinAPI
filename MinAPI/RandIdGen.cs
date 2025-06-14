namespace MinAPI;

public static class RandIdGen
{
    public static string GenerateId()
    {
        return Guid.NewGuid().ToString("N")[..8];
    }
}