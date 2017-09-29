using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class AddLight_ViewPort : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddLight_Render class.
        /// </summary>
        public AddLight_ViewPort()
          : base("AddLight_Render", "Add Light",
              "Add light objects into current context",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Start Point", "Start Point", "Light Start Point", GH_ParamAccess.item);
            pManager.AddPointParameter("End Point", "End Point", "Light End Point", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Light Type", "Light Type", "Type of light, for example, directional light, point light, or ambient light", GH_ParamAccess.item);
            pManager.AddNumberParameter("Intensity", "Intensity", "Intensity", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Message", "Message", "Message", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d startPt = new Point3d();
            Point3d endPt = new Point3d();
            double lightIntensity = 10.0;
            int lightType = 4;

            DA.GetData(0, ref startPt);
            DA.GetData(1, ref endPt);
            DA.GetData(2, ref lightType);
            DA.GetData(3, ref lightIntensity);

            Light directLight = new Light();
            Vector3d dirVector = Point3d.Subtract(startPt, endPt);

            directLight.Direction = dirVector;
            directLight.Intensity = lightIntensity;




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
            get { return new Guid("4cf96a3b-857b-4d56-a483-af54efe83d6e"); }
        }
    }
}