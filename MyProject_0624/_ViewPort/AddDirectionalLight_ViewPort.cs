using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624._ViewPort
{
    public class AddDirectionalLight_ViewPort : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddDirectionalLight_ViewPort class.
        /// </summary>
        public AddDirectionalLight_ViewPort()
          : base("Add Directional Light_ViewPort", "AddDirectionalLight",
              "Adding a new directional light obj into the project",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {

            pManager.AddTextParameter("Name", "Name", "Name", GH_ParamAccess.item);
            pManager.AddPointParameter("Location", "Location", "Location of the Light Object", GH_ParamAccess.item);
            pManager.AddColourParameter("Color", "Diffuse Color", "Diffuse Color of the Light", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Directional?", "Directional Light?", "Input a boolean to determine whether this light object is directional light or not", GH_ParamAccess.item);
            pManager.AddPointParameter("Target", "Target Point", "Target of the Directional Light", GH_ParamAccess.item);
            pManager.AddNumberParameter("Intensity", "Intensity of the light", "Intensity(Min 0.0, Max 1.0) of the light object", GH_ParamAccess.item);
            pManager.AddBooleanParameter("initialize?", "initialize the light", "set true to initialize the light", GH_ParamAccess.item);
            
            //pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddVectorParameter("Light Direction", "Light Direction", "Light Direction", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Light newLight = new Light();
            string lightName = "GH_Light";
            Point3d lightLoc = new Point3d(0, 0, 0);
            Point3d destinationLoc = new Point3d(0, 0, 0);
            bool isDirectional = false;
            bool initializeBool = false;
            double lightIntensity = 1.0;
            Color ambColor = new Color();
            ambColor = Color.Cyan;

            DA.GetData(0, ref lightName);
            DA.GetData(1, ref lightLoc);
            DA.GetData(2, ref ambColor);
            DA.GetData(3, ref isDirectional);
            DA.GetData(4, ref destinationLoc);
            DA.GetData(5, ref lightIntensity);
            DA.GetData(6, ref initializeBool);

            

            newLight.Name = lightName;
            newLight.LightStyle = LightStyle.WorldDirectional;
            newLight.Location = lightLoc;
            newLight.Diffuse = ambColor;
            newLight.Intensity = lightIntensity;
            newLight.IsEnabled = initializeBool;

            

            if (isDirectional == true) 
            {
                if (destinationLoc == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please set a destination pt for the directional light");
                    return;
                }

                else
                {
                    Vector3d directVector = Point3d.Subtract(destinationLoc, lightLoc);
                    newLight.Direction = directVector;
                    DA.SetData(0, directVector);
                }
            }

            if (initializeBool == true)
            {

                List<Guid> foundLightID = hs_functions.getLightObjectIds(lightName);


                if (foundLightID.Count > 0)
                {
                    List<int> foundIDIndex = new List<int>();
                    foreach(Guid id in foundLightID)
                    {
                        Rhino.RhinoDoc.ActiveDoc.Lights.Modify(id, newLight);
                        //Rhino.RhinoDoc.ActiveDoc.Lights. = newLight;
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
            get { return new Guid("ac4ba5c0-b181-4764-90f5-cea025b767f9"); }
        }
    }
}