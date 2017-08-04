using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class isLinear : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the isLinear class.
        /// </summary>
        public isLinear()
          : base("isLinear", "isLinear",
              "Checking the input list of curves if they rae linear",
              "HS_Toolbox", "lazyTools")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("InputCurves", "Crv", "Curves for checking whether its linear", GH_ParamAccess.list);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Results", "R", "Result returns true if the curve is linear", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inputCrvs = new List<Curve>();
            List<bool> results = new List<bool>();

            DA.GetDataList(0, inputCrvs);
            
            foreach (Curve crv in inputCrvs)
            {
                bool tempResult = crv.IsLinear();

                results.Add(tempResult);

            }
            DA.SetDataList(0, results);

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
                return Properties.Resources.isLinear;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("09ddc509-856f-49af-9245-4f98f248a25f"); }
        }
    }
}