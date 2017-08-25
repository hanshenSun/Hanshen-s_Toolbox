using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class getBrepOutline_Brep : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the getBrepOutline__Brep class.
        /// </summary>
        public getBrepOutline_Brep()
          : base("getBrepOutline__Brep", "getBrepOutline__Brep",
              "get Brep's outline",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("inputBrep", "Brep", "input brep for outline", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Outline Crv", "Crv", "Outline Curve of the input brep", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Brep> inputBrep = new List<Brep>();
            DA.GetDataList(0,inputBrep);


            List<Curve> outlines = hs_functions.getBrepOutline(inputBrep);

            DA.SetDataList(0, outlines);


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
            get { return new Guid("3789139d-6668-4fae-870f-b3dd6ec9f8a5"); }
        }
    }
}