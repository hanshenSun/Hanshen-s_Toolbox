using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class staticCompare : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the staticCompare class.
        /// </summary>
        public staticCompare()
          : base("staticCompare", "staticCompare",
              "staticCompare",
              "DGF", "Sub1")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("feedBack", "Serial Feedback", "Serial Feekback", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("outPut", "outPut", "outPut", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string inputText = "";
            string outputText = "";

            if (!DA.GetData(0, ref inputText)) { return; }

            if (inputText.Contains("ok"))
            {
                outputText = "True";
            }

            DA.SetData(0, outputText);
            //doing something
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
            get { return new Guid("f5fea98b-c3ad-453d-8a86-4e7f65588fbd"); }
        }
    }
}