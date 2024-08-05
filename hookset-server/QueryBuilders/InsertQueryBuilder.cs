namespace hookset_server.QueryBuilders
{

    public class InsertQuery
    {
        public string? _tableName;
        public string? _columnNames;
        public string? _paramNames;
    }
    public class InsertQueryBuilder
    {
        public InsertQuery _insertQuery = new InsertQuery();

        public InsertQueryBuilder addTableName(string tableName)
        {
            _insertQuery._tableName = tableName;
            return this;
        }

        public InsertQueryBuilder addColumnNames(string[] columnNames)
        {
            var columnNameString = "(";
            for(var i = 0; i < columnNames.Length; i++)
            {
                if (i == columnNames.Length - 1) columnNameString += $"{columnNames[i]})";
                else columnNameString += $"{columnNames[i]},";
            }

            _insertQuery._columnNames = columnNameString;
            return this;
        }

        public InsertQueryBuilder addParamNames(string[] paramNames)
        {
            var paramNamesString = "(";
            for (var i = 0; i < paramNames.Length; i++)
            {
                if (i == paramNames.Length - 1) paramNamesString += $" @{paramNames[i]} )";
                else paramNamesString += $" @{paramNames[i]},";
            }

            _insertQuery._paramNames = paramNamesString;
            return this;
        }


        public string buildInsertQuery(bool? returnWithId)
        {
            var baseInsertQuery = $"INSERT INTO {_insertQuery._tableName} {_insertQuery._columnNames} VALUES {_insertQuery._paramNames}";

            if (returnWithId != null && returnWithId == true) return baseInsertQuery += " SELECT SCOPE_IDENTITY();";

            return baseInsertQuery += ";";
        }
    }
}
