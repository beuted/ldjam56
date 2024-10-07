using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DictionaryVector2iConverter<U> : JsonConverter
{
  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    var dictionary = (Dictionary<Vector2i, U>)value;

    writer.WriteStartObject();

    foreach (KeyValuePair<Vector2i, U> pair in dictionary)
    {
      writer.WritePropertyName(pair.Key.X + "," + pair.Key.Y);
      serializer.Serialize(writer, pair.Value);
    }

    writer.WriteEndObject();
  }

  public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
  {
    var result = new Dictionary<Vector2i, U>();
    var jObject = JObject.Load(reader);

    foreach (var x in jObject)
    {
      var parts = x.Key.Split(',');
      Vector2i key = new Vector2i(int.Parse(parts[0]), int.Parse(parts[1]));
      U value = (U)x.Value.ToObject(typeof(U));
      result.Add(key, value);
    }

    return result;
  }

  public override bool CanRead
  {
    get { return true; }
  }

  public override bool CanConvert(Type objectType)
  {
    return typeof(Dictionary<Vector2i, U>).IsAssignableFrom(objectType);
  }
}