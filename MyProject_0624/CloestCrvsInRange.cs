using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using MyProject_0624.Properties;

namespace MyProject_0624
{
    public class CloestCrvsInRange : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CloestCrvsInRange()
          : base("Cloest Crvs in range", "CloestCrvsInRange",
              "Finding the  curves using geometry inclusion algorithm",
              "HS_Toolbox", "analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "CrvA", "Curve (New) to generate pipes from", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curves", "CrvB", "Curve (Old) to seach from", GH_ParamAccess.list);
            pManager.AddBooleanParameter("Preview", "Preview", "preview option for the inclusion range", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "Distance", "Tolerance for evaluation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Child Crv Percentage Tolerance", "Tolerance", "Child Crv Percentage Tolerance, for exampe: for tolerance domain between 90% - 110%, input 0.1 HERE", GH_ParamAccess.list);
            pManager[4].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Index", "I", "index of Found Child Curves",GH_ParamAccess.list);
            pManager.AddBrepParameter("PipeBrep", "Brep", "Brep for Preview", GH_ParamAccess.item);
            pManager.AddNumberParameter("distance", "Distance", "distance", GH_ParamAccess.list);
            pManager.AddTextParameter("Child/Parent?", "R", "Child or Parents?", GH_ParamAccess.list);

        }


        private static void findCloestPt(Point3d[] points, Curve inputCrv, double tol, ref List<bool> tempBool)
        {

            foreach (Point3d pt in points)
            {
                double tempParameter;
                //List<bool> tempBool = new List<bool>();
                List<bool> dummyList = new List<bool>();
                if (inputCrv.ClosestPoint(pt, out tempParameter, tol))//check if there is any closest point within the tolerated distance
                {

                    tempBool.Add(true);
                    
                }
                else
                {
                    
                    break;
                }

                //return tempBools;
            }

        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve inputCrvA = new NurbsCurve(2,2);
            List<Curve> inputCrvsB = new List<Curve>();
            double tolerance = new double();
            double childTolerance = new double();
            bool previewSwitch = new bool();
            List<int> index = new List<int>();
            List<double> tempMaxdistanceList = new List<double>();
            List<double> FoundCrvsLength = new List<double>();
            List<double> foundDistance = new List<double>();
            List<string> childParentBool = new List<string>();



            DA.GetData(0, ref inputCrvA);
            DA.GetDataList(1, inputCrvsB);
            DA.GetData(2, ref previewSwitch);
            DA.GetData(3, ref tolerance);


            if (tolerance <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Distance cannot be less than 0");
            }
               


            double inputCrvALength = inputCrvA.GetLength();


            int i = 0;
            foreach (Curve crv in inputCrvsB)
            {
                Point3d[] tempPts;
                crv.DivideByCount(2, true, out tempPts); // dividing every curve into 3 pts (including ends)
                List<bool> tempBool = new List<bool>();


                //foreach (Point3d pt in tempPts)
                //{
                //    double tempParameter;


                //    if (inputCrvA.ClosestPoint(pt,out tempParameter, tolerance))//check if there is any closest point within the tolerated distance
                //    {

                //        tempBools.Add(true);
                //    }
                //    /*
                //    else
                //    {
                //        return;
                //    }

                //    Potential Class Impementation for faster computation
                //    */ 

                //}

                findCloestPt(tempPts, inputCrvA, tolerance, ref tempBool);




                
                if (tempBool.Count == 3)//if found
                {
                    index.Add(i);
                    

                    Point3d tempPtA;
                    Point3d tempPtB;
                    inputCrvA.ClosestPoints(crv, out tempPtA, out tempPtB);//find out where the cloest points are
                    foundDistance.Add(tempPtA.DistanceTo(tempPtB));


                    double tempCrvLength = crv.GetLength();


                    FoundCrvsLength.Add(tempCrvLength);

                    bool childResult = tempCrvLength / inputCrvALength > 1 + childTolerance;

                    if (tempCrvLength/inputCrvALength < childTolerance)
                    {
                        childParentBool.Add("child");

                    }
                    else if (tempCrvLength / inputCrvALength > 1 + childTolerance)
                    {
                        childParentBool.Add("parent");
                    }


                }



                
               


                

                i++;
            }



            if (previewSwitch == true)
            {
                Brep[] curvePipe = Brep.CreatePipe(inputCrvA, tolerance, false, PipeCapMode.Round, true, 0.1, 0.1);
                Brep pipeGeometry = curvePipe[0];
                DA.SetData(1, pipeGeometry);
                    
            }
            DA.SetDataList(0, index);
            DA.SetDataList(2, foundDistance);
            DA.SetDataList(3, childParentBool);



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:


                return Resources.CloestCrvsInRange;


                //return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("948be464-92a3-4bdf-8d20-a18ee7d50439"); }
        }
    }
}