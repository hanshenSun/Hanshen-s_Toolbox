using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

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
            pManager.AddNumberParameter("Tolerance", "t", "Tolerance for evaluation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Index", "i", "index of Found Child Curves",GH_ParamAccess.list);
            pManager.AddBrepParameter("PipeBrep", "brep", "Brep for Preview", GH_ParamAccess.item);
            pManager.AddNumberParameter("distance", "distance", "distance", GH_ParamAccess.list);
            
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
            bool previewSwitch = new bool();
            List<int> index = new List<int>();
            List<double> tempMaxdistanceList = new List<double>();

            double tempMaxdistance;
            double tempMaxParaA;
            double tempMaxParaB;
            double tempMinDistance;
            double tempMinParaA;
            double tempMinParaB;


            DA.GetData(0, ref inputCrvA);
            DA.GetDataList(1, inputCrvsB);
            DA.GetData(2, ref previewSwitch);
            DA.GetData(3, ref tolerance);


            

            int i = 0;
            foreach (Curve crv in inputCrvsB)
            {


                /*
                if (Curve.GetDistancesBetweenCurves(inputCrvA, crv, tolerance, out tempMaxdistance, out tempMaxParaA, out tempMaxParaB, out tempMinDistance, out tempMinParaA, out tempMinParaB))
                {

                

                    if (tempMaxdistance <= tolerance)
                    {
                        index.Add(i);
                        tempMaxdistanceList.Add(tempMaxdistance);
                    }

                }
                */
                Point3d[] tempPts;
                crv.DivideByCount(2, true, out tempPts);
                List<bool> tempBools = new List<bool>();


                foreach (Point3d pt in tempPts)
                {
                    double tempParameter;
                    

                    if (inputCrvA.ClosestPoint(pt,out tempParameter, tolerance))
                    {

                        tempBools.Add(true);
                    }
                }


                if (tempBools.Count == 3)
                {
                    index.Add(i);
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
            DA.SetDataList(2, tempMaxdistanceList);



        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
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