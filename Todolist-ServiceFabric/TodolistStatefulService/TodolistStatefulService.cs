﻿using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;

namespace TodolistStatefulService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TodolistStatefulService : StatefulService, ITodolistStatefulService
    {
        public TodolistStatefulService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task Create(TodoItem todoItem)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, TodoItem>>("TodoListItems");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await myDictionary.AddAsync(tx, todoItem.Id, todoItem);

                // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are
                // discarded, and nothing is saved to the secondary replicas.
                await tx.CommitAsync();
            }
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<TodoItem> GetTodoItem(int id)
        {
            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, TodoItem>>("TodoListItems");
            using (var tx = this.StateManager.CreateTransaction())
            {
                var item = await myDictionary.TryGetValueAsync(tx, id);
                return item.Value;
            }
        }

        public Task<IEnumerable<TodoItem>> GetTodoItems()
        {
            var lijst =  new List<TodoItem>() { new TodoItem() { Id = 1, Titel = "Sjoerd een box geven omdat het werkt" } };

            return Task.FromResult<IEnumerable<TodoItem>>(lijst);
        }

        public Task Update(int id, TodoItem todoItem)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] { new ServiceReplicaListener(context => this.CreateServiceRemotingListener(context)) };
        }

        ///// <summary>
        ///// This is the main entry point for your service replica.
        ///// This method executes when this replica of your service becomes primary and has write status.
        ///// </summary>
        ///// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following sample code with your own logic
        //    //       or remove this RunAsync override if it's not needed in your service.

        //    var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        using (var tx = this.StateManager.CreateTransaction())
        //        {
        //            var result = await myDictionary.TryGetValueAsync(tx, "Counter");

        //            ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
        //                result.HasValue ? result.Value.ToString() : "Value does not exist.");

        //            await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

        //            // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are
        //            // discarded, and nothing is saved to the secondary replicas.
        //            await tx.CommitAsync();
        //        }

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}
    }
}