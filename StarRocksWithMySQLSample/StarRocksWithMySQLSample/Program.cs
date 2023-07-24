using Dapper;
using MySqlConnector;

var connStr = "server=127.0.0.1;user id=catcherwong;password=1234567890;port=9030;ssl mode=none";

var sql = @"select os, count(*) as count
from uba.detail 
where event_time >= '2023-07-01' and event_time < '2023-07-02' 
group by os";

using (var conn = new MySqlConnection(connStr))
{
    await conn.OpenAsync();
    var res = await conn.QueryAsync<OsResult>(sql);

    foreach (var item in res)
    {
        Console.WriteLine($"{item.OsName}, {item.Count}");
    }
}

Console.WriteLine("done");


public class OsResult
{ 
    public int Os { get; set; }

    public int Count { get; set; }

    public string OsName => Os == 1 ? "android" : "ios";
}