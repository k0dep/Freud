using Freud;
using NUnit.Framework;
using ProtoBuf;

namespace FreudTest
{
    [TestFixture]
    public class StructureTest
    {
        [Test]
        public void SimpleStructureTest([Values("not zero", "", null)] string stringDatas)
        {
            var instance = new Foo()
            {
                field = 1324,
                Property = stringDatas
            };

            var freud = new FreudManager();

            var bytes = freud.Serialize(instance);

            var instanceReconstruct = freud.Deserialize<Foo>(bytes);

            Is.EqualTo(instanceReconstruct.field).ApplyTo(1324);
            Is.EqualTo(instanceReconstruct.Property).ApplyTo(stringDatas);
        }

        [Test]
        public void NullInstanceTest()
        {

            var freud = new FreudManager();

            var bytes = freud.Serialize<Bar>(null);

            var instanceReconstruct = freud.Deserialize<Bar>(bytes);

            Is.Null.ApplyTo(instanceReconstruct);
        }

        [Test]
        public void NullMemberTest()
        {
            var instance = new FooBarBaz();

            var freud = new FreudManager();

            var bytes = freud.Serialize(instance);

            var instanceReconstruct = freud.Deserialize<Bar>(bytes);

            Is.Null.ApplyTo(instanceReconstruct);
        }

        [Test]
        public void ClassWithStruct([Values("not zero", "", null)] string stringDatas, [Values("not zero", "", null)] string stringDatasNested)
        {
            var instance = new Foo()
            {
                field = 1324,
                Property = stringDatasNested
            };

            var barInst = new Bar()
            {
                field = 1244,
                Property = stringDatas,
                Struct = instance
            };

            var freud = new FreudManager();

            var bytes = freud.Serialize(barInst);

            var instanceReconstruct = freud.Deserialize<Bar>(bytes);

            Is.EqualTo(instanceReconstruct.Struct).ApplyTo(instance);

            Is.EqualTo(instanceReconstruct.field).ApplyTo(1244);
            Is.EqualTo(instanceReconstruct.Property).ApplyTo(stringDatas);
        }

        [Test]
        public void AbstractClassMemberThrow()
        {
            var freud = new FreudManager();
            Assert.Throws<FreudTypeCheckException>(() => freud.Serialize(new FooBar()));
            Assert.Throws<FreudTypeCheckException>(() => freud.Deserialize<FooBar>(new byte[0]));
        }

        [Test]
        public void InterfaceMemberThrow()
        {
            var freud = new FreudManager();
            Assert.Throws<FreudTypeCheckException>(() => freud.Serialize(new FooBaz()));
            Assert.Throws<FreudTypeCheckException>(() => freud.Deserialize<FooBaz>(new byte[0]));
        }
    }



    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public struct Foo : IFoo
    {
        public int field;
        public string Property { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class Bar : Baz
    {
        public int field;
        public string Property { get; set; }
        public Foo Struct;
    }

    public class FooBarBaz
    {
        public Bar bar;
    }

    public class FooBar
    {
        public Baz throws;
    }

    public abstract class Baz
    {
    }

    public interface IFoo
    {
    }

    public class FooBaz
    {
        public IFoo throws;
    }
}
