using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class getElevation_GIS : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the getElevation_GIS class.
        /// </summary>
        public getElevation_GIS()
          : base("getElevation_GIS", "getElevation",
              "Get Elevation data in meters using Google Elevation API",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Lat", "Latitude", "Latitude", GH_ParamAccess.list);
            pManager.AddNumberParameter("Lon", "Longitude", "Longitude", GH_ParamAccess.list);
            pManager.AddNumberParameter("Res", "Resolution", "Resolution", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
            get { return new Guid("aa400a42-54f7-4943-811f-83d754529fdf"); }
        }
    }
}