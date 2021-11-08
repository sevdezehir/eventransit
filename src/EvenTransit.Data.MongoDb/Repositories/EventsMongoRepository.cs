using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EvenTransit.Data.MongoDb.Settings;
using EvenTransit.Domain.Abstractions;
using EvenTransit.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EvenTransit.Data.MongoDb.Repositories
{
    public class EventsMongoRepository : BaseMongoRepository<Event>, IEventsRepository
    {
        public EventsMongoRepository(IOptions<MongoDbSettings> mongoDbSettings, MongoDbConnectionStringBuilder connectionStringBuilder) : base(mongoDbSettings, connectionStringBuilder)
        {
        }
        
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

        public Service GetServiceByEvent(string eventName, string serviceName)
        {
            var @event = Collection.Find(x => x.Name == eventName).FirstOrDefault();
            
            return @event.Services.FirstOrDefault(x => x.Name == serviceName);
        }
        
        public async Task<Service> GetServiceByEventAsync(string eventName, string serviceName)
        {
            var result = await Collection.FindAsync(x => x.Name == eventName);
            var @event = await result.FirstOrDefaultAsync();
            
            return @event.Services.FirstOrDefault(x => x.Name == serviceName);
        }

        public async Task AddServiceToEventAsync(Guid eventId, Service serviceData)
        {
            var @event = await Collection.Find(x => x.Id == eventId).FirstOrDefaultAsync();
            @event.Services.Add(serviceData);
            
            await Collection.ReplaceOneAsync(x => x.Id == eventId, @event);
        }

        public async Task UpdateServiceOnEventAsync(Guid eventId, Service serviceData)
        {
            var @event = await Collection.Find(x => x.Id == eventId).FirstOrDefaultAsync();
            var filter = Builders<Event>.Filter.Eq(x => x.Id, eventId)
                         & Builders<Event>.Filter.ElemMatch(x => x.Services, Builders<Service>.Filter.Eq(x => x.Name, serviceData.Name));

            var serviceIndex = @event.Services.FindIndex(x => x.Name == serviceData.Name);
            @event.Services[serviceIndex] = serviceData;
            
            await Collection.ReplaceOneAsync(filter, @event);
        }

        public async Task AddEvent(Event dataModel)
        {
            dataModel.Id = Guid.NewGuid();
            await Collection.InsertOneAsync(dataModel);
        }

        public async Task DeleteEventAsync(Guid id)
        {
            await Collection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task DeleteServiceAsync(Guid eventId, string serviceName)
        {
            var @event = await Collection.Find(x => x.Id == eventId).FirstOrDefaultAsync();
            var filter = Builders<Event>.Filter.Eq(x => x.Id, eventId)
                         & Builders<Event>.Filter.ElemMatch(x => x.Services, Builders<Service>.Filter.Eq(x => x.Name, serviceName));
            
            var service = @event.Services.FirstOrDefault(x => x.Name == serviceName);
            @event.Services.Remove(service);
            
            await Collection.ReplaceOneAsync(filter, @event);
        }
    }
}