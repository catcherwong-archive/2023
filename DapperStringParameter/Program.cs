using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DapperStringParameter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var appkey = "test";

            // nvarchar(4000)
            Raw(new { appkey = appkey });

            // varchar(50)
            Raw(new
            {
                appkey = new DbString
                {
                    Value = appkey,
                    IsAnsi = true,
                    Length = 50,
                }

                // IsAnsi = true  => varchar char
                // IsAnsi = false => nvarchar nchar
                // IsFixedLength = true => char nchar
                // IsFixedLength = false => varchar nvarchar
            });

            // varchar(30)
            DynamicParameters parameters = new();
            parameters.Add("appkey", appkey, DbType.AnsiString, ParameterDirection.Input, 30);
            Dp(parameters);

            // char(30)
            DynamicParameters parameters3 = new();
            parameters3.Add("appkey", appkey, DbType.AnsiStringFixedLength, ParameterDirection.Input, 30);
            Dp(parameters3);

            // nvarchar(30)
            DynamicParameters parameters4 = new();
            parameters4.Add("appkey", appkey, DbType.String, ParameterDirection.Input, 30);
            Dp(parameters4);

            // nchar(30)
            DynamicParameters parameters5 = new();
            parameters5.Add("appkey", appkey, DbType.StringFixedLength, ParameterDirection.Input, 30);
            Dp(parameters5);
        }

        static void Raw(object obj)
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            var list = conn.Query<string>(sql: sql, param: obj, commandTimeout: 3);
            Console.WriteLine(list.Count());
        }

        static void Dp(DynamicParameters dp)
        {
            using SqlConnection conn = new(connStr);
            conn.Open();
            var list = conn.Query<string>(sql: sql, param: dp, commandTimeout: 3);
            Console.WriteLine(list.Count());
        }

        static readonly string connStr = "server=127.0.0.1;Initial Catalog=test;uid=sa;pwd=123456;Pooling=true;Max Pool Size=512;Min Pool Size=0;Encrypt=false;TrustServerCertificate=false;";
        static readonly string sql = "SELECT top 10 Id FROM t_test t1 WITH(NOLOCK) WHERE appkey = @appkey";
    }
}
