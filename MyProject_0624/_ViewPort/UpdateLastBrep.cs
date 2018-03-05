using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624._ViewPort
{
    public class UpdateLastBrep : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the UpdateLastItem class.
        /// </summary>
        public UpdateLastBrep()
          : base("UpdateLastItem", "UpdateLastItem",
              "Delete the most recent geometry in current rhino file and bake a new one into the doc",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("start?", "start?", "set true to start this operation", GH_ParamAccess.item);
            pManager.AddBrepParameter("Brep", "Brep", "Brep geometry for update", GH_ParamAccess.item);
            pManager.AddTextParameter("Layer Name", "Layer", "Desired layer for baking", GH_ParamAccess.item);
            pManager[2].Optional = true;

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Output Message", "Output", "Output Message", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool startBool = false;
            Brep geometryforUpdate = new Brep();
            string outputMessage = "True";
            Rhino.DocObjects.ObjectAttributes bakeAttribute = new Rhino.DocObjects.ObjectAttributes();
            string layerName = null;
            //layerName.ToString()
            DA.GetData(0, ref startBool);
            DA.GetData(1, ref geometryforUpdate);

            if (layerName == null)
            {
                bakeAttribute.LayerIndex = Rhino.RhinoDoc.ActiveDoc.Layers.CurrentLayerIndex;
            }
            else
            {
                bakeAttribute.LayerIndex = Rhino.RhinoDoc.ActiveDoc.Layers.Find(layerName, true);

            }

            System.Collections.Generic.List<Guid> allBrepsIDs = new List<Guid>();
            allBrepsIDs = hs_functions.getBrepIds(startBool);

            if (allBrepsIDs.Count != 0)
            {
                Guid lastID = allBrepsIDs[0];
                Rhino.RhinoDoc.ActiveDoc.Objects.Delete(lastID, true);//delete most recent brep                
            }
            Rhino.RhinoDoc.ActiveDoc.Objects.AddBrep(geometryforUpdate, bakeAttribute);

            DA.SetData(0, outputMessage);

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
            get { return new Guid("4f1bb41c-67d6-4031-bada-9e6e24e8ce7e"); }
        }
    }
}