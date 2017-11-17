using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624._ViewPort
{
    public class AddSpotLight : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddSpotLight class.
        /// </summary>
        public AddSpotLight()
          : base("AddSpotLight", "Add Spot Light",
              "Adds a spot light to the current rhino documenmt",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Name", "Name", "Name is used to update ", GH_ParamAccess.item);
            pManager.AddPointParameter("Location", "Location", "Location of the Light Object", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "Diffuse Color", "Diffuse Color of the Light", GH_ParamAccess.item);
            pManager.AddNumberParameter("Intensity", "Intensity of the light", "Intensity(Min 0.0, Max 1.0) of the light object", GH_ParamAccess.item);
            pManager.AddBooleanParameter("initialize?", "initialize the light", "set true to initialize the light", GH_ParamAccess.item);

            //pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
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
            Light newLight = new Light();
            string lightName = "";
            Point3d lightLoc = new Point3d(0, 0, 0);
            Color lightColor = new Color();
            lightColor = Color.Cyan;
            double lightIntensity = 0.0;
            bool initializeBool = false;

            DA.GetData(0, ref lightName);
            DA.GetData(1, ref lightLoc);
            DA.GetData(2, ref lightColor);
            DA.GetData(3, ref lightIntensity);
            DA.GetData(4, ref initializeBool);

            newLight.Name = lightName;
            newLight.LightStyle = LightStyle.WorldSpot;
            newLight.Location = lightLoc;
            newLight.Diffuse = lightColor;
            newLight.Intensity = lightIntensity;
            newLight.IsEnabled = initializeBool;

            if (initializeBool == true)
            {

                List<Guid> foundLightID = hs_functions.getLightObjectIds(lightName);


                if (foundLightID.Count > 0)
                {
                    List<int> foundIDIndex = new List<int>();
                    foreach (Guid id in foundLightID)
                    {
                        Rhino.RhinoDoc.ActiveDoc.Lights.Modify(id, newLight);
                    }
                }
                else
                {
                    Rhino.RhinoDoc.ActiveDoc.Lights.Add(newLight);
                }
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
            get { return new Guid("1b08b942-55ff-408a-bf23-6079a1adf691"); }
        }
    }
}