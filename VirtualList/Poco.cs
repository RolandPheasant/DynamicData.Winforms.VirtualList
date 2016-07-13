namespace VirtualList
{
   public class Poco
    {
        public int Id { get; }
        public int X { get; }
        public int Y { get; }
        public int Z { get; }


        public Poco(int id, int x, int y, int z)
            {
            Id = id;
            X = x;
            Y = y;
            Z = z;
            }


        public override string ToString()
        {
            return $"ID= {Id.ToString().PadLeft(4)}   X={X.ToString().PadLeft(4)}   Y={Y.ToString().PadLeft(3)}   Z={Z.ToString().PadLeft(4)}";
        }
    }
}
