using System;
using System.IO;
using System.Linq;
using System.Numerics;
namespace NSteam.NetworkDictionary
{

    enum NetDictionaryData
    {
        String = 'a',
        SString = 'A',
        Vec3 = 'b',
        SVec3 = 'B',
        Float = 'c',
        SFloat = 'C',
        Int = 'd',
        SInt = 'D',
        Vec2 = 'e',
        SVec2 = 'E'

    }
    public static class NetDicionary
    {

        public static byte[] Serialize(string resion, string data)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {

                bw.Write((char)NetDictionaryData.String);
                bw.Write(resion);
                bw.Write(data);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, string serial, string data)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.SString);

                bw.Write(resion);
                bw.Write(serial);
                bw.Write(data);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, Vector3 vector3)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.Vec3);

                bw.Write(resion);
                bw.Write(vector3.X);
                bw.Write(vector3.Y);
                bw.Write(vector3.Z);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, string serial, Vector3 vector3)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.SVec3);

                bw.Write(resion);
                bw.Write(serial);
                bw.Write(vector3.X);
                bw.Write(vector3.Y);
                bw.Write(vector3.Z);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, float _float)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.Float);

                bw.Write(resion);
                bw.Write(_float);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, string serial, float _float)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.SFloat);

                bw.Write(resion);
                bw.Write(serial);
                bw.Write(_float);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, int _int)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.Int);

                bw.Write(resion);
                bw.Write(_int);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, string serial, int _int)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.SInt);

                bw.Write(resion);
                bw.Write(serial);
                bw.Write(_int);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, Vector2 _Vector2)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.Vec2);

                bw.Write(resion);
                bw.Write(_Vector2.X);
                bw.Write(_Vector2.Y);
                return ms.ToArray();
            }
        }

        public static byte[] Serialize(string resion, string serial, Vector2 _Vector2)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((char)NetDictionaryData.SVec2);

                bw.Write(resion);
                bw.Write(serial);
                bw.Write(_Vector2.X);
                bw.Write(_Vector2.Y);
                return ms.ToArray();
            }
        }


        private static string[] DeSerializeString(byte[] data)
        {
            string[] returningdata = new string[2];
            using (MemoryStream ms = new MemoryStream(data, 0, data.Length))
            using (BinaryReader br = new BinaryReader(ms))
            {
                br.ReadChar();
                returningdata[0] = br.ReadString();
                returningdata[1] = br.ReadString();
            }
            return returningdata;
        }

        private static string[] DeSerializeSString(byte[] data)
        {
            string[] returningdata = new string[3];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);

            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadString();
            returningdata[2] = br.ReadString();

            return returningdata;
        }

        private static string[] DeSerializeVec3(byte[] data)
        {
            string[] returningdata = new string[4];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadSingle().ToString();
            returningdata[2] = br.ReadSingle().ToString();
            returningdata[3] = br.ReadSingle().ToString();
            return returningdata;
        }

        private static string[] DeSerializeSVec3(byte[] data)
        {
            string[] returningdata = new string[5];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadString();
            returningdata[2] = br.ReadSingle().ToString();
            returningdata[3] = br.ReadSingle().ToString();
            returningdata[4] = br.ReadSingle().ToString();
            return returningdata;
        }


        private static string[] DeSerializeFloat(byte[] data)
        {
            string[] returningdata = new string[2];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadSingle().ToString();
            return returningdata;
        }

        private static string[] DeSerializeSFloat(byte[] data)
        {
            string[] returningdata = new string[3];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadString();
            returningdata[2] = br.ReadSingle().ToString();
            return returningdata;
        }


        private static string[] DeSerializeInt(byte[] data)
        {
            string[] returningdata = new string[2];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadInt32().ToString();
            return returningdata;
        }

        private static string[] DeSerializeSInt(byte[] data)
        {
            string[] returningdata = new string[3];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadString();
            returningdata[2] = br.ReadInt32().ToString();
            return returningdata;
        }


        private static string[] DeSerializeVec2(byte[] data)
        {
            string[] returningdata = new string[3];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadSingle().ToString();
            returningdata[2] = br.ReadSingle().ToString();
            return returningdata;
        }

        private static string[] DeSerializeSVec2(byte[] data)
        {
            string[] returningdata = new string[4];
            MemoryStream ms = new MemoryStream(data, 0, data.Length);
            BinaryReader br = new BinaryReader(ms);
            br.ReadChar();
            returningdata[0] = br.ReadString();
            returningdata[1] = br.ReadString();
            returningdata[2] = br.ReadSingle().ToString();
            returningdata[3] = br.ReadSingle().ToString();
            return returningdata;
        }

        public static string[] DeSerialize(byte[] data)
        {
            string[] returningdata;
            NetDictionaryData fierstData;
            using (MemoryStream ms = new MemoryStream(data, 0, data.Length))
            using (BinaryReader br = new BinaryReader(ms))
            {
                fierstData = (NetDictionaryData)br.ReadChar();
            }
            switch (fierstData)
            {
                case NetDictionaryData.String:
                    returningdata = DeSerializeString(data);
                    break;
                case NetDictionaryData.SString:
                    returningdata = DeSerializeSString(data);
                    break;
                case NetDictionaryData.Vec3:
                    returningdata = DeSerializeVec3(data);
                    break;
                case NetDictionaryData.SVec3:
                    returningdata = DeSerializeSVec3(data);
                    break;
                case NetDictionaryData.Float:
                    returningdata = DeSerializeFloat(data);
                    break;
                case NetDictionaryData.SFloat:
                    returningdata = DeSerializeSFloat(data);
                    break;
                case NetDictionaryData.Int:
                    returningdata = DeSerializeInt(data);
                    break;
                case NetDictionaryData.SInt:
                    returningdata = DeSerializeSInt(data);
                    break;
                case NetDictionaryData.Vec2:
                    returningdata = DeSerializeVec2(data);
                    break;
                case NetDictionaryData.SVec2:
                    returningdata = DeSerializeSVec2(data);
                    break;
                default:
                    returningdata = new string[] { "", "" };
                    break;
            }


            return returningdata;
        }

        public static string GenerateRandomSerial(int length = 4, string filter = "")
        {
            if (filter == null || filter == "" || filter.Trim(' ') != filter)
            {
                filter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ12@3#4567890_";
            }
            var random = new Random();
            return new string(Enumerable.Repeat(filter, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static string[] HandleMessage(string data)
        {
            string[] newdata = data.Split("{{cut}}");


            return newdata;
        }
        public static string GenMessage(string[] data)
        {
            string newdata = "";
            foreach (string s in data)
            {
                newdata += s + "{{cut}}";
            }

            return newdata;
        }
    }
}
