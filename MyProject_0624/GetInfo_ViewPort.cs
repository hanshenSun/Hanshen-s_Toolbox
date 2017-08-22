using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.Display;

namespace MyProject_0624
{
    public class GetInfo_ViewPort : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GetInfo_ViewPort()
          : base("Get Viewport Information", "GetInfo_ViewPort",
              "Get Camera Target points and viewport plane of the current viewport",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Camera Pt", "Camera", "Camera location of current viewport", GH_ParamAccess.item);
            pManager.AddPointParameter("Target Pt", "Target", "Target location of current viewport", GH_ParamAccess.item);
            pManager.AddNumberParameter("Focus Length", "Length", "Focal length of current camera", GH_ParamAccess.item);
            pManager.AddPlaneParameter("ViewPort Plane", "Plane", "View plan of current viewport", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RhinoViewport vp = RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

            Point3d cameraPt = vp.CameraLocation;
            Point3d targetPt = vp.CameraTarget;
            double focusLength = vp.Camera35mmLensLength;
            Plane focusPlane = new Plane();
            vp.GetCameraFrame(out focusPlane);


            DA.SetData(0, cameraPt);
            DA.SetData(1, targetPt);
            DA.SetData(2, focusLength);
            DA.SetData(3, focusPlane);




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
                return Properties.Resources.GetInfo_Viewport;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0d07bf74-9671-4b5b-93ab-afba21d1e188"); }
        }
    }
}