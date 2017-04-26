namespace Ants
{
    public class AntLoc : Location
    {
        public AntLoc(int row, int col, int team)
            : base(col, row)
        {
            Team = team;
        }

        public int Team { get; private set; }


        public Location ToLoc()
        {
            return new Location(Col, Row);
        }
    }
}