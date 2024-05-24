using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

public static class Data
{
    public class Player
    {
        public int gold = 100;
        public int gems = 10;
        public int wood = 1000;
        public int stone = 1000;
        public int food = 500;
        public List<Building> buildings = new List<Building>();
        public int goldProduction = 0;
        public int stoneProduction = 0;
        public int woodProduction = 0;
        public int foodProduction = 0;
    }

    public class Building
    {
        public string id = "";
        public long databaseID = 0;
        public int level = 0;
        public int x = 0;
        public int y = 0;
    }

    public static string Serialize<T>(this T target)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();
        xml.Serialize(writer, target);
        return writer.ToString();
    }

    public static T Deserialize<T>(this string target)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(target);
        return (T)xml.Deserialize(reader);
    }
}
