using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class Silhouette_ViewPort : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the Silhouette_ViewPort class.
        /// </summary>
        public Silhouette_ViewPort()
          : base("Silhouette_ViewPort", "Silhouette",
              "Create a Silhouette representation of the input geometries",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Input Breps", "Brep", "Breps for creating silhouette", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Silhouette Curves", "Crv", "Silhouette Curves", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> inputGeos = new List<Brep>();

            DA.SetDataList(0, inputGeos);

            foreach (Brep b in inputGeos)
            {
                //Rhino.Geometry.sil
            }
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
            get { return new Guid("626f4fee-bcef-4f58-b3a4-c03d1e424550"); }
        }
    }
}