using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class CurveDirectionUnifier : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CurveDirectionUnifier()
          : base("CurveDirectionUnifier", "CurveDirUnifier",
              "Unifies all the input curves towards a similar direction within tolerance",
              "HS_ToolBox", "GeoFix")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "Crv", "Curves for sorting", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "t", "Tolerance in degree for the unifier", GH_ParamAccess.item);
            
            




        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Sorted Curves", "Crv", "Sorted Curves", GH_ParamAccess.list);
            pManager.AddVectorParameter("CurveTangent", "t", "Vector at middle", GH_ParamAccess.list);
            pManager.AddTextParameter("Last Updated Date", "date","BuildDate", GH_ParamAccess.item);
            pManager.AddVectorParameter("firstVector", "FirstVec", "firstVector", GH_ParamAccess.item);
            pManager.AddNumberParameter("DiffAngles", "angle", "Difference in Angles", GH_ParamAccess.list);



        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //initial variable
            List<Curve> originCrvs = new List<Curve>();
            double flipTolerance = new double();

            //calculation variable




            //setting input data to varibles
            if (!DA.GetDataList(0, originCrvs)) return;
            if (!DA.GetData(1, ref flipTolerance)) return;
            
            





            //adding warnings
            if (originCrvs.Count<= 1) {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Need more than one curve for sorting");

                return;
            }


            if (flipTolerance < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Tolerance cannot be less than 0");

                return;
                
            }





            //Now let the coding game begin!!!
            //Yayyyyyy
            //Curve firstCurve = originCrvs[0];
            //Vector3d firstVector = firstCurve.TangentAt(flipTolerance);


            List<Vector3d> curveVectors = new List<Vector3d>();
            Vector3d evaluatedVectors = new Vector3d();
            List<double> diffAngles = new List<double>();







            foreach (Curve curvetoSort in originCrvs)
            {

                double curveLength = curvetoSort.GetLength();
                double halfLength = curveLength / 2;
                evaluatedVectors = curvetoSort.TangentAt(halfLength);
                curveVectors.Add(evaluatedVectors);


            }



            Vector3d firstVector = curveVectors[0];
            double tempAngle = new double();



            for (int i = 1; i< curveVectors.Count; i +=1)
            {
                tempAngle = Vector3d.VectorAngle(firstVector, curveVectors[i]);
                diffAngles.Add(tempAngle);

            }







            DA.SetDataList(0, originCrvs);
            DA.SetDataList(1, curveVectors);
            DA.SetData(2, "0627_11pm");
            DA.SetData(3, firstVector);
            DA.SetDataList(4, diffAngles);
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
            get { return new Guid("44239989-3646-462f-883e-3ed15d0b279f"); }
        }
    }
}