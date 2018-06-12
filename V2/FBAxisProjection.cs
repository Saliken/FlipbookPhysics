using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics.V2
{
    class FBAxisProjection
    {
        public float AMinimumValue;
        public float AMaximumValue;
        public float BMinimumValue;
        public float BMaximumValue;

        public float IntervalDistance()
        {
            if (AMinimumValue < BMinimumValue)
                return BMinimumValue - AMaximumValue;
            else
                return AMinimumValue - BMaximumValue;
        }
    }
}
