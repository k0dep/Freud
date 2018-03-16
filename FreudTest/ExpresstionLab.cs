using System;
using System.IO;
using System.Linq.Expressions;
using NUnit.Framework;

[TestFixture]
public class ExpresstionLab
{
    public void ExprLaboratory()
    {
    }

    public void s(object obj, Stream stream)
    {
        foobar obj_foobar = (foobar) obj;
        if (obj_foobar != null)
        {
            int val_foobar_intField = obj_foobar.intField;
            byte[] bytes_foobar_intField = BitConverter.GetBytes(val_foobar_intField);
            stream.Write(bytes_foobar_intField, 0, bytes_foobar_intField.Length);

            int val_foobar_byteProp = obj_foobar.byteProp;
            byte[] bytes_foobar_byteProp = BitConverter.GetBytes(val_foobar_byteProp);
            stream.Write(bytes_foobar_byteProp, 0, bytes_foobar_byteProp.Length);

            foobaz val_foobar_foobazField = obj_foobar.foobazField;
            if (val_foobar_foobazField != null)
            {
                string val_foobaz_stringField = val_foobar_foobazField.stringField;
                if (val_foobaz_stringField != null)
                {
                }
                else
                {
                    stream.WriteByte(0xFF);
                }

                float val_foobaz_floatProp = val_foobar_foobazField.floatProp;
                byte[] bytes_foobaz_floatProp = BitConverter.GetBytes(val_foobaz_floatProp);
                stream.Write(bytes_foobaz_floatProp, 0, bytes_foobaz_floatProp.Length);
            }
            else
            {
                stream.WriteByte(0xFF);
            }
        }
        else
        {
            stream.WriteByte(0xFF);
        }


        object accessor = null;

        // class
        {
            var variable = accessor;
            if (variable != null)
            {
                // class field serialization
            }
            else
            {
                stream.WriteByte(0xFF);
            }
        }

        //struct
        {
            var variable = accessor;
            // struct field serialization
        }
    }

    Expression GenerateConcreteSerialization(Expression varObject, Expression varStream)
    {
        return null;
    }
}

public class foobar
{
    public int intField;
    public byte byteProp { get; set; }
    public foobaz foobazField;
}

public class foobaz
{
    public string stringField;
    public float floatProp { get; set; }
}