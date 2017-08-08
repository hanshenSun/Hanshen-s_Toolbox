using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class AddAttribute_Pts : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the AddAttribute_Pts class.
        /// </summary>
        public AddAttribute_Pts()
          : base("AddAttribute_Pts", "AddAttribute_Pts",
              "Add Attribute (Userdictionary) to points",
              "HS_ToolBox", "BIM Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Pts", "Points to add attributes", GH_ParamAccess.item);
            pManager.AddTextParameter("Name of the Dictionary", "Name", "Dictionary Name", GH_ParamAccess.list);
            pManager.AddTextParameter("Name of the Attribute", "Attribute", "Dictionary Attribute", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Points with Attributes", "Pts", "Points with Attributes", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> dictionaryName = new List<string>();
            List<string> dictionaryData = new List<string>();


            Point3d inputPt = new Point3d();



            DA.GetData(0, ref inputPt);
            DA.GetDataList(1, dictionaryName);
            DA.GetDataList(2, dictionaryData);

            hs_Point newPoint = new hs_Point(inputPt, dictionaryName, dictionaryData);


            DA.SetData(0, newPoint);

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
                return Properties.Resources.AddAttribute_Pt;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("8f9c44de-dfe5-420f-a296-2b6520a01d81"); }
        }
    }
}