namespace Ants
{
    public interface IDistanceCalculator
    {
        double Distance(Location loc1, Location loc2);
        double SquaredDistance(Location loc1, Location loc2);
        int ManhattenDistance(Location loc1, Location loc2);
    }
}