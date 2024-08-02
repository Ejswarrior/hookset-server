namespace hookset_server
{
    public class QueryBuilder
    {
        private string tableName = "";

        public  QueryBuilder(string _tableName) {
            _tableName = tableName;
        }

        public string Select ()
        {
            return $"SELECT * FROM {tableName}";
        }
    }
}
