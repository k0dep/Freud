using Freud;
using NUnit.Framework;
using ProtoBuf;
using System.IO;

namespace FreudTest
{
    [TestFixture]
    public class TimingShowerTest
    {
        FreudManager reflManager = new FreudManager(new ReflectionTypeInfoFactory());
        FreudManager exprManager = new FreudManager(new ExpressionTypeInfoFactory());

        [Test]
        public void TimesReflectionSerealize([Values(0)]int count)
        {
            var barInst = new TimesFoo()
            {
                field = 1244,
                Prop = "kdjshosudh",
                a = 1.2f,
                _propBar = new Bar()
                {
                    field = 12343,
                    Property = "asdsafas",
                    Struct = new Foo()
                    {
                        field = 02342,
                        Property = "sds",
                    }
                }
            };

            for (int i = 0; i < count; i++)
            {
                reflManager.Serialize(barInst);
            }
        }

        [Test]
        public void TimesExpressionSerealize([Values(5000000)]int count)
        {
            var barInst = new TimesFoo()
            {
                field = 1244,
                Prop = "kdjshosudh",
                a = 1.2f,
                _propBar = new Bar()
                {
                    field = 12343,
                    Property = "asdsafas",
                    Struct = new Foo()
                    {
                        field = 02342,
                        Property = "sds",
                    }
                }
            };

            for (int i = 0; i < count; i++)
            {
                exprManager.Serialize(barInst);
            }
        }

        [Test]
        public void TimesProtoSerealize([Values(5000000)]int count)
        {
            var barInst = new TimesFoo()
            {
                field = 1244,
                Prop = "kdjshosudh",
                a = 1.2f,
                _propBar = new Bar()
                {
                    field = 12343,
                    Property = "asdsafas",
                    Struct = new Foo()
                    {
                        field = 02342,
                        Property = "sds",
                    }
                }
            };

            for (int i = 0; i < count; i++)
            {
                using (var s = new MemoryStream())
                {
                    Serializer.Serialize(s, barInst);
                    var b = s.GetBuffer();
                }
            }
        }

        [Test]
        public void TimesCount([Values(5000000)]int count)
        {
            for (int i = 0; i < count; )
            {
                i++;
            }
        }

        
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class TimesFoo
    {
        public int field;
        public float a;
        public string Prop { get; set; }
        public Bar _propBar;
    }
}