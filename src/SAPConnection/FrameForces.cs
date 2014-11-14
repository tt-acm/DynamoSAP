using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAPConnection
{
    public class FrameForces
    {
        //FIELDS
        private string myID;
        private string myName;

        private double myF1_start;
        private double myF1_end;
        private double myF2_start;
        private double myF2_end;
        private double myF3_start;
        private double myF3_end;

        private double myM1_start;
        private double myM1_end;
        private double myM2_start;
        private double myM2_end;
        private double myM3_start;
        private double myM3_end;

       // private string myStatus;
        //private string myChangeID;

        //PROPERTIES
        public string ID
        {
            get { return myID; }
            set
            {
                try
                {
                    //test for the empty string
                    if (value == "")
                    {
                        throw new ArgumentException("The input string cannot be empty");
                    }

                    myID = value;
                }
                catch (Exception e) //should catch the null case
                {
                    throw e;
                }
            }
        }

        public string Name
        {
            get { return myName; }
            set
            {
                myName = value;               
            }
        }

        //public string ChangeID
        //{
        //    get { return myChangeID; }
        //    set
        //    {
        //        try
        //        {
        //            //test for the empty string
        //            if (value == "")
        //            {
        //                throw new ArgumentException("The input string cannot be empty");
        //            }

        //            myChangeID = value;
        //        }
        //        catch (Exception e)  //should catch the null case
        //        {
        //            throw e;
        //        }
        //    }
        //}

        //public string Status
        //{
        //    get { return myStatus; }
        //    set
        //    {
        //        try
        //        {
        //            //test for the empty string
        //            if (value == "")
        //            {
        //                throw new ArgumentException("The input string cannot be empty");
        //            }

        //            myStatus = value;
        //        }
        //        catch (Exception e)    //should catch the null case
        //        {
        //            throw e;
        //        }
        //    }
        //}

        //Shear
            // axial P
        public double P_start
        {
            get { return myF1_start; }
            set { myF1_start = value; }
        }
        public double P_end
        {
            get { return myF1_end; }
            set { myF1_end = value; }
        }
            // Major V2
        public double V2_start
        {
            get { return myF2_start; }
            set { myF2_start = value; }
        }
        public double V2_end
        {
            get { return myF2_end; }
            set { myF2_end = value; }
        }
         // Minor V3
        public double V3_start
        {
            get { return myF3_start; }
            set { myF3_start = value; }
        }
        public double V3_end
        {
            get { return myF3_end; }
            set { myF3_end = value; }
        }
        // Moment
            // Torsion
        public double T_start
        {
            get { return myM1_start; }
            set { myM1_start = value; }
        }
        public double T_end
        {
            get { return myM1_end; }
            set { myM1_end = value; }
        }
            //minor M2
        public double M2_start
        {
            get { return myM2_start; }
            set { myM2_start = value; }
        }
        public double M2_end
        {
            get { return myM2_end; }
            set { myM2_end = value; }
        }
            // major M3
        public double M3_start
        {
            get { return myM3_start; }
            set { myM3_start = value; }
        }
        public double M3_end
        {
            get { return myM3_end; }
            set { myM3_end = value; }
        }

        public override string ToString()
        {
            return Name;
        }

        //CONSTRUCTORS
        public FrameForces() { }

        public FrameForces(double[] P, double[] V2, double[] V3, double[] T, double[] M2, double[] M3)
        {
            myF1_start = P[0];
            myF1_end = P[1];
            myF2_start = V2[0];
            myF2_end = V2[1];
            myF3_start = V3[0];
            myF3_end = V3[1];

            myM1_start = T[0];
            myM1_end = T[1];
            myM2_start = M2[0];
            myM2_end = M2[1];
            myM3_start = M3[0];
            myM3_end = M3[1];
        }

    }
}
