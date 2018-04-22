[![BuildStatus](https://travis-ci.org/k0dep/Freud.svg?branch=master)](https://travis-ci.org/k0dep/Freud) [![Nugetstatus](https://buildstats.info/nuget/Freud?includePreReleases=true)](https://www.nuget.org/packages/Freud)

---

# Freud
 Freud - сериализатор/десериализатор C# объектов в массив байт. Цель freud - предоставить простой инструмент для **быстрой** сериализации и десериализации **не сложных** объектов.

# Возможности
 - Сериализация без подготовки классов(без атрибутов, без регистрации в фреймворке)
 - Эффективность(~1.3x быстрее protobuf-net)
 - Простота
 - Возможность расширения

# Что не умеет
 - Не поддерживает полиморфизм, из этого следует:
   * В классах не должно быть свойств или полей с типом интерфейс/абстрактный класс
 - Dictionary<K, V> (скоро будет)
 - List<T> (скоро будет)

# Пример использования
```csharp
public class Foo
{
    public int field;
    public string Property { get; set; }
}

...

var instance = new Foo()
{
    field = 1324,
    Property = stringDatas
};

var freud = new FreudManager();
var bytes = freud.Serialize(instance);
var instanceReconstruct = freud.Deserialize<Foo>(bytes);
```
