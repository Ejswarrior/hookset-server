using hookset_server.models;

namespace hookset_server.QueryBuilders
{
    public class WhereQueries
    {
        public string sqlName { get; set; }
        public string paramName { get; set; }
    }
    public class SelectQuery
    {
        public string? _tableName;
        public string? _selectValues;
        public string? whereValue;
        public string? leftJoin;
        public string? leftJoinValues;
    }

    public class SelectQueryBuilder
    {
        private SelectQuery _selectQuery = new SelectQuery();

        public SelectQueryBuilder addTableName(string tableName)
        {
            _selectQuery._tableName = tableName;
            return this;
        }

        public SelectQueryBuilder addSelectValues(string[]? selectValues, bool? selectCount)
        {
            if (selectValues != null)
            {
                var selectValuesString = "";
                for (int i = 0; i < selectValues.Length; i++)
                {
                    if (i == selectValues.Length - 1) selectValuesString += $"{selectValues[i]} ";
                    else selectValuesString += $"{selectValues[i]}, ";
                }
                _selectQuery._selectValues += selectCount != null ? $"COUNT({selectValuesString})" : selectValuesString;
            }
            else _selectQuery._selectValues += selectCount != null ? $"COUNT(*)" : "*";
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
        public SelectQueryBuilder addLeftJoin(string leftTableName, string tableJoinId, string equalValue)
        {

            _selectQuery.leftJoin = $"LEFT JOIN {leftTableName} ON {leftTableName}.{tableJoinId} = {equalValue}";
            return this;
        }
        public SelectQueryBuilder addLeftJoinValues(string[] leftJoinValues, string tableValueName)
        {
            var joinString = "";
            for (int i = 0; i < leftJoinValues.Length; i++)
            {
                if (i == leftJoinValues.Length - 1) joinString += $" {tableValueName}.{leftJoinValues[i]}";
                else joinString += $" {tableValueName}.{leftJoinValues[i]},";
            }
            _selectQuery.leftJoinValues = joinString;
            return this;
        }



        public string buildSelectQuery()
        {
            if (_selectQuery._tableName == null) return "";
            var selectValues = _selectQuery._selectValues != null ? _selectQuery._selectValues : "*";
            var baseSelectQuery = $"SELECT {selectValues} FROM";
            Console.WriteLine(baseSelectQuery);

            if (_selectQuery.leftJoinValues != null) baseSelectQuery += $" {_selectQuery._tableName},{_selectQuery.leftJoinValues} ";
            if (_selectQuery.leftJoin != null) baseSelectQuery += _selectQuery.leftJoin;

            if (_selectQuery.whereValue == null) return baseSelectQuery + $" {_selectQuery._tableName};";

            if (_selectQuery.leftJoin == null) return baseSelectQuery += $" {_selectQuery._tableName} WHERE {_selectQuery.whereValue};";
            else return baseSelectQuery += $" WHERE {_selectQuery.whereValue};";
        }
    }
}
