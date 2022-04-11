//WARNING: DON'T EDIT THIS FILE!!!
using Common;
using Pb;

namespace Network
{
    public class MessageDispatch<T> : Singleton<MessageDispatch<T>>
    {
        public void Dispatch(T sender, Response message)
        {
            if (message.UserRegister != null) { MessageDistributor<T>.Instance.RaiseEvent(sender, message.UserRegister); }
            if (message.UserLogin != null) { MessageDistributor<T>.Instance.RaiseEvent(sender, message.UserLogin); }
        }

        public void Dispatch(T sender, Request message)
        {
            if (message.UserRegister != null) { MessageDistributor<T>.Instance.RaiseEvent(sender, message.UserRegister); }
            if (message.UserLogin != null) { MessageDistributor<T>.Instance.RaiseEvent(sender, message.UserLogin); }
        }
    }
}
