using System;
using System.IO;
using Common;
using Google.Protobuf;
using Network;
using Pb;
using UnityEngine;

namespace Service
{
    public class UserService:Singleton<UserService>,IDisposable
    {
        public UserService()
        {
          
        }

        public void Init()
        {
            MessageDistributor<Session>.Instance.Subscribe<UserLoginResponse>(OnUserLogin);
            MessageDistributor<Session>.Instance.Subscribe<UserRegisterResponse>(OnUserRegister);
        }

        public void Dispose()
        {
            MessageDistributor<Session>.Instance.Unsubscribe<UserLoginResponse>(OnUserLogin);
            MessageDistributor<Session>.Instance.Unsubscribe<UserRegisterResponse>(OnUserRegister);
        }
        

        public void UserLogin(string userName, string password)
        {
            Message message = new Message()
            {
                Request =new()
                {
                    UserLogin =new()
                    {
                        UserName = userName,
                        Password = password,
                    }
                }
            };

            NetworkManager.Instance.Session.Send(message);
        }
        
        private void OnUserRegister(Session sender, UserRegisterResponse message)
        {
            Debug.Log("UserRegister:"+message.Result.Success);
            EventHelper.BroadCastEvent(message);
        }

        public void UserRegister(string userName, string password)
        {
            Message message = new Message()
            {
                Request =new()
                {
                    UserRegister = new()
                    {
                        UserName = userName,
                        Password = password,
                    }
                }
            };
            NetworkManager.Instance.Session.Send(message);
        }
        
        private void OnUserLogin(Session sender, UserLoginResponse message)
        {
            Debug.Log("UserLogin:"+message.Result.Success);
            EventHelper.BroadCastEvent(message);
        }


    }
}