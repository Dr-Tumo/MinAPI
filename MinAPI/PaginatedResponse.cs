namespace MinAPI;

public record PaginatedResponse<T>(List<T> Items, int Page);