using System;
using NUnit.Framework;
using Freud;

namespace FreudTest
{
    [TestFixture]
    public class SimpleTypesFlow
    {
        [Test]
        public void TestBool()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize(true);
            Is.EqualTo(freud.Deserialize<byte>(bytes)).ApplyTo(true);

            bytes = freud.Serialize(false);
            Is.EqualTo(freud.Deserialize<byte>(bytes)).ApplyTo(false);
        }

        [Test]
        public void TestByte()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize((byte) 25);

            Is.EqualTo(freud.Deserialize<byte>(bytes)).ApplyTo(25);
        }

        [Test]
        public void TestShort()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize((short)25);

            Is.EqualTo(freud.Deserialize<short>(bytes)).ApplyTo(25);
        }


        [Test]
        public void TestInt()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize((int)25);

            Is.EqualTo(freud.Deserialize<int>(bytes)).ApplyTo(25);
        }

        [Test]
        public void TestLong()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize((long)9482648273);

            Is.EqualTo(freud.Deserialize<long>(bytes)).ApplyTo(9482648273);
        }

        [Test]
        public void TestUShort()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize(ushort.MaxValue);

            Is.EqualTo(freud.Deserialize<ushort>(bytes)).ApplyTo(ushort.MaxValue);
        }


        [Test]
        public void TestUInt()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize(uint.MaxValue);

            Is.EqualTo(freud.Deserialize<uint>(bytes)).ApplyTo(uint.MaxValue);
        }

        [Test]
        public void TestULong()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize(ulong.MaxValue);

            Is.EqualTo(freud.Deserialize<ulong>(bytes)).ApplyTo(ulong.MaxValue);
        }


        [Test]
        public void TestSingle()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize(25.23f);

            Is.EqualTo(freud.Deserialize<float>(bytes)).ApplyTo(25.23f);
        }

        [Test]
        public void TestDouble()
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize(130.122);

            Is.EqualTo(freud.Deserialize<double>(bytes)).ApplyTo(130.122);
        }

        [Test]
        public void TestString([Values("not empty string", "", null)] string data)
        {
            var freud = new FreudManager();
            var bytes = freud.Serialize(data);

            Is.EqualTo(freud.Deserialize<string>(bytes)).ApplyTo(data);
        }


    }
}
