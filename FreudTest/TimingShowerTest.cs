using Freud;
using NUnit.Framework;
using ProtoBuf;
using System.IO;
using Freud.PrimitiveTypeInfo;

namespace FreudTest
{
    public class TimingShowerTest
    {
        FreudManager exprManager = new FreudManager(new ExpressionTypeInfoFactory());

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


        public void TimesCount([Values(5000000)]int count)
        {
            for (int i = 0; i < count; )
            {
                i++;
            }
        }


        public void TestStringPrimitiveTimes([Values(5000000)] int count)
        {
            var primitive = new StringPrimitiveTypeInfo();
            var memStream = new MemoryStream();

            for (int i = 0; i < count; i++)
            {
                memStream.Seek(0, SeekOrigin.Begin);
                primitive.Serialize("teststring", memStream);
            }

            memStream.Dispose();
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