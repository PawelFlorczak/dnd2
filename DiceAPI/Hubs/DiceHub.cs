using Microsoft.AspNetCore.SignalR;
using DiceAPI.Models;
using System.Threading.Tasks;

namespace DiceAPI.Hubs
{
    public class DiceHub : Hub
    {
        public async Task SendRoll(DiceRoll roll)
        {
            // Wyślij do wszystkich podłączonych klientów
            await Clients.All.SendAsync("ReceiveRoll", roll);
        }
    }
}