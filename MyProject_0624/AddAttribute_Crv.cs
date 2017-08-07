using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class AddAttribute_Crv : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public AddAttribute_Crv()
          : base("Add Attribute_Crv", "AddAttribute_Crv",
              "Add Attribute (Userdictionary) to curves",
              "HS_ToolBox", "BIM Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("curve for adding attribute", "Crv", "Curve for adding attributes", GH_ParamAccess.item);
            pManager.AddTextParameter("Name of the Dictionary", "Name", "Dictionary Name", GH_ParamAccess.list);
            pManager.AddTextParameter("Name of the Attribute", "Attribute", "Dictionary Attribute", GH_ParamAccess.list);
            


        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Assigned Curve", "Crv", "Curve with assigned attributes",GH_ParamAccess.list);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve inputGeometry = new NurbsCurve(3, 3);
            List<string> attriName = new List<string>();
            List<string> attriData = new List<string>();


            // Then we need to access the input parameters individually. 
            // When data cannot be extracted from a parameter, we should abort this method.
            
            DA.GetData(0, ref inputGeometry);
            DA.GetDataList(1, attriName);
            DA.GetDataList(2, attriData);


            int dictionaryIndex = attriName.Count;

            if (dictionaryIndex != attriData.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Length of dictionary and attribute needs to match");

            }
            

            for (int i = 0; i< dictionaryIndex; i++)
            {
                inputGeometry.UserDictionary.Set(attriName[i], attriData[i]);

            }

            DA.SetData(0, inputGeometry);


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
                return Properties.Resources.AddAttribute_Crv;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("dd7a4a5c-3e0a-4ffd-af13-c0e289930a26"); }
        }
    }
}