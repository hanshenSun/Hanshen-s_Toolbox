using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class DirectionUnifier_Crv : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public DirectionUnifier_Crv()
          : base("CurveDirectionUnifier", "CurveDirUnifier",
              "Unifies all the input curves towards a similar direction within tolerance",
              "HS_ToolBox", "GeometryFix")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "Crv", "Curves for sorting", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "t", "Tolerance in Degree for the unifier", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Sorted Curves", "Crv", "Sorted Curves", GH_ParamAccess.list);
            pManager.AddVectorParameter("CurveTangent", "T", "Vector at middle", GH_ParamAccess.list);
            pManager.AddVectorParameter("firstVector", "FirstVec", "firstVector", GH_ParamAccess.item);
            pManager.AddNumberParameter("DiffAngles", "Angle", "Difference in Angles", GH_ParamAccess.list);
            pManager.AddTextParameter("flipped?", "f?", "flipped?", GH_ParamAccess.list);
        }



        private static void findCrvVector(Curve inputCrv, out Vector3d evaluatedVectors)//this method finds the vector of curve at center point
        {
            
            double curveLength = inputCrv.GetLength();
            double halfLength = curveLength / 2;
            evaluatedVectors = inputCrv.TangentAt(halfLength);


        }
        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            //initial variable
            List<Curve> inputCrvs = new List<Curve>();
            List<Curve> outputCrvs = new List<Curve>();
            List<Vector3d> curveVectors = new List<Vector3d>();
            List<double> diffAngles = new List<double>();
            List<string> flipBool = new List<string>();

            double flipTolerance = new double();



            //setting input data to varibles
            if (!DA.GetDataList(0, inputCrvs)) return;
            if (!DA.GetData(1, ref flipTolerance)) return;
            
            
            

            //adding warnings
            if (inputCrvs.Count<= 1) {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Need more than one curve for sorting");
                return;
            }


            if (flipTolerance < 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Tolerance cannot be less than 0");
                return;
                
            }




            


            //retrieving the vector at midpoint of the first curve at a key reference vector
            Vector3d firstCrvVector = new Vector3d();
            findCrvVector(inputCrvs[0], out firstCrvVector);
            outputCrvs.Add(inputCrvs[0]);


            //looping for getting the rest of the crv vector and formatting crvs
            for (int i = 1; i < inputCrvs.Count; i += 1)
            {

                //retrieve vector
                Curve tempCrv = inputCrvs[i];
                Vector3d tempVector = new Vector3d();
                findCrvVector(tempCrv, out tempVector);
                curveVectors.Add(tempVector);

                //retrieve angles in degree
                double tempAngle = Vector3d.VectorAngle(firstCrvVector, tempVector) * 57.2958;

                diffAngles.Add(tempAngle);

                //flipping curves if necessary
                if (tempAngle > flipTolerance)
                {
                    tempCrv.Reverse();
                    
                    flipBool.Add("true");
                }

                else
                {
                    flipBool.Add("false");
                }
                outputCrvs.Add(tempCrv);

            }






            DA.SetDataList(0, outputCrvs);
            DA.SetDataList(1, curveVectors);
            DA.SetData(2, firstCrvVector);
            DA.SetDataList(3, diffAngles);
            DA.SetDataList(4, flipBool);

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
                return Properties.Resources.DirectionUnifier_Crv;
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