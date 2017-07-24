using System;
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
          : base("Crv in Radius", "Cloest Crv in Radius",
              "Find Similar Member by Searching Within a Certain Radius",
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
            pManager.AddBooleanParameter("Preview", "Preview", "preview option for the inclusion range", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance", "t", "Tolerance for evaluation",GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Result", "r", "Result from the evaluation",GH_ParamAccess.item);
            pManager.AddIntegerParameter("Index", "i", "Found Member Index", GH_ParamAccess.list);
            pManager.AddGeometryParameter("Pipe Brep", "g", "Pipe Geometry for Preview", GH_ParamAccess.item);
            pManager.AddTextParameter("output", "p", "outputMessage", GH_ParamAccess.item);


        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double tolerance = new double();
            bool previewSwitch = new bool();
            

            Curve inputCrvA = new NurbsCurve(2,2);
            
            List<Curve> inputCrvsB = new List<Curve>();
            List<string> message = new List<string>();
            List<bool> results = new List<bool>();
            List<int> ouputIndex = new List<int>();
            List<Brep> pipeGeometries = new List<Brep>();


            DA.GetData(0, ref inputCrvA);
            DA.GetDataList(1, inputCrvsB);
            DA.GetData(2, ref previewSwitch);
            DA.GetData(3, ref tolerance);



            if (previewSwitch == true)
            {
                Brep[] curvePipe = Brep.CreatePipe(inputCrvA, tolerance, false, PipeCapMode.Round, true, 0.1, 0.1);
                Brep pipeGeometry = curvePipe[0];
                pipeGeometries.Add(pipeGeometry);

            }

            
            Point3d mainStartPt = inputCrvA.PointAtStart;
            Point3d mainEndpt = inputCrvA.PointAtEnd;





            int i = 0;
            foreach (Curve crv in inputCrvsB) {
                Point3d startptTemp = crv.PointAtStart;
                Point3d endptTemp = crv.PointAtEnd;

                double distanceA = mainStartPt.DistanceTo(startptTemp);
                double distanceB = mainStartPt.DistanceTo(endptTemp);

                //if one point matches
                if (distanceA < tolerance || distanceB < tolerance)
                {
                    if (distanceA < distanceB)
                    {
                        //means start point of the temp curve is closer
                        double distanceEndtoEnd = mainEndpt.DistanceTo(endptTemp);
                        if (distanceEndtoEnd < tolerance)
                        {
                            results.Add(true);
                            ouputIndex.Add(i);
                            message.Add("Found/NotFlipped");
                        }
                    }
                    if (distanceA > distanceB)
                    {
                        //means end point of the temp curve is closer
                        double distanceEndtoStart = mainEndpt.DistanceTo(startptTemp);
                        if (distanceEndtoStart < tolerance)
                        {
                            results.Add(true);
                            ouputIndex.Add(i);
                            message.Add("Found/Flipped");
                        }
                    }
                    
                }

                else
                {
                    results.Add(false);
                    message.Add("NOTfound/");

                }
                i++;
            }

            

            
            DA.SetDataList(0, results);
            DA.SetDataList(1, ouputIndex);
            DA.SetData(2, pipeGeometries);
            DA.SetData(3, message);


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