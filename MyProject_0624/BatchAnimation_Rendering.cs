using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino;
using Rhino.Commands;

namespace MyProject_0624
{
    public class BatchAnimation_Rendering : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the BatchAnimation_Rendering class.
        /// </summary>
        public BatchAnimation_Rendering()
          : base("BatchAnimation_Rendering", "Batch_Render",
              "Automates rendering consecutively using Rhino's current renderer. Code for doing batch rendering for animation using V-Ray, Based on Lauren Vasey's script posted on the Grasshopper3d forum",
              "HS_ToolBox", "ViewPort")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Start?", "Start?", "Set True to start rendering", GH_ParamAccess.item);
            pManager.AddTextParameter("Folder to save renderings", "Dir", "Folder to save renderings", GH_ParamAccess.item);
            pManager.AddTextParameter("File Name", "Name", "(optional) File Name", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Slider for animation sequence", "Slider", "Slider for animation sequence", GH_ParamAccess.item);
            pManager[2].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Finished Time", "t", "Finished Time", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool startBool = new bool();
            string fileDir = "";
            string fileName = "";
            int sequenceIndex = new int();

            DA.GetData(0, ref startBool);
            DA.GetData(1, ref fileDir);
            DA.GetData(2, ref fileName);
            DA.GetData(3, ref sequenceIndex);

            if (sequenceIndex.GetType() != typeof(int))//checking if the sqeuence if integer 
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please make sure the slider outputs integers");
            }

            if (startBool == true)
            {
                string fileLocation = fileDir + "/" + fileName + "_" + sequenceIndex.ToString() + ".png";

                RhinoApp.RunScript("Render", true);
                RhinoApp.RunScript("_-SaveRenderWindowAs \n\"" + fileLocation + "\"\n", true);
                RhinoApp.RunScript("_ - CloseRenderWindow", true);
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
                return Properties.Resources.Batch_Render;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("9d02f94a-b36d-4dd9-8ba7-d7cc03d78ed4"); }
        }
    }
}