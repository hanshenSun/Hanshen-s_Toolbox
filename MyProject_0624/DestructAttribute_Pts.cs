using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class DestructAttribute_Pts : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the DestructAttribute_Pts class.
        /// </summary>
        public DestructAttribute_Pts()
          : base("DestructAttribute_Pts", "DestructAttribute_Pts",
              "Retrieve Attribute (Userdictionary) of Points",
              "HS_ToolBox", "BIM Tools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Decorated Pts", "pts", "Input List of Points for retriving attributes", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Deconstructed Pt", "pt", "Deconstructed Pt", GH_ParamAccess.item);
            pManager.AddTextParameter("Dictionary Name", "N", "Dictionary Name", GH_ParamAccess.list);
            pManager.AddTextParameter("Dictionary Data", "D", "Dictionary Data", GH_ParamAccess.list);
            
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //List<Point> inputPoint = new List<Point>();
            List<string> dictionaryName = new List<string>();
            List<string> dictionaryData = new List<string>();


            hs_Point inputPt = new hs_Point();

            


            DA.GetData(0, ref inputPt);

            dictionaryName = inputPt.Name;
            dictionaryData = inputPt.Value;
            Point3d deconstructedPt = inputPt.basePt;





            DA.SetData(0, deconstructedPt);
            DA.SetDataList(1, dictionaryName);
            DA.SetDataList(2, dictionaryData);
            




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
                return Properties.Resources.DestructAttribute_Pt;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("70c2364e-1e07-4832-974a-9d6bac3c54e5"); }
        }
    }
}