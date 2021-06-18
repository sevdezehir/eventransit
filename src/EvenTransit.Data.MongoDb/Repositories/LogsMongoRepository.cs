using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using EvenTransit.Domain.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories
{
    public class LogsMongoRepository : BaseMongoRepository<Logs>, ILogsRepository
    {
        public LogsMongoRepository(IOptions<MongoDbSettings> mongoDbSettings) : base(mongoDbSettings)
        {
        }

        public async Task InsertLogAsync(Logs model)
        {
            model.Id = Guid.NewGuid();
            model.CreatedOn = DateTime.UtcNow;

            await Collection.InsertOneAsync(model);
        }

        public async Task<LogFilter> GetLogsAsync(Expression<Func<Logs, bool>> predicate, int page)
        {
            const int perPage = 100;

            var count = await Collection.Find(predicate).CountDocumentsAsync();
            var totalPages = (int) Math.Ceiling((double) count / perPage);
            var result = await Collection.Find(predicate)
                .Sort(Builders<Logs>.Sort.Ascending(x => x.CreatedOn))
                .Skip((page - 1) * perPage)
                .Limit(perPage)
                .ToListAsync();

            return new LogFilter
            {
                Items = result,
                TotalPages = totalPages
            };
        }

        public async Task<Logs> GetByIdAsync(Guid id)
        {
            return await Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<long> GetLogsCountAsync(DateTime startDate, DateTime endDate, LogType type)
        {
            return await Collection.CountDocumentsAsync(x =>
                x.CreatedOn >= startDate && x.CreatedOn <= endDate && x.LogType == type);
        }

        public long GetLogsCount(DateTime startDate, DateTime endDate, LogType type)
        {
            return Collection.CountDocuments(x =>
                x.CreatedOn >= startDate && x.CreatedOn <= endDate && x.LogType == type);
        }
    }
}