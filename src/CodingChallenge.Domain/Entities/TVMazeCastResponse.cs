using System.Collections.Generic;

namespace CodingChallenge.Domain.Entities;
// Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
public class TVMazeCastDataResponse
{
    public TVMazeCastDataResponse()
    {

    }
    public List<TVMazeCastItem> CastList { get; set; } = new List<TVMazeCastItem>();
    public bool RateLimited { get; set; }
    public bool IsSuccessful { get; set; }
    public bool AlreadyStored { get; set; }
    public bool NotFound { get; set; }
}

public class Character
{
    public Character()
    {
        image = new Image();
        _links = new Links();
    }
    public int id { get; set; }
    public string url { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public Image image { get; set; }
    public Links _links { get; set; }
}

public class Country
{
    public Country()
    {

    }
    public string name { get; set; } = string.Empty;
    public string code { get; set; } = string.Empty;
    public string timezone { get; set; } = string.Empty;
}

public class Image
{
    public Image()
    {

    }
    public string medium { get; set; } = string.Empty;
    public string original { get; set; } = string.Empty;
}

public class Links
{
    public Links()
    {
        self = new Self();
    }
    public Self self { get; set; }
}

public class Person
{
    public Person()
    {
        country = new Country();
        image = new Image();
        _links = new Links();
    }
    public int id { get; set; }
    public string url { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public Country country { get; set; }
    public string birthday { get; set; } = string.Empty;
    public string deathday { get; set; } = string.Empty;
    public string gender { get; set; } = string.Empty;
    public Image image { get; set; }
    public int updated { get; set; }
    public Links _links { get; set; }
}

public class TVMazeCastItem
{
    public TVMazeCastItem()
    {
        person = new Person();
        character = new Character();
    }
    public Person person { get; set; }
    public Character character { get; set; }
    public bool self { get; set; }
    public bool voice { get; set; }
}

public class Self
{
    public Self()
    {

    }
    public string href { get; set; } = string.Empty;
}



