using System;
using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace MyProject_0624
{
    public class SetIntersectionDiff_String : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public SetIntersectionDiff_String()
          : base("Set Intersection and Difference", "Set Int/Diff",
              "From two lists of strings, find out which indexs are intersecting and the index of those that are not intersecting",
              "HS_ToolBox", "Analysis")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Input Text List A", "List A", "Input Text List A", GH_ParamAccess.list);
            pManager.AddTextParameter("Input Text List B", "List B", "Input Text List B", GH_ParamAccess.list);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("List Intersection", "Intersection", "List of value that exists in both lists", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Index of the found member in List A","indexFoundA", "Index of the found member in List A", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Index of the found member in List B", "indexFoundB", "Index of the found member in List B", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Index of the not found member in List A", "indexNotFoundA", "Index of the not found member in List A", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Index of the not found member in List B", "indexNotFoundB", "Index of the not found member in List B", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string>inputListA = new List<string>();
            List<string>inputListB = new List<string>();

            List<string> sharedList = new List<string>();

            List<int> foundAIndex = new List<int>();
            List<int> foundBIndex = new List<int>();

            List<int> notFoundAIndex = new List<int>();
            List<int> notFoundBIndex = new List<int>();

            DA.GetDataList(0, inputListA);
            DA.GetDataList(1, inputListB);



            //finding the intersection
            IEnumerable<string> IEListA = inputListA;
            IEnumerable<string> IEListB = inputListB;


            IEnumerable<string> IEShared = IEListA.Intersect(IEListB);
            sharedList = IEShared.ToList();

            //looping through ListA
            for (int iA = 0; iA < inputListA.Count(); iA++)
            {
                bool localBoolA = sharedList.Contains(inputListA[iA]);
                if (localBoolA == true)
                {
                    foundAIndex.Add(iA);
                }

                else
                {
                    notFoundAIndex.Add(iA);

                }
            }

            //looping though ListB
            for (int iB = 0; iB < inputListB.Count(); iB++)
            {
                bool localBoolB = sharedList.Contains(inputListB[iB]);
                if (localBoolB == true)
                {
                    foundBIndex.Add(iB);
                }

                else
                {
                    notFoundBIndex.Add(iB);

                }
            }

            DA.SetDataList(0, sharedList);
            DA.SetDataList(1, foundAIndex);
            DA.SetDataList(2, foundBIndex);
            DA.SetDataList(3, notFoundAIndex);
            DA.SetDataList(4, notFoundBIndex);

            

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
                return Properties.Resources.setIntersectionDiff;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b7fb188c-2e63-4063-b6b7-568d6a550f35"); }
        }
    }
}