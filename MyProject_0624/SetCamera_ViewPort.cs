using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Display;
using Rhino.DocObjects;

namespace MyProject_0624
{
    public class SetCamera_ViewPort : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the SetCamera_ViewPort class.
        /// </summary>
        public SetCamera_ViewPort()
          : base("Set Current Camera in ViewPort", "SetCurrentCamera",
              "Set Current Camera in a specified viewport",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Camera Point", "Camera", "Location  of the camera", GH_ParamAccess.item);
            pManager.AddPointParameter("Target Point", "Target", "Target of the camera", GH_ParamAccess.item);
            //pManager.AddTextParameter("ViewPort Name", "View", "Name of the viewport for setting", GH_ParamAccess.item);
            pManager.AddNumberParameter("Camera Focus Length", "Length", "Camera Focus Length", GH_ParamAccess.item);
            pManager[2].Optional = true;


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Output message", "Output", "Output message", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d camPt = new Point3d();
            Point3d tarPt = new Point3d();
            //string viewName = "";
            double cameraAngle = 35;
            RhinoViewport vp = new RhinoViewport();


            DA.GetData(0, ref camPt);
            DA.GetData(1, ref tarPt);
            
            DA.GetData(2, ref cameraAngle);



            
            vp = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView.ActiveViewport;

            vp.SetCameraLocations(tarPt, camPt);

            vp.Camera35mmLensLength = cameraAngle;

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();

















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
                return Properties.Resources.SetCurrentCamera_Viewport;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("d09806dd-7bd8-4fa0-998d-101768184d70"); }
        }
    }
}