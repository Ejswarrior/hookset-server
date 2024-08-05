namespace hookset_server.QueryBuilders
{
    public class WhereQueries
    {
        public string sqlName { get; set; }
        public string paramName { get; set; }
    }
    public class SelectQuery
    {
        public string? _tableName = "";
        public string? _selectValues = "";
        public string? whereValue;
    }

    public class SelectQueryBuilder
    {
        private SelectQuery _selectQuery = new SelectQuery();

        public SelectQueryBuilder addTableName(string tableName)
        {
            _selectQuery._tableName = tableName;
            return this;
        }

        public SelectQueryBuilder addSelectValues(string[]? selectValues)
        {
            if (selectValues != null)
            {
                var selectValuesString = "";
                for (int i = 0; i < selectValues.Length; i++)
                {
                    if (i == selectValues.Length - 1) selectValuesString += $"{selectValues[i]} ";
                    else selectValuesString += $"{selectValues[i]}, ";
                }
                _selectQuery._selectValues += selectValuesString;
            }
            else _selectQuery._selectValues += "*";
            return this;
        }

        public SelectQueryBuilder getWhereValues(List<WhereQueries> whereValues)
        {
            var whereString = "";
            for (int i = 0; i < whereValues.Count; i++)
            {
                if (i == 0) whereString += $"{whereValues[i].sqlName} = @{whereValues[i].paramName}";
                else whereString += $" AND {whereValues[i].sqlName} = @{whereValues[i].paramName}";
            }

            _selectQuery.whereValue = whereString;
            return this;
        }
        public string buildSelectQuery()
        {
            if (_selectQuery._selectValues == null || _selectQuery._tableName == null) return "";
            var baseSelectQuery = $"SELECT {_selectQuery._selectValues} FROM {_selectQuery._tableName}";

            if (_selectQuery.whereValue == null) return baseSelectQuery + ";";

            return baseSelectQuery += $" WHERE {_selectQuery.whereValue};";
        }
    }
}
