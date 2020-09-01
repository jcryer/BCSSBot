using BCSSBot.API.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
