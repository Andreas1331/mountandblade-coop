using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBCoopLibrary
{
    public class Vector3
    {
        public float X { get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }
        public float Y
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }
        public float Z
        {
            get
            {
                return Z;
            }
            set
            {
                Z = value;
            }
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
