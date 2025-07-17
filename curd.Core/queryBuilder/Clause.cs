namespace curd.Core.queryBuilder
{
    public class Clause
    {
        private static readonly string[] validOperatorTypes = [
            "WHERE",
            "ORDER BY",
            "AND",
            "NOT",
            "OR"
            ];

        public string operatorType;
        public string condition;
        public Clause(string _operatorType, string _condition) 
        {
            if (!validOperatorTypes.Contains(_operatorType.ToUpper()))
            {
                throw new ArgumentException($"Operator type '{_operatorType}' not supported.");
            }
            this.operatorType = _operatorType;
            this.condition = _condition;
        } 
    }
}
