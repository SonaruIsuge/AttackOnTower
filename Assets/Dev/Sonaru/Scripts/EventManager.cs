using System;
using System.Collections.Generic;
using System.Linq;

namespace Dev.Sonaru
{
    public static class EventManager
    {
        private static Dictionary<Type, List<Delegate>> actionDictionary;

        public static void Register<T>(Action<T> callback) where T : CustomEvent
        {
            var type = typeof(T);
            actionDictionary ??= new Dictionary<Type, List<Delegate>>();

            if (!actionDictionary.ContainsKey(type))
            {
                actionDictionary.Add(type, new List<Delegate>());
            }

            if (!actionDictionary[type].Contains(callback))
            {
                actionDictionary[type].Add(callback);
            }
        }


        public static void Unregister<T>(Action<T> callback) where T : CustomEvent
        {
            if(actionDictionary == null) 
                return;

            var type = typeof(T);
            if(!actionDictionary.ContainsKey(type))
                return;

            if (actionDictionary[type].Contains(callback))
            {
                actionDictionary[type].Remove(callback);
            }
        }


        public static void RaiseEvent<T>(T args) where T : CustomEvent
        {
            if(actionDictionary == null) 
                return;

            var type = typeof(T);
            if (actionDictionary.ContainsKey(type))
            {
                var actions = actionDictionary[type];
                foreach (var action in actions.Cast<Action<T>>().ToList())
                {
                    action(args);
                }
            }
        }


        public static void Clear()
        {
            if(actionDictionary == null)
                return;
            
            actionDictionary.Clear();
            actionDictionary = null;
        }
    }
}