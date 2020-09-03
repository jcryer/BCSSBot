using BCSSBot.API.Models;
using System;
using BCSSBot.Database.Models;

namespace BCSSBot
{
    public class CallbackHolder
    {
        public Action<ulong, Permission[]> Callback;

        public CallbackHolder(Action<ulong, Permission[]> callback)
        {
            Callback = callback;
        }
    }
}
