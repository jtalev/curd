using curd.Core.queryBuilder;
using curd.Core.queryParser;

namespace curd.Core.Test
{
    public class QueryBuilerTest
    {
        [Theory]
        [MemberData(nameof(GetCreateQueryParams))]
        public void BuildCreateQuery(string tableName, Value[] values, string resultQuery)
        {
            Assert.Equal(resultQuery, QueryBuilder.BuildCreateQuery(tableName, values));
        }

        public static TheoryData<string, Value[], string> GetCreateQueryParams()
        {
            return new()
            {
                {
                    "stores",
                    new Value[]
                    {
                        new Value("business_name", "'Haymes Geelong West'"),
                    }
                    , "INSERT INTO stores (business_name) VALUES ('Haymes Geelong West');"
                },
                {
                    "note",
                    new Value[]
                    {
                        new Value("uuid", "'lkjbsdlkj23kljbslk2332'"),
                        new Value("job_id", "3"),
                        new Value("created_at", "'06/07/2025'")
                    },
                    "INSERT INTO note (uuid, job_id, created_at) VALUES ('lkjbsdlkj23kljbslk2332', 3, '06/07/2025');"
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetReadQueryParams))]
        public void BuildReadQuery(string tableName, string[]? columnNames, Clause[]? clauses, string resultQuery)
        {
            Assert.Equal(resultQuery, QueryBuilder.BuildReadQuery(tableName, columnNames, clauses));
        }

        public static TheoryData<string, string[]?, Clause[]?, string> GetReadQueryParams()
        {
            return new()
            {
                {
                    "note",
                    ["uuid", "job_id"],
                    new Clause[]
                    {
                        new Clause("WHERE", "uuid = '123'")
                    },
                    "SELECT uuid, job_id FROM note WHERE uuid = '123';"
                },
                {
                    "note",
                    ["uuid", "job_id"],
                    null,
                    "SELECT uuid, job_id FROM note;"
                },
                {
                    "note",
                    null,
                    null,
                    "SELECT * FROM note;"
                },
                {
                    "note",
                    null,
                    new Clause[]
                    {
                        new Clause("WHERE", "uuid = '123'"),
                        new Clause("AND", "job_id = 1"),
                        new Clause("ORDER BY", "job_id")
                    },
                    "SELECT * FROM note WHERE uuid = '123' AND job_id = 1 ORDER BY job_id;"
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetDeleteQueryParams))]
        public void BuildDeleteQuery(string tableName, Clause[]? clauses, string resultQuery)
        {
            Assert.Equal(resultQuery, QueryBuilder.BuildDeleteQuery(tableName, clauses));
        }

        public static TheoryData<string, Clause[]?, string> GetDeleteQueryParams()
        {
            return new()
            {
                {
                    "note",
                    new Clause[]
                    {
                        new Clause("WHERE", "job_id = 1"),
                    },
                    "DELETE FROM note WHERE job_id = 1;"
                },
                {
                    "note",
                    null,
                    "DELETE FROM note;"
                },
                {
                    "note",
                    new Clause[]
                    {
                        new Clause("WHERE", "job_id = 1"),
                        new Clause("OR", "uuid = '123'")
                    },
                    "DELETE FROM note WHERE job_id = 1 OR uuid = '123';"
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetUpdateQueryParams))]
        public void BuildUpdateQuery(string tableName, Value[] values, Clause[] clauses, string resultQuery)
        {
            Assert.Equal(resultQuery, QueryBuilder.BuildUpdateQuery(tableName, values, clauses));
        }

        public static TheoryData<string, Value[], Clause[], string> GetUpdateQueryParams()
        {
            return new()
            {
                {
                    "note",
                    new Value[]
                    {
                        new Value("uuid", "'7654321'"),
                    },
                    new Clause[]
                    {
                        new Clause("WHERE", "job_id = 1"),
                    },
                    "UPDATE note SET uuid = '7654321' WHERE job_id = 1;"
                },
                {
                    "note",
                    new Value[]
                    {
                        new Value("job_id", "4"),
                        new Value("uuid", "'123212'"),
                    },
                    new Clause[]
                    {
                        new Clause("WHERE", "job_id = 1"),
                    },
                    "UPDATE note SET job_id = 4, uuid = '123212' WHERE job_id = 1;"
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetBuildQueryData))]
        public void Parse(QueryIR input, string result)
        {
            string query = QueryBuilder.BuildQuery(input);

            Assert.Equal(query, result);
        }

        public static TheoryData<QueryIR, string> GetBuildQueryData()
        {
            return new()
            {
                {
                    new QueryIR
                    (
                        "read",
                        "stores",
                        new List<string>
                        {

                        },
                        new List<Value>
                        {

                        },
                        new List<Clause>
                        {
                            new Clause( "WHERE", "uuid = '123'")
                        }
                    ),
                    "SELECT * FROM stores WHERE uuid = '123';"
                },
                {
                    new QueryIR
                    (
                        "create",
                        "stores",
                        new List<string>
                        {

                        },
                        new List<Value>
                        {
                            new Value( "uuid", "'123'" ),
                            new Value( "store", "'Haymes Geelong West'" ),
                        },
                        new List<Clause>
                        {

                        }
                    ),
                    "INSERT INTO stores (uuid, store) VALUES ('123', 'Haymes Geelong West');"
                },
                {
                    new QueryIR
                    (
                        "read",
                        "stores",
                        new List<string>
                        {
                            "uuid",
                            "store"
                        },
                        new List<Value>
                        {

                        },
                        new List<Clause>
                        {
                            new Clause( "WHERE", "uuid = '123'")
                        }
                    ),
                    "SELECT uuid, store FROM stores WHERE uuid = '123';"
                },
                {
                    new QueryIR
                    (
                        "read",
                        "stores",
                        new List<string>
                        {
                            "uuid",
                            "store"
                        },
                        new List<Value>
                        {

                        },
                        new List<Clause>
                        {
                            new Clause( "WHERE", "uuid = '123'"),
                            new Clause( "AND", "uuid = '321'")
                        }
                    ),
                    "SELECT uuid, store FROM stores WHERE uuid = '123' AND uuid = '321';"
                }
            };
        }
    }
}
