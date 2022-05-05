using DB.Data;

namespace DB.Repository;

public class SpotifyRepository : ISpotifyRepository
{
    private readonly SpotifyContext _ctx;

    public SpotifyRepository(SpotifyContext ctx)
    {
        _ctx = ctx;
    }
}