using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class StringMatchIndex_String : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the StringMatchIndex_String class.
        /// </summary>
        public StringMatchIndex_String()
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
            pManager.AddTextParameter("List of text to search from", "Input Text", "List of text to search from", GH_ParamAccess.list);
            pManager.AddTextParameter("Item/items to search", "Key Text", "Key item/items to search", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Found Index", "Found Index", "Found Index from the searched list", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> inputStrs = new List<string>();
            List<string> keyStrs = new List<string>();
            List<int> foundIndex = new List<int>();
            Dictionary<string, int> dictList = new Dictionary<string, int>();


            DA.GetDataList(0, inputStrs);
            DA.GetDataList(1, keyStrs);
            
            for (int i = 0; i< inputStrs.Count; i++)
            {
                dictList.Add(inputStrs[i], i);
            }

            foreach (string keyStr in keyStrs)
            {
                int tempIndex = new int();
                dictList.TryGetValue(keyStr, out tempIndex);
                foundIndex.Add(tempIndex);
            }

            DA.SetDataList(0, foundIndex);
            
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
                return Properties.Resources.StringMatchIndex;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b848e947-8da9-4152-b77d-826e61a3cbba"); }
        }
    }
}