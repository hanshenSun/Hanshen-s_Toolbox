using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class ptToEarthCoordinates_Gis : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the ptToEarthPt class.
        /// </summary>
        public ptToEarthCoordinates_Gis()
          : base("ptToEarthPt", "ptToEarthPt",
              "Converts Rhino Coordinates to Longitude and Latitude",
              "HS_ToolBox", "GIS")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("pt", "pt", "qury point", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Lat", "Lat", "Latitude", GH_ParamAccess.item);
            pManager.AddNumberParameter("Lon", "Lon", "Longitude", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d inputPt = new Point3d();
            double longitude = 0.0;
            double latitude = 0.0;

            DA.GetData(0, ref inputPt);

            EarthAnchorPoint eaPt = new EarthAnchorPoint();
            eaPt.ModelBasePoint = inputPt;
            Rhino.UnitSystem us = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem;

            eaPt.GetModelToEarthTransform(us);
            latitude = eaPt.EarthBasepointLatitude;
            longitude = eaPt.EarthBasepointLongitude;

            DA.SetData(0, latitude);
            DA.SetData(1, longitude);
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
            get { return new Guid("d1757957-c918-4f6c-875c-ad7be7b0e6d4"); }
        }
    }
}