
    public class EventHelper
    {
        public static void BroadCastEvent<T>(T message) 
        {
            string type = typeof(T).Name;
            FairyGUI.GRoot.inst.BroadcastEvent(type, message);
        }
    }
