using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class DestructAttribute_Crv : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public DestructAttribute_Crv()
          : base("Deconstruct Attribute_Crv", "DestructAttribute_Crv",
              "Retrieve Attribute (Userdictionary) to a curve",
              "HS_ToolBox", "BIM Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve for retrieving data", "Crv", "Curve for retrieving data", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Dictionary Name", "N", "Dictionary Name", GH_ParamAccess.list);
            pManager.AddTextParameter("Dictionary Data", "D", "Dictionary Data", GH_ParamAccess.list);
            

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inputCrvs = new List<Curve>();
            List<string> dictionaryName = new List<string>();
            List<string> dictionaryData = new List<string>();
            

            DA.GetDataList(0, inputCrvs);
            
            

            for (int i = 0; i < inputCrvs.Count; i++)
            {

                foreach (string tempKeys in inputCrvs[i].UserDictionary.Keys)
                {

                    //string[] keyName = new string[] {tempKeys};
                    
                    dictionaryName.Add(tempKeys);
                }
                
                foreach (string tempData in inputCrvs[i].UserDictionary.Values)
                {
                    //string[] valueName = new string[] {tempData};
                    dictionaryData.Add(tempData);
                }

                
            }

            DA.SetDataList(0, dictionaryName);
            DA.SetDataList(1, dictionaryData);
            

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
                return Properties.Resources.DestructAttribute_Crv;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("3fd4952f-dda3-4684-b6ee-927a45a8b457"); }
        }
    }
}