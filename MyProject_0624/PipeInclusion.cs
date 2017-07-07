﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class PipeInclusion : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PipeInclusion()
          : base("Brep Inclusion for diagonal members", "PointInPipe",
              "Brep (Pipe) Inclusion for diagonal members",
              "HS_ToolBox", "analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "CrvA", "Curve (New) to generate pipes from", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curves", "CrvB", "Curve (Old) to seach from", GH_ParamAccess.list);

            pManager.AddPointParameter("Point", "pt", "Point for evaluation", GH_ParamAccess.list);
            pManager.AddNumberParameter("Tolerance", "t", "Tolerance for evaluation",GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Result", "r", "Result from the evaluation",GH_ParamAccess.item);
            pManager.AddMeshParameter("Pipe Mesh", "m", "Pipe Mesh for Preview", GH_ParamAccess.item);
            pManager.AddTextParameter("output", "p", "outputMessage", GH_ParamAccess.item);


        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve inputCrvA = new NurbsCurve(2,2);
            List<Curve> inputCrvsB = new List<Curve>();

            List<Point3d> testPts = new List<Point3d>();
            double tolerance = new double();
            //Brep curvePipe = new Brep();
            //PipeCapMode cap = PipeCapMode.Round;
            List<bool> results = new List<bool>();
             



            DA.GetData(0, ref inputCrvA);
            DA.GetDataList(1, inputCrvsB);
            DA.GetDataList(2, testPts);
            DA.GetData(3, ref tolerance);



            //Brep[] curvePipe = Brep.CreatePipe(inputCrv, 0.5, true, cap, true, 0.0,0.0);
            Brep[] curvePipe = Brep.CreatePipe(inputCrvA, tolerance, false, PipeCapMode.Round, true, 0.1, 0.1);
            Brep pipeGeometry = curvePipe[0];

            string message = "Got PIPe";


            //diving inputCrvsB
            
            



            
            foreach (Point3d pt in testPts)
            {
                bool result = new bool();
                result = pipeGeometry.IsPointInside(pt, tolerance, true);
                results.Add(result);

            }
            
            DA.SetDataList(0, results);
            DA.SetData(1, message);


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
            get { return new Guid("dfd95aa3-a02f-47fd-ba8e-2a7a29348588"); }
        }
    }
}