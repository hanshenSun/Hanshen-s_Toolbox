using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class GetCenterline_Pipe : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the GetCenterline_Pipe class.
        /// </summary>
        public GetCenterline_Pipe()
          : base("CenterLine_Pipe", "GetCenterline_Pipe",
              "Reprieves the centerline OR CURVES! of pipes",
              "HS_ToolBox", "GeometryFix")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Pipe Surfaces", "Pipes", "Input Surfaces for Retrieving centerline", GH_ParamAccess.list);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("centerLines", "centerLines", "centerLines", GH_ParamAccess.list);

        }




        private static Curve getTweenCrv(Surface pipeSrf, double midParam, int srfDirection)
        {
            
            Curve isoCrvA = pipeSrf.IsoCurve(srfDirection, 0);
            Curve isoCrvB = pipeSrf.IsoCurve(srfDirection, midParam);
            Curve[] tweenCrvs = Curve.CreateTweenCurves(isoCrvA, isoCrvB, 1);
            Curve tweenCrv = tweenCrvs[0];

            return tweenCrv;
        } 


        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Surface> inputSrf = new List<Surface>();
            List<Curve> centerCurves = new List<Curve>();
            
            DA.GetDataList(0, inputSrf);
            

            
            foreach(Surface srf in inputSrf)
            {
                double paramU = srf.Domain(0).Mid;
                double paramV = srf.Domain(1).Mid;

                Curve crvU = srf.IsoCurve(0, paramU);
                Curve crvV = srf.IsoCurve(1, paramV);

                int surfaceDirection = new int();
                double midParam = new double();

                if (crvU.GetLength() < crvV.GetLength())
                {
                    surfaceDirection = 1;
                    midParam = paramU;

                }
                else
                {
                    surfaceDirection = 0;
                    midParam = paramV;
                }

                centerCurves.Add(getTweenCrv(srf, midParam, surfaceDirection));
                
            }
            DA.SetDataList(0, centerCurves);



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
                return Properties.Resources.GetCenterLine_Pipe;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("f840e39a-7312-4ce3-9ae1-5542beb37628"); }
        }
    }
}