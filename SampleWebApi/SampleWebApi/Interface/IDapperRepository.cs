using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleWebApi.Interface
{
    public interface IDapperRepository
    {        
        int Execute(string command, object param = null, int? commandTimeout = null, CommandType commandtype = CommandType.Text);

        TEntity FirstOrDefault<TEntity>(string command, object param = null, int? commandTimeout = null);
               
        IEnumerable<TEntity> Query<TEntity>(string command, object param = null, int? commandTimeout = null);
    }
}
