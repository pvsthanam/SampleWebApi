using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using SampleWebApi.Interface;
using System.Data;
using Dapper;

namespace SampleWebApi.CommonService
{
    public class DapperService : IDapperRepository
    {
        private IDbConnection _db = new MySqlConnection(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
        public DapperService()
        {

        }

        public int Execute(string command, object param = null, int? commandTimeout = null, CommandType commandtype = CommandType.Text)
        {
            return _db.Execute(command, param);
        }

        public IEnumerable<TEntity> Query<TEntity>(string command, object param = null, int? commandTimeout = null)
        {
            return _db.Query<TEntity>(command, param);
        }

        public TEntity FirstOrDefault<TEntity>(string command, object param = null, int? commandTimeout = null)
        {
            return _db.QueryFirstOrDefault<TEntity>(command, param);
        }

    }
}