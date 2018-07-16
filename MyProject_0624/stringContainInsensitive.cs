using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class stringContainInsensitive : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public stringContainInsensitive()
          : base("StringMatchIndex_String", "Text Match_Index",
              "Getting the index of correspoinding strings in the input list, converted from custom rhino plugin",
              "HS_ToolBox", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Str", "String", "String to be searched from", GH_ParamAccess.item);
            pManager.AddTextParameter("subStr", "sub string", "string to find", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("r", "r", "result", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string inputStr = string.Empty;
            string subStr = string.Empty;
            DA.GetData(0, ref inputStr);
            DA.GetData(1, ref subStr);

            bool tempBool = hs_functions.containInsensitive(inputStr, subStr);
            DA.SetData(0, tempBool);
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
            get { return new Guid("d0fce871-6cbf-447e-a425-cb11759582b7"); }
        }
    }
}