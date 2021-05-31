using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Core.Abstractions.Data;
using EvenTransit.Core.Entities;
using MongoDB.Driver;

namespace EvenTransit.Data.Repositories
{
    public class EventsMongoRepository : BaseMongoRepository<Event>, IEventsRepository
    {
        public async Task<List<Event>> GetEventsAsync()
        {
            var result = await Collection.FindAsync(_ => true);
            return await result.ToListAsync();
        }

        public async Task<Event> GetEventAsync(Expression<Func<Event, bool>> predicate)
        {
            var result = await Collection.FindAsync(predicate);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<List<Service>> GetServicesByEventAsync(string eventName, string serviceName)
        {
            var result = await Collection.FindAsync(x => x.Name == eventName);
            var @event = await result.FirstOrDefaultAsync();
            
            return @event.Services.Where(x => x.Name == serviceName).ToList();
        }

        public async Task AddServiceToEvent(string eventId, Service serviceData)
        {
            // TODO Refactor
            var @event = await Collection.Find(x => x._id == eventId).FirstOrDefaultAsync();
            @event.Services.Add(serviceData);
            
            await Collection.ReplaceOneAsync(x => x._id == eventId, @event);
        }

        public async Task UpdateServiceOnEvent(string eventId, Service serviceData)
        {
            // TODO Refactor
            var @event = await Collection.Find(x => x._id == eventId).FirstOrDefaultAsync();
            var filter = Builders<Event>.Filter.Eq(x => x._id, eventId)
                         & Builders<Event>.Filter.ElemMatch(x => x.Services, Builders<Service>.Filter.Eq(x => x.Name, serviceData.Name));

            var serviceIndex = @event.Services.FindIndex(x => x.Name == serviceData.Name);
            @event.Services[serviceIndex] = serviceData;
            
            await Collection.ReplaceOneAsync(filter, @event);
        }
    }
}