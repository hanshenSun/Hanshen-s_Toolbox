using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class EarthCoordinatesToXY : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the EarthCoordinatesToXY class.
        /// </summary>
        public EarthCoordinatesToXY()
          : base("EarthCoordinatesToXY", "EarthCoordinatesToXY",
              "Convert Latitude and Longitude to XY data using Mercator Projection",
              "HS_ToolBox", "GIS")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Lat", "Lat", "Latitude", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lon", "Lon", "Longitude", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("XY Point", "XY Point", "XY Point", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double latitude = 0.0;
            double longitude = 0.0;

            DA.GetData(0, ref latitude);
            DA.GetData(1, ref longitude);

            double latitudeY = MercatorProjection.latToY(latitude);
            double latitudeX = MercatorProjection.lonToX(longitude);


            Point3d quryedPt = new Point3d(latitudeX, latitudeY, 0);
            DA.SetData(0, quryedPt);

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
            get { return new Guid("201a98a7-899b-491a-85c1-aaef4b709c19"); }
        }
    }
}