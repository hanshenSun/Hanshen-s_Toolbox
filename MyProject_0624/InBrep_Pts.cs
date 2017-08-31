using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class InBrep_Pts : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the InBrep_Pts class.
        /// </summary>
        public InBrep_Pts()
          : base("InBrep_Pts", "Point In Brep",
              "Evaluate if a point is in the brep",
              "HS_ToolBox", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("pt", "pt", "pt", GH_ParamAccess.item);
            pManager.AddBrepParameter("Brep", "Brep", "Brep", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("bool", "bool", "bool", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d inputPt = new Point3d();
            Brep inputBrep = new Brep();
            

            DA.GetData(0, ref inputPt);
            DA.GetData(1, ref inputBrep);

            bool result = hs_functions.pointInBreps(inputPt, inputBrep, Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);

            DA.SetData(0, result);
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
            get { return new Guid("75b1130c-43d1-4eb6-8c81-e7406da3f342"); }
        }
    }
}