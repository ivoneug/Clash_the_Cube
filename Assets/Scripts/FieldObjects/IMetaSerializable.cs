namespace ClashTheCube
{
    public interface IMetaSerializable
    {
        void MetaSave(MetaSnapshot snapshot);
        MetaSnapshot MetaLoad(string table, int identifier);
        void MetaRemove(string table, int identifier);
    }
}