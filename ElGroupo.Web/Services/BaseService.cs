using ElGroupo.Domain;
using ElGroupo.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Services
{
    public abstract class BaseService
    {
        internal ElGroupoDbContext _dbContext = null;
        public BaseService(ElGroupoDbContext ctx)
        {
            _dbContext = ctx;
        }
        public User GetActiveUser(long id)
        {
            return _dbContext.Users.FirstOrDefault(x => x.Id == id);
        }
    }
}
