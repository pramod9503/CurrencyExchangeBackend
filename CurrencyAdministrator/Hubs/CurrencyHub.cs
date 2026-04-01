using CurrencyRepo.Abstracts;
using Microsoft.AspNetCore.SignalR;
using CurrencyRepo.Models.BackModels;

namespace CurrencyAdministrator.Hubs
{
    public class CurrencyHub : Hub<ICurrencyClient>
    {
        public CurrencyHub() { }

        public async Task NotifyUpdate(CurrencyMessageModel messageModel) 
        {
            await Clients.All.ReceiveUpdate(messageModel);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
